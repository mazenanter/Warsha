using Domain.Common;
using MediatR;

namespace Application.Features.Client.Profile.Commands.UpdateProfile
{
    public class UpdateProfileCommand : IRequest<Result>
    {
        public string Name { get; set; } = default!;
        public string PhoneNumber { get; set; } = default!;
    }
}