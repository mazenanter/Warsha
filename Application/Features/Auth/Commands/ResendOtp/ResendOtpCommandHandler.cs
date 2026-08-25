using Application.Features.Auth.DTOs;
using Application.Interfaces;
using Domain.Common;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Application.Features.Auth.Commands.ResendOtp
{
    public class ResendOtpCommandHandler : IRequestHandler<ResendOtpCommand, Result<AuthResult>>
    {
        private readonly IAuthService _authService;
        private readonly IEmailService _emailService;
        private readonly ILogger<ResendOtpCommandHandler> _logger;
        private readonly IUnitOfWork _unitOfWork;

        public ResendOtpCommandHandler(IAuthService authService, IEmailService emailService, ILogger<ResendOtpCommandHandler> logger, IUnitOfWork unitOfWork)
        {
            _authService = authService;
            _emailService = emailService;
            _logger = logger;
            _unitOfWork = unitOfWork;
        }

        public async Task<Result<AuthResult>> Handle(ResendOtpCommand request, CancellationToken cancellationToken)
        {
            var result = await _authService.ResendOtp(request.Email);
            if (!result.IsSuccess)
                return result;

            var client  = await _unitOfWork.Clients.FindAsync(c=>c.UserId == result.Data!.UserId);
            try
            {

                await _emailService.SendOtpAsync(request.Email, client.Name, result.Data!.OTP);
                _logger.LogInformation($"Email sent successful To {request.Email}");
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error when sent email to {request.Email} With error {ex}");
                result.Data!.OTP = null;
                return Result<AuthResult>.Failure("Failed to resend otp please try again");
            }
            result.Data!.OTP = null;
            return result;
        }
    }
}
