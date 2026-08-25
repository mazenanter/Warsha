using Application.Features.Auth.DTOs;
using Domain.Common;
using MediatR;

namespace Application.Features.Auth.Commands.CreateClient
{
    public class ClientRegisterCommand : IRequest<Result<AuthResult>>
    {
        public string Name { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public string Password { get; set; }
    }
}
