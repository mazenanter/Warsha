using Application.Features.Auth.DTOs;
using Domain.Common;
using MediatR;

namespace Application.Features.Auth.Commands.ClientLogin
{
    public class ClientLoginCommand : IRequest<Result<AuthResult>>
    {
        public string Email { get; set; }
        public string Password { get; set; }
    }
}
