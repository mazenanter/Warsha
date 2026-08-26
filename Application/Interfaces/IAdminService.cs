using Application.Features.Admin.Commands.AssignUserPermissions;
using Application.Features.Admin.Commands.CreateAdminUser;
using Application.Features.Admin.Commands.CreateEmployee;
using Domain.Common;

namespace Application.Interfaces
{
    public interface IAdminService
    {
        Task<Result> CreateAdminUser(CreateAdminUserCommand request);
        Task<Result> CreateEmployee(CreateEmployeeCommand request);
        Task<Result> AssignUserPermission (AssignUserPermissionsCommand request);
    }
}
