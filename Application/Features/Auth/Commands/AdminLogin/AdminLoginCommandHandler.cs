using Application.Features.Auth.DTOs;
using Application.Interfaces;
using Domain.Common;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Application.Features.Auth.Commands.AdminLogin
{
    public class AdminLoginCommandHandler
     : IRequestHandler<AdminLoginCommand, Result<AuthResult>>
    {
        private readonly IAuthService _authService;
        private readonly ILogger<AdminLoginCommandHandler> _logger;

        public AdminLoginCommandHandler(IAuthService authService, ILogger<AdminLoginCommandHandler> logger)
        {
            _authService = authService;
            _logger = logger;
        }

        public async Task<Result<AuthResult>> Handle(AdminLoginCommand request, CancellationToken cancellationToken)
        {
            var result = await _authService.AdminLoginAsync(
            new AdminLoginRequest
            {
                Email = request.Email,
                Password = request.Password
            }
            );

            if (!result.IsSuccess)
            {
                _logger.LogWarning(
                    "Failed admin login attempt for {Email}", request.Email);
                return result;
            }

            _logger.LogInformation(
                "Admin login successful for {Email}", request.Email);

            return result;
        }
    }
}
