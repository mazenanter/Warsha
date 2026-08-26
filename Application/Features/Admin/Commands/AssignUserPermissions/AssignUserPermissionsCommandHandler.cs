using Application.Interfaces;
using Domain.Common;
using MediatR;

namespace Application.Features.Admin.Commands.AssignUserPermissions
{
    public class AssignUserPermissionsCommandHandler : IRequestHandler<AssignUserPermissionsCommand, Result>
    {
        private readonly IAdminService _adminService;

        public AssignUserPermissionsCommandHandler(IAdminService adminService)
        {
            _adminService = adminService;
        }

        public async Task<Result> Handle(AssignUserPermissionsCommand request, CancellationToken cancellationToken)
        {
            return await _adminService.AssignUserPermission(request);
        }
    }
}
