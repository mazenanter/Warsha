using Application.Features.Workshop.DTOs;
using Application.Interfaces;
using Application.Interfaces.Repositories;
using Domain.Common;
using Domain.Entities;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Application.Features.Workshop.Commands.Offers.CreateOffer
{
    public class CreateOfferCommandHandler : IRequestHandler<CreateOfferCommand, Result<OfferCreatedResponseDto>>
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

        public async Task<Result<OfferCreatedResponseDto>> Handle(CreateOfferCommand request, CancellationToken cancellationToken)
        {
            var userId = _currentUserService.UserId;
            var workshop = await _unitOfWork.Workshops
    .GetByUserIdWithServicesAsync(
        userId,
        cancellationToken);
            if (workshop == null)
            {
                _logger.LogWarning("Workshop not found");
                return Result<OfferCreatedResponseDto>.Failure("Workshop not found");
            }
            Offer offer;
            if (request.serviceId.HasValue)
            {
                offer= workshop.AddServiceOffer(request.serviceId.Value, request.discountPercentage, request.startAt, request.endAt);
            }else
            {
                 offer= workshop.AddWorkshopOffer(request.discountPercentage, request.startAt, request.endAt);
            }

            await _unitOfWork.SaveChangesAsync(cancellationToken);
            _logger.LogInformation(
           "Offer created successfully for workshop {WorkshopId}",
           workshop.Id);
            return Result<OfferCreatedResponseDto>.Success(new OfferCreatedResponseDto { Id = offer.Id },"Offer created successfully.");
        }
    }
}
