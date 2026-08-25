using Application.Interfaces;
using Domain.Common;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Application.Features.Workshop.Commands.Admin.Verify
{
    public class VerifyWorkshopCommandHandler : IRequestHandler<VerifyWorkshopCommand, Result>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<VerifyWorkshopCommandHandler> _logger;
        private readonly IIdentityService _identityService;

        public VerifyWorkshopCommandHandler(IUnitOfWork unitOfWork, ILogger<VerifyWorkshopCommandHandler> logger, IIdentityService identityService)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
            _identityService = identityService;
   
        }

        public async Task<Result> Handle(VerifyWorkshopCommand request, CancellationToken cancellationToken)
        {
            var workshop = await _unitOfWork.Workshops.GetByIdAsync(request.WorkshopId);
            if (workshop == null) 
            {
                _logger.LogWarning("Workshop with ID {WorkshopId} not found.", request.WorkshopId);
                return Result.Failure($"Workshop with ID {request.WorkshopId} not found.");
            }
            if(workshop.IsVerified)
            {
                _logger.LogInformation("Workshop with ID {WorkshopId} is already verified.", request.WorkshopId);
                return Result.Success($"Workshop with ID {request.WorkshopId} is already verified.");
            }
            if (!await _identityService.IsActive(workshop.UserId))
            {
                _logger.LogWarning("User with ID {UserId} is not active.", workshop.UserId);
                return Result.Failure($"User with ID {workshop.UserId} is not active.");
            }

            workshop.Verify();
            await _unitOfWork.Workshops.UpdateAsync(workshop);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            return Result.Success($"Workshop with ID {request.WorkshopId} has been verified.");
        }
    }
}
