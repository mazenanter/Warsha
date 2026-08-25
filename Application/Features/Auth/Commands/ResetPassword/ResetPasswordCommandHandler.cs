using Application.Features.Auth.DTOs;
using Application.Interfaces;
using Domain.Common;
using MediatR;

namespace Application.Features.Auth.Commands.ResetPassword
{
    public class ResetPasswordCommandHandler : IRequestHandler<ResetPasswordCommand, Result>
    {
        private readonly IAuthService _authService;

        public ResetPasswordCommandHandler(IAuthService authService)
        {
            _authService = authService;
        }

        public async Task<Result> Handle(ResetPasswordCommand request, CancellationToken cancellationToken)
        {
            var resetPasswordRequest = new ResetPasswordRequest
            {
                Email = request.Email,
                NewPassword = request.NewPassword,
                Otp = request.OTP
            };
            return await _authService.ResetPassword(resetPasswordRequest);
        }
    }
}
