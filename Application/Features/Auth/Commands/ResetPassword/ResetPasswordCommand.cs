using Domain.Common;
using MediatR;

namespace Application.Features.Auth.Commands.ResetPassword
{
    public class ResetPasswordCommand : IRequest<Result>
    {
        public string Email { get; set; } = null!;
        public string NewPassword { get; set; } = null!;
        public string OTP { get; set; } = null!;
    }
}
