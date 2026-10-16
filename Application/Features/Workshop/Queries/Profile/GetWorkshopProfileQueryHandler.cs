using Application.Features.Workshop.DTOs;
using Application.Interfaces;
using Domain.Common;
using MediatR;

namespace Application.Features.Workshop.Queries.Profile
{
    public class GetWorkshopProfileQueryHandler : IRequestHandler<GetWorkshopProfileQuery, Result<WorkshopProfileResponseDto>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICurrentUserService _currentUserService;

        public GetWorkshopProfileQueryHandler(IUnitOfWork unitOfWork, ICurrentUserService currentUserService)
        {
            _unitOfWork = unitOfWork;
            _currentUserService = currentUserService;
        }

        public async Task<Result<WorkshopProfileResponseDto>> Handle(GetWorkshopProfileQuery request, CancellationToken cancellationToken)
        {
            var userId = _currentUserService.UserId;
            var workshop = await _unitOfWork.Workshops.FindAsync(x => x.UserId == userId);

            if (workshop == null)
                return Result<WorkshopProfileResponseDto>.Failure("Workshop not found.");
             
            var responseDto = new WorkshopProfileResponseDto
            {
                Id = workshop.Id,
                Name = workshop.Name,

                DescriptionEn = workshop.DescriptionEn ?? string.Empty,
                DescriptionAr = workshop.DescriptionAr ?? string.Empty,
                AddressEn = workshop.AddressEn,
               AddressAr = workshop.AddressAr,
                PhoneNumber = workshop.Phone,
                Email = workshop.Email,
            };

            return Result<WorkshopProfileResponseDto>.Success(responseDto,"Workshop profile retrieved successfully.");
        }
    }
}
