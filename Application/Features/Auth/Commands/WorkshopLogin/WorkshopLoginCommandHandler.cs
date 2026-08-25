using Application.Features.Auth.DTOs;
using Application.Interfaces;
using Domain.Common;
using MediatR;

namespace Application.Features.Auth.Commands.WorkshopLogin
{
    public class WorkshopLoginCommandHandler : IRequestHandler<WorkshopLoginCommand, Result<AuthResult>>
    {
        private readonly IAuthService _authService;

        public WorkshopLoginCommandHandler(IAuthService authService)
        {
            _authService = authService;
        }

        public async Task<Result<AuthResult>> Handle(WorkshopLoginCommand request, CancellationToken cancellationToken)
        {
            var workshopLoginRequest = new WorkshopLoginRequest
            {
                Password = request.Password,
                UserName = request.UserName
            };
            return await _authService.WorkshopLoginAsync(workshopLoginRequest);
        }
    }
}
