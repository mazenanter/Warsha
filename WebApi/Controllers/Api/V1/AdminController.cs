using Application.Features.Admin.Commands.CreateAdminUser;
using Application.Features.Admin.Commands.CreateEmployee;
using Application.Features.Admin.Queries.GetAllPermissions;
using Application.Features.Admin.Queries.GetUserPermissions;
using Application.Features.Auth.Commands.AdminLogin;
using Application.Features.Workshop.Commands.Admin.Unverify;
using Application.Features.Workshop.Commands.Admin.Verify;
using Domain.Constants;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using WebApi.Authorization;

namespace WebApi.Controllers.Api.V1
{
    [Route("api/v1/[controller]")]
    [ApiController]
    public class AdminController : ApiControllerBase
    {
        [HttpPost("login")]
        public async Task<IActionResult> Login(AdminLoginCommand command)
        {
            var result = await Mediator.Send(command);
            return HandleGenericResult(result);
        }
        [HttpPost("create-admin")]
        [Authorize(Roles.SuperAdmin)]
        public async Task<IActionResult> CreateAdmin(CreateAdminUserCommand command)
        {
            var result = await Mediator.Send(command);
            return HandleResult(result);
        }
        [HttpPost("create-employee")]
        [Authorize(Roles = $"{Roles.SuperAdmin},{Roles.Admin}")]
        public async Task<IActionResult> CreateEmployee(CreateEmployeeCommand command)
        {
            var result = await Mediator.Send(command);
            return HandleResult(result);
        }
        [HttpPost("verify-workshop/{id:int}")]
        [HasPermission(Permissions.Workshops.Verify)]
        public async Task<IActionResult> VerifyWorkshop(int id)
        {
            var command = new VerifyWorkshopCommand { WorkshopId = id };
            var result = await Mediator.Send(command);
            return HandleResult(result);
        }
        [HttpPost("unverify-workshop/{id:int}")]
        [HasPermission(Permissions.Workshops.UnVerify)]
        public async Task<IActionResult> UnverifyWorkshop(int id)
        {
            var command = new UnverifyWorkshopCommand { WorkshopId = id };
            var result = await Mediator.Send(command);
            return HandleResult(result);
        }
        [HttpGet("permissions")]
        [Authorize(Roles= Roles.SuperAdmin)]
        public async Task<IActionResult> GetAllPermissions()
        {
            var result = await Mediator.Send(new GetAllPermissionsQuery());
            return HandleGenericResult(result);
        }
        [HttpGet("user-permissions/{id:int}")]
        [Authorize(Roles = Roles.SuperAdmin)]
        public async Task<IActionResult> GetUserPermissions([FromRoute] int id)
        {
            var result = await Mediator.Send(new GetUserPermissionsQuery { UserId = id});
            return HandleGenericResult(result);
        }
    }
}
