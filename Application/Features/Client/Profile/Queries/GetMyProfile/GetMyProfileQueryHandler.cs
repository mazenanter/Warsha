using Application.Features.Client.Profile.DTOs;
using Application.Interfaces;
using Domain.Common;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.Client.Profile.Queries.GetMyProfile
{
    public class GetMyProfileQueryHandler : IRequestHandler<GetMyProfileQuery, Result<ClientProfileResponseDto>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICurrentUserService _currentUserService;

        public GetMyProfileQueryHandler(IUnitOfWork unitOfWork, ICurrentUserService currentUserService)
        {
            _unitOfWork = unitOfWork;
            _currentUserService = currentUserService;
        }

        public async Task<Result<ClientProfileResponseDto>> Handle(GetMyProfileQuery request, CancellationToken cancellationToken)
        {
            var clientId = _currentUserService.ClientId;
            if (clientId is null)
                return Result<ClientProfileResponseDto>.Failure("Client not found in token");

            var profile = await _unitOfWork.Clients.GetAll()
                .Where(c => c.Id == clientId.Value)
                .Select(c => new ClientProfileResponseDto
                {
                    Id = c.Id,
                    Name = c.Name,
                    Email = c.Email,
                    PhoneNumber = c.PhoneNumber,
                    CreatedAt = c.CreatedAt
                })
                .FirstOrDefaultAsync(cancellationToken);

            if (profile is null)
                return Result<ClientProfileResponseDto>.Failure("Client not found");

            return Result<ClientProfileResponseDto>.Success(profile, "Profile retrieved successfully");
        }
    }
}