using Application.Interfaces;
using Application.Interfaces.Repositories;
using Domain.Common;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Application.Features.Workshop.Commands.Offers.CreateOffer
{
    public class CreateOfferCommandHandler : IRequestHandler<CreateOfferCommand, Result>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<CreateOfferCommandHandler> _logger;
        private readonly IWorkshopRepository _workshopRepository;
        private readonly ICurrentUserService _currentUserService;

        public CreateOfferCommandHandler(IUnitOfWork unitOfWork, ILogger<CreateOfferCommandHandler> logger, ICurrentUserService currentUserService, IWorkshopRepository workshopRepository)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
            _currentUserService = currentUserService;
            _workshopRepository = workshopRepository;
        }

        public async Task<Result> Handle(CreateOfferCommand request, CancellationToken cancellationToken)
        {
            var userId = _currentUserService.UserId; 
            var workshop = await _workshopRepository.GetByUserId(userId);
            if(workshop == null)
            {
                _logger.LogWarning("Workshop not found");
                return Result.Failure("Workshop not found");
            }
            if (request.serviceId.HasValue)
            {
                workshop.AddServiceOffer(request.serviceId.Value, request.discountPercentage, request.startAt, request.endAt);
            }else
            {
                workshop.AddWorkshopOffer(request.discountPercentage, request.startAt, request.endAt);
            }

            await _unitOfWork.SaveChangesAsync(cancellationToken);
            _logger.LogInformation(
           "Offer created successfully for workshop {WorkshopId}",
           workshop.Id);
            return Result.Success("Offer created successfully.");
        }
    }
}
