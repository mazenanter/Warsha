using Application.Features.Workshop.Commands.Admin.Unverify;
using Application.Features.Workshop.Commands.Admin.Verify;
using Application.Features.Workshop.Commands.Services.AddWorkshopService;
using Application.Features.Workshop.Commands.Services.DeleteWorkshopService;
using Application.Features.Workshop.Commands.Services.ToggleServiceVisibility;
using Application.Features.Workshop.Commands.Services.UpdateWorkshopService;
using Application.Features.Workshop.Commands.Specializations.AddSpecialization;
using Application.Features.Workshop.Commands.Specializations.RemoveSpecialization;
using Application.Features.Workshop.Commands.UpdateProfile;
using Application.Features.Workshop.Commands.UpdateSettings;
using Application.Features.Workshop.Queries.Services.GetAllServices;
using Application.Features.Workshop.Queries.Services.GetServiceById;
using Domain.Constants;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace WebApi.Controllers.Api.V1
{
    [Route("api/v1/[controller]")]
    [ApiController]
    public class WorkshopController : ApiControllerBase
    {

        [HttpPost("verify/{id:int}")]   
        [Authorize(Roles =$"{Roles.SuperAdmin},{Roles.Admin}")]
        public async Task<IActionResult> VerifyWorkshop(int id)
        {
            var command = new VerifyWorkshopCommand { WorkshopId = id };
            var result = await Mediator.Send(command);
            return HandleResult(result);
        }
        [HttpPost("unverify/{id:int}")]
        [Authorize(Roles = $"{Roles.SuperAdmin},{Roles.Admin}")]
        public async Task<IActionResult> UnverifyWorkshop(int id)
        {
            var command = new UnverifyWorkshopCommand { WorkshopId = id };
            var result = await Mediator.Send(command);
            return HandleResult(result);
        }
        [HttpPost("update-profile/{id:int}")]
        [Authorize(Roles = $"{Roles.SuperAdmin},{Roles.Workshop}")]
        public async Task<IActionResult> UpdateWorkshopProfile([FromRoute] int id,[FromBody] UpdateWorkshopProfileCommand command)
        {
            command.WorkshopId = id;    
            var result = await Mediator.Send(command);
            return HandleResult(result);
        }
        [HttpPost("update-settings/{id:int}")]
        [Authorize(Roles = $"{Roles.SuperAdmin},{Roles.Workshop}")]
        public async Task<IActionResult> UpdateWorkshopSettings([FromRoute] int id, [FromBody] UpdateWorkshopSettingsCommand command)
        {
            command.WorkshopId = id;
            var result = await Mediator.Send(command);
            return HandleResult(result);
        }
        [HttpPost("service")]
        [Authorize(Roles = $"{Roles.SuperAdmin},{Roles.Workshop}")]
        public async Task<IActionResult> AddService( [FromBody] AddWorkshopServiceCommand command)
        {
            var result = await Mediator.Send(command);
            return HandleResult(result);
        }
        [HttpPost("me/specialization")]
        [Authorize(Roles = $"{Roles.Workshop}")]
        public async Task<IActionResult> AddSpecialization([FromBody] AddWorkshopSpecializationCommand command)
        {
            var result = await Mediator.Send(command);
            return HandleResult(result);
        }
        [HttpDelete("me/specializations/{specializationId:int}")]
        [Authorize(Roles = Roles.Workshop)]
        public async Task<IActionResult> RemoveSpecialization(
    int specializationId)
        {
            var command = new RemoveWorkshopSpecializationCommand
            {
                SpecializationId = specializationId
            };

            var result = await Mediator.Send(command);

            return HandleResult(result);
        }
        [HttpPut("service/{id:int}")]
        [Authorize(Roles = $"{Roles.SuperAdmin},{Roles.Workshop}")]
        public async Task<IActionResult> AddService([FromRoute] int id,[FromBody] UpdateWorkshopServiceCommand command)
        {
            command.WorkshopServiceId = id;

            var result = await Mediator.Send(command);
            return HandleResult(result);
        }
        [HttpDelete("service/{id:int}")]
        [Authorize(Roles = $"{Roles.SuperAdmin},{Roles.Workshop}")]
        public async Task<IActionResult> DeleteService([FromRoute] int id)
        {
            

            var result = await Mediator.Send(new DeleteWorkshopServiceCommand { WorkshopServiceId = id});
            return HandleResult(result);
        }
        [HttpPut("service/toggle-visibility/{id:int}")]
        [Authorize(Roles = $"{Roles.SuperAdmin},{Roles.Workshop}")]
        public async Task<IActionResult> ToggleVisibility([FromRoute] int id)
        {


            var result = await Mediator.Send(new ToggleServiceVisibilityCommand { WorkshopServiceId = id });
            return HandleResult(result);
        }
        [HttpGet("service")]
        [Authorize(Roles = $"{Roles.SuperAdmin},{Roles.Workshop}")]
        public async Task<IActionResult> GetAllServices([FromQuery]GetAllWorkshopServicesQuery query)
        {


            var result = await Mediator.Send(query);
            return HandleGenericResult(result);
        }
        [HttpGet("service/{id:int}")]
        [Authorize(Roles = $"{Roles.SuperAdmin},{Roles.Workshop}")]
        public async Task<IActionResult> GetAllServices([FromRoute] int id)
        {


            var result = await Mediator.Send(new GetWorkshopServiceByIdQuery { WorkshopId = id});
            return HandleGenericResult(result);
        }
    }
}
