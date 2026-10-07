using Application.Features.Auth.DTOs;
using Domain.Common;
using MediatR;

namespace Application.Features.Auth.Commands.WorkshopLogin
{
    public class WorkshopLoginCommand : IRequest<Result<AuthResult>>
    {
        public string Email { get; set; } = null!;
        public string Password { get; set; } = null!;
    }
}
