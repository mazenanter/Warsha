using Domain.Common;
using MediatR;

namespace Application.Features.Auth.Commands.RevokeToken
{
    public class RevokeTokenCommand : IRequest<Result>
    {
        public string Token { get; set; }
    }
}
