using Application.Features.Auth.DTOs;
using Domain.Common;
using MediatR;

namespace Application.Features.Auth.Commands.ResendOtp
{
    public class ResendOtpCommand : IRequest<Result<AuthResult>>
    {
        public string Email { get; set; }
    }
}
