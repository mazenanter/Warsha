using Application.Features.Auth.Commands.CreateClient;
using Application.Features.Auth.DTOs;
using Application.Interfaces;
using Domain.Common;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Application.Features.Auth.Commands.ClientRegister
{
    public class ClientRegiterCommandHandler : IRequestHandler<ClientRegisterCommand, Result<AuthResult>>
    {
        private readonly IAuthService _authService;
        private readonly ILogger<ClientRegiterCommandHandler> _logger;
        private readonly IEmailService _emailService;

        public ClientRegiterCommandHandler(IAuthService authService, ILogger<ClientRegiterCommandHandler> logger, IEmailService emailService)
        {
            _authService = authService;
            _logger = logger;
            _emailService = emailService;
        }

        public async Task<Result<AuthResult>> Handle(ClientRegisterCommand request, CancellationToken cancellationToken)
        {
            var registerRequest = new ClientRegisterRequest
            {
                Email = request.Email,
                Name = request.Name,
                Password = request.Password,
                PhoneNumber = request.PhoneNumber
            };
            var result = await _authService.ClientRegisterAsync(registerRequest);
            if (!result.IsSuccess)
            {
                return result;
            }
            try
            {
                await _emailService.SendOtpAsync(request.Email, request.Name, result.Data!.OTP!);
                _logger.LogInformation($"Email Sent successful To {request.Email}");
            }
            catch (Exception ex)
            {
                _logger.LogError($"An Error accored when sent email to {request.Email} With Message Error {ex}");
                result.Data!.OTP = null;
                return Result<AuthResult>.Success(result.Data!, "Registration successful, but we couldn't send the verification email. Please try to resend it");
            }
            result.Data.OTP = null;
            return result;
        }
    }
}
