using Application.Interfaces;
using Domain.Common;
using Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Application.Features.Client.Reviews.Commands.AddReview
{
    public class AddReviewCommandHandler : IRequestHandler<AddReviewCommand, Result>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICurrentUserService _currentUserService;
        private readonly ILogger<AddReviewCommandHandler> _logger;

        public AddReviewCommandHandler(IUnitOfWork unitOfWork, ICurrentUserService currentUserService, ILogger<AddReviewCommandHandler> logger)
        {
            _unitOfWork = unitOfWork;
            _currentUserService = currentUserService;
            _logger = logger;
        }

        public async Task<Result> Handle(AddReviewCommand request, CancellationToken cancellationToken)
        {
            var clientId = _currentUserService.ClientId;
            if (clientId is null)
                return Result.Failure("Client not found in token");

            var workshop = await _unitOfWork.Workshops.GetByIdAsync(request.WorkshopId);
            if (workshop is null || !workshop.IsVerified)
            {
                _logger.LogWarning("Workshop {WorkshopId} not found for review", request.WorkshopId);
                return Result.Failure("Workshop not found");
            }

            var alreadyReviewed = await _unitOfWork.Reviews.FindAsync(
                x => x.ClientId == clientId.Value && x.WorkshopId == request.WorkshopId);

            if (alreadyReviewed is not null)
            {
                _logger.LogWarning("Client {ClientId} already reviewed workshop {WorkshopId}", clientId.Value, request.WorkshopId);
                return Result.Failure("You have already reviewed this workshop");
            }

            await using var transaction = await _unitOfWork.BeginTransactionAsync(cancellationToken);
            try
            {
                var review = Review.Create(request.WorkshopId, clientId.Value, request.Rating, request.Comment);
                await _unitOfWork.Reviews.AddAsync(review);
                await _unitOfWork.SaveChangesAsync(cancellationToken);

                var average = await _unitOfWork.Reviews.GetAll()
                    .Where(r => r.WorkshopId == request.WorkshopId)
                    .AverageAsync(r => r.Rating, cancellationToken);

                workshop.UpdateRating(average);
                await _unitOfWork.Workshops.UpdateAsync(workshop);
                await _unitOfWork.SaveChangesAsync(cancellationToken);

                await transaction.CommitAsync(cancellationToken);
            }
            catch
            {
                await transaction.RollbackAsync(cancellationToken);
                throw;
            }

            _logger.LogInformation("Client {ClientId} reviewed workshop {WorkshopId}", clientId.Value, request.WorkshopId);
            return Result.Success("Review added successfully");
        }
    }
}