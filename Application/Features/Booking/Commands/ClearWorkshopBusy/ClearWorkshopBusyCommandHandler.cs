using Application.Interfaces;
using Domain.Common;
using MediatR;

namespace Application.Features.Booking.Commands.ClearWorkshopBusy
{
    public class ClearWorkshopBusyCommandHandler : IRequestHandler<ClearWorkshopBusyCommand, Result>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICurrentUserService _currentUser;

        public ClearWorkshopBusyCommandHandler(IUnitOfWork unitOfWork, ICurrentUserService currentUser)
        {
            _unitOfWork = unitOfWork;
            _currentUser = currentUser;
        }

        public async Task<Result> Handle(ClearWorkshopBusyCommand command, CancellationToken ct)
        {
            var workshopId = _currentUser.WorkshopId!.Value;
            var workshop = await _unitOfWork.Workshops.GetByIdAsync(workshopId, ct);

            if (workshop is null) return Result.Failure("Workshop not found");

            workshop.ClearBusy();
            await _unitOfWork.Workshops.UpdateAsync(workshop);
            await _unitOfWork.SaveChangesAsync(ct);

            return Result.Success("Workshop is now available for bookings");
        }
    }
}
