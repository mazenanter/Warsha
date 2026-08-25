using Application.Features.Auth.DTOs;
using Domain.Common;
using MediatR;

namespace Application.Features.Auth.Commands.ForgotPassword
{
    public class ForgotPasswordCommand : IRequest<Result<AuthResult>>
    {
        public string Email { get; set; }
    }
}
