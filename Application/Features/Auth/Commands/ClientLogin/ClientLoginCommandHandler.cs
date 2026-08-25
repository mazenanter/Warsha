using Application.Features.Auth.DTOs;
using Application.Interfaces;
using Domain.Common;
using MediatR;

namespace Application.Features.Auth.Commands.ClientLogin
{
    public class ClientLoginCommandHandler : IRequestHandler<ClientLoginCommand, Result<AuthResult>>
    {
        private readonly IAuthService _authService;

        public ClientLoginCommandHandler(IAuthService authService)
        {
            _authService = authService;
        }

        public async Task<Result<AuthResult>> Handle(ClientLoginCommand request, CancellationToken cancellationToken)
        {
            var clientLoginRequest = new ClientLoginRequest
            {
                Email = request.Email,
                Password = request.Password
            };
            return await _authService.ClientLoginAsync(clientLoginRequest);
        }
    }
}
