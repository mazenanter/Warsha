using Application.Features.Auth.DTOs;
using Application.Interfaces;
using Domain.Common;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Application.Features.Auth.Commands.ForgotPassword
{
    public class ForgotPasswordCommandHandler : IRequestHandler<ForgotPasswordCommand, Result<AuthResult>>
    {
        private readonly IAuthService _authService;
        private readonly IEmailService _emailService;
        private readonly ILogger<ForgotPasswordCommandHandler> _logger;
        private readonly IUnitOfWork _unitOfWork;

        public ForgotPasswordCommandHandler(IAuthService authService, IEmailService emailService, ILogger<ForgotPasswordCommandHandler> logger, IUnitOfWork unitOfWork)
        {
            _authService = authService;
            _emailService = emailService;
            _logger = logger;
            _unitOfWork = unitOfWork;
        }

        public async Task<Result<AuthResult>> Handle(ForgotPasswordCommand request, CancellationToken cancellationToken)
        {
            var result = await _authService.ForgotPassword(request.Email);
            if (!result.IsSuccess)
                return result;

            var client = await _unitOfWork.Clients.FindAsync(c => c.UserId == result.Data!.UserId);
            try
            {
                await _emailService.SendOtpAsync(request.Email, client.Name, result!.Data!.OTP);
                _logger.LogInformation($"Email sent successful to {request.Email}");
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error when sent email to {request.Email} with error {ex}");
                result.Data.OTP = null;
                return Result<AuthResult>.Failure("Error to sent email please try again");
            }
            result.Data.OTP = null;
            return result;
        }
    }
}
