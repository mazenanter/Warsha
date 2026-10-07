using Application.Interfaces;
using Domain.Common;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Application.Features.Client.Workshops.Commands.UnsaveWorkshop
{
    public class UnsaveWorkshopCommandHandler : IRequestHandler<UnsaveWorkshopCommand, Result>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICurrentUserService _currentUserService;
        private readonly ILogger<UnsaveWorkshopCommandHandler> _logger;

        public UnsaveWorkshopCommandHandler(IUnitOfWork unitOfWork, ICurrentUserService currentUserService, ILogger<UnsaveWorkshopCommandHandler> logger)
        {
            _unitOfWork = unitOfWork;
            _currentUserService = currentUserService;
            _logger = logger;
        }

        public async Task<Result> Handle(UnsaveWorkshopCommand request, CancellationToken cancellationToken)
        {
            var clientId = _currentUserService.ClientId;
            if (clientId is null)
                return Result.Failure("Client not found in token");

            var savedWorkshop = await _unitOfWork.SavedWorkshops.FindAsync(
                x => x.ClientId == clientId.Value && x.WorkshopId == request.WorkshopId);

            if (savedWorkshop is null)
            {
                _logger.LogWarning("Saved workshop {WorkshopId} not found for client {ClientId}", request.WorkshopId, clientId.Value);
                return Result.Failure("Saved workshop not found");
            }

            await _unitOfWork.SavedWorkshops.DeleteAsync(savedWorkshop);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("Workshop {WorkshopId} unsaved successfully by client {ClientId}", request.WorkshopId, clientId.Value);
            return Result.Success("Workshop unsaved successfully");
        }
    }
}