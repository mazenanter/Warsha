using Application.Features.Auth.DTOs;
using Application.Interfaces;
using Domain.Common;
using MediatR;

namespace Application.Features.Auth.Commands.WorkshopRegister
{
    public class WorkshopRegisterCommandHandler : IRequestHandler<WorkshopRegisterCommand, Result<AuthResult>>
    {
        private readonly IAuthService _authService;

        public WorkshopRegisterCommandHandler(IAuthService authService)
        {
            _authService = authService;
        }

        public async Task<Result<AuthResult>> Handle(WorkshopRegisterCommand request, CancellationToken cancellationToken)
        {
            var registerRequest = new WorkshopRegisterRequest
            {
                Name = request.Name,
                Phone = request.Phone,
                Address = request.Address,
                Password = request.Password,
         Email = request.Email,
                
            };

            var result  = await _authService.WorkShopRegisterAsync(registerRequest);
            return result;
        }
    }
}
