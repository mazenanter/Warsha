using Application.Interfaces;
using Domain.Common;
using MediatR;
using Microsoft.Extensions.Logging;
using System.ComponentModel.DataAnnotations;
using System.Globalization;

namespace Application.Features.Workshop.Commands.UpdateProfile
{
    public class UpdateWorkshopProfileCommandHandler : IRequestHandler<UpdateWorkshopProfileCommand, Result>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<UpdateWorkshopProfileCommandHandler> _logger;
        private readonly ICurrentUserService _currentUserService;
        public UpdateWorkshopProfileCommandHandler(IUnitOfWork unitOfWork, ILogger<UpdateWorkshopProfileCommandHandler> logger, ICurrentUserService currentUserService)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
            _currentUserService = currentUserService;
        }

        public async Task<Result> Handle(UpdateWorkshopProfileCommand request, CancellationToken cancellationToken)
        {
            var userId = _currentUserService.UserId;
            var workshop = await _unitOfWork.Workshops.FindAsync(x => x.UserId == userId);
            if(workshop is null)
            {
                _logger.LogWarning("Workshop with this Id not found.");
                return Result.Failure($"Workshop with this Id not found.");
            }
            if(!workshop.IsVerified)
            {
                _logger.LogWarning("Workshop with ID {WorkshopId} is not verified.", workshop.Id);
                return Result.Failure($"Workshop with ID {workshop.Id} is not verified.");
            }
            if (!TimeOnly.TryParseExact(
        request.OpeningTime,
        "hh:mm tt",
        CultureInfo.InvariantCulture,
        DateTimeStyles.None,
        out var openingTime))
            {
                throw new ValidationException("Opening time format is invalid.");
            }

            if (!TimeOnly.TryParseExact(
                    request.ClosingTime,
                    "hh:mm tt",
                    CultureInfo.InvariantCulture,
                    DateTimeStyles.None,
                    out var closingTime))
            {
                throw new ValidationException("Closing time format is invalid.");
            }


            workshop.UpdateDetails(request.Name, request.Phone, request.AddressEn,request.AddressAr,request.DescriptionEn,request.DescriptionAr ,request.Lat, request.Lng, openingTime, closingTime);
           await _unitOfWork.Workshops.UpdateAsync(workshop);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("Workshop with ID {WorkshopId} updated successfully.", workshop.Id);

            return Result.Success("Workshop profile updated successfully.");

        }
    }
}
