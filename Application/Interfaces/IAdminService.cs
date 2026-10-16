using Application.Features.Admin.Commands.AssignUserPermissions;
using Application.Features.Admin.Commands.CreateAdminUser;
using Application.Features.Admin.Commands.CreateEmployee;
using Application.Features.Admin.DTOs;
using Domain.Common;

namespace Application.Interfaces
{
    public interface IAdminService
    {
        Task<Result<CreateAdminAndEmployeeDto>> CreateAdminUser(CreateAdminUserCommand request);
        Task<Result<CreateAdminAndEmployeeDto>> CreateEmployee(CreateEmployeeCommand request);
        Task<Result> AssignUserPermission (AssignUserPermissionsCommand request);
    }
}
