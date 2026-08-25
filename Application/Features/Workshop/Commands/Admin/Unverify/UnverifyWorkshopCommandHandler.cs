using Application.Interfaces;
using Domain.Common;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Application.Features.Workshop.Commands.Admin.Unverify
{
    public class UnverifyWorkshopCommandHandler : IRequestHandler<UnverifyWorkshopCommand, Result>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<UnverifyWorkshopCommandHandler> _logger;

        public UnverifyWorkshopCommandHandler(IUnitOfWork unitOfWork, ILogger<UnverifyWorkshopCommandHandler> logger)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        public async Task<Result> Handle(UnverifyWorkshopCommand request, CancellationToken cancellationToken)
        {
            var workshop = await _unitOfWork.Workshops.GetByIdAsync(request.WorkshopId);
            if (workshop == null) 
            {
                _logger.LogWarning("Workshop with ID {WorkshopId} not found.", request.WorkshopId);
                return Result.Failure($"Workshop with ID {request.WorkshopId} not found.");
            }

            if (!workshop.IsVerified)
            {
                _logger.LogInformation("Workshop with ID {WorkshopId} is already unverified.", request.WorkshopId);
                return Result.Failure($"Workshop with ID {request.WorkshopId} is already unverified.");
            }
            workshop.Unverify();
            await _unitOfWork.Workshops.UpdateAsync(workshop);

            await _unitOfWork.SaveChangesAsync(cancellationToken);
            _logger.LogInformation("Workshop with ID {WorkshopId} has been unverified successfully.", request.WorkshopId);
            return Result.Success($"Workshop with ID {request.WorkshopId} has been unverified successfully.");

        }
    }
}
