using Application.Interfaces;
using Domain.Common;
using Domain.Entities;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Application.Features.Client.Workshops.Commands.SaveWorkshop
{
    public class SaveWorkshopCommandHandler : IRequestHandler<SaveWorkshopCommand, Result>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICurrentUserService _currentUserService;
        private readonly ILogger<SaveWorkshopCommandHandler> _logger;

        public SaveWorkshopCommandHandler(IUnitOfWork unitOfWork, ICurrentUserService currentUserService, ILogger<SaveWorkshopCommandHandler> logger)
        {
            _unitOfWork = unitOfWork;
            _currentUserService = currentUserService;
            _logger = logger;
        }

        public async Task<Result> Handle(SaveWorkshopCommand request, CancellationToken cancellationToken)
        {
            var clientId = _currentUserService.ClientId;
            if (clientId is null)
                return Result.Failure("Client not found in token");

            var workshop = await _unitOfWork.Workshops.GetByIdAsync(request.WorkshopId);
            if (workshop is null)
            {
                _logger.LogWarning("Workshop with id: {WorkshopId} not found", request.WorkshopId);
                return Result.Failure("Workshop not found");
            }

            var alreadySaved = await _unitOfWork.SavedWorkshops.FindAsync(
                x => x.ClientId == clientId.Value && x.WorkshopId == request.WorkshopId);

            if (alreadySaved is not null)
            {
                _logger.LogWarning("Workshop {WorkshopId} already saved by client {ClientId}", request.WorkshopId, clientId.Value);
                return Result.Failure("Workshop already saved");
            }

            var savedWorkshop = SavedWorkshop.Create(clientId.Value, request.WorkshopId);
            await _unitOfWork.SavedWorkshops.AddAsync(savedWorkshop);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("Workshop {WorkshopId} saved successfully by client {ClientId}", request.WorkshopId, clientId.Value);
            return Result.Success("Workshop saved successfully");
        }
    }
}