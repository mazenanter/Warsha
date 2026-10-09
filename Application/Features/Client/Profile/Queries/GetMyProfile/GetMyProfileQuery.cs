using Application.Features.Client.Profile.DTOs;
using Domain.Common;
using MediatR;

namespace Application.Features.Client.Profile.Queries.GetMyProfile
{
    public class GetMyProfileQuery : IRequest<Result<ClientProfileResponseDto>>
    {
    }
}