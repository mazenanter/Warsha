using Application.Features.Auth.DTOs;
using Domain.Common;
using MediatR;

namespace Application.Features.Auth.Commands.WorkshopRegister
{
    public class WorkshopRegisterCommand : IRequest<Result<AuthResult>>
    {
        public string Name { get; set; }
        public string Phone { get; set; }
        public string Address { get; set; }
        public string Password { get; set; }
      
    }
}
