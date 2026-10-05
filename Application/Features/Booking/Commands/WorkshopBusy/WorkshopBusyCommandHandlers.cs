using Application.Interfaces;
using Domain.Common;
using MediatR;

namespace Application.Features.Booking.Commands.WorkshopBusy
{
    public class SetWorkshopBusyCommandHandler : IRequestHandler<SetWorkshopBusyCommand, Result>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICurrentUserService _currentUser;

        public SetWorkshopBusyCommandHandler(IUnitOfWork unitOfWork, ICurrentUserService currentUser)
        {
            _unitOfWork = unitOfWork;
            _currentUser = currentUser;
        }

        public async Task<Result> Handle(SetWorkshopBusyCommand command, CancellationToken ct)
        {
            var workshopId = _currentUser.WorkshopId!.Value;
            var workshop = await _unitOfWork.Workshops.GetByIdAsync(workshopId, ct);

            if (workshop is null) return Result.Failure("Workshop not found");

            workshop.SetBusy(command.BusyFrom, command.BusyUntil);
            await _unitOfWork.Workshops.UpdateAsync(workshop);
            await _unitOfWork.SaveChangesAsync(ct);

            return Result.Success($"Workshop set as busy until {command.BusyUntil:g}");
        }
    }
}
