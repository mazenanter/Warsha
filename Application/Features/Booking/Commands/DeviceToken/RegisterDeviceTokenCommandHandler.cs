using Application.Interfaces;
using Domain.Common;
using Domain.Entities;
using MediatR;

namespace Application.Features.Booking.Commands.DeviceToken
{
    public class RegisterDeviceTokenCommandHandler : IRequestHandler<RegisterDeviceTokenCommand, Result>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICurrentUserService _currentUser;

        public RegisterDeviceTokenCommandHandler(IUnitOfWork unitOfWork, ICurrentUserService currentUser)
        {
            _unitOfWork = unitOfWork;
            _currentUser = currentUser;
        }

        public async Task<Result> Handle(RegisterDeviceTokenCommand command, CancellationToken ct)
        {
            var userId = _currentUser.UserId;

            var existing = await _unitOfWork.DeviceTokens
                .GetByUserAndDeviceAsync(
                    userId,
                    command.DeviceId,
                    ct);

            if (existing != null)
            {
                existing.Update(
                    command.Token,
                    command.Platform);
            }
            else
            {
                var deviceToken = Domain.Entities.DeviceToken.Create(
       userId,
       command.Token,
       command.Platform,
       command.DeviceId);


                await _unitOfWork.DeviceTokens.AddAsync(
                    deviceToken,
                    ct);
            }

            await _unitOfWork.SaveChangesAsync(ct);
            return Result.Success("Device token registered");
        }
    }
}
