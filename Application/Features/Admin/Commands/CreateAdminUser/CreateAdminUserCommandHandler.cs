using Application.Interfaces;
using Domain.Common;
using MediatR;

namespace Application.Features.Admin.Commands.CreateAdminUser
{
    public class CreateAdminUserCommandHandler : IRequestHandler<CreateAdminUserCommand, Result>
    {
        private readonly IAdminService _adminService;

        public CreateAdminUserCommandHandler(IAdminService adminService)
        {
            _adminService = adminService;
        }

        public async Task<Result> Handle(CreateAdminUserCommand request, CancellationToken cancellationToken)
        {
            return await _adminService.CreateAdminUser(request);
        }
    }
}
