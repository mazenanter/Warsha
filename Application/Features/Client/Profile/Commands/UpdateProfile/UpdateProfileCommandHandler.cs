using Application.Interfaces;
using Domain.Common;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Application.Features.Client.Profile.Commands.UpdateProfile
{
    public class UpdateProfileCommandHandler : IRequestHandler<UpdateProfileCommand, Result>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICurrentUserService _currentUserService;
        private readonly ILogger<UpdateProfileCommandHandler> _logger;

        public UpdateProfileCommandHandler(IUnitOfWork unitOfWork, ICurrentUserService currentUserService, ILogger<UpdateProfileCommandHandler> logger)
        {
            _unitOfWork = unitOfWork;
            _currentUserService = currentUserService;
            _logger = logger;
        }

        public async Task<Result> Handle(UpdateProfileCommand request, CancellationToken cancellationToken)
        {
            var clientId = _currentUserService.ClientId;
            if (clientId is null)
                return Result.Failure("Client not found in token");

            var client = await _unitOfWork.Clients.GetByIdAsync(clientId.Value);
            if (client is null)
            {
                _logger.LogWarning("Client {ClientId} not found", clientId.Value);
                return Result.Failure("Client not found");
            }

            // Email is passed through unchanged until the Identity sync is wired up
            client.UpdateProfile(request.Name, client.Email, request.PhoneNumber);

            await _unitOfWork.Clients.UpdateAsync(client);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("Client {ClientId} profile updated successfully", clientId.Value);
            return Result.Success("Profile updated successfully");
        }
    }
}