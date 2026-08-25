using Application.Features.Specialization.Commands.Active;
using Application.Features.Specialization.Commands.Add;
using Application.Features.Specialization.Commands.Delete;
using Application.Features.Specialization.Commands.Update;
using Application.Features.Specialization.Queries.GetById;
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
    public class SpecializationController : ApiControllerBase
    {
        [HttpPost]
        [Authorize(Roles =$"{Roles.SuperAdmin},{Roles.Admin}")]
        public async Task<IActionResult> Add(AddSpecializationCommand command)
        {
            var result = await Mediator.Send(command);
            return HandleResult(result);
        }
        [HttpPut("{id:int}")]
        [Authorize(Roles = $"{Roles.SuperAdmin},{Roles.Admin}")]
        public async Task<IActionResult> Update([FromRoute]int id,UpdateSpecializationCommand command)
        {
            command.Id = id;
            var result = await Mediator.Send(command);
            return HandleResult(result);
        }
        [HttpPut("inactive/{id:int}")]
        [Authorize(Roles = $"{Roles.SuperAdmin},{Roles.Admin}")]
        public async Task<IActionResult> DeActive([FromRoute] int id)
        {
         
            var result = await Mediator.Send(new InActiveSpecializationCommand { Id = id});
            return HandleResult(result);
        }
        [HttpPut("active/{id:int}")]
        [Authorize(Roles = $"{Roles.SuperAdmin},{Roles.Admin}")]
        public async Task<IActionResult> Active([FromRoute] int id)
        {

            var result = await Mediator.Send(new ActiveSpecializationCommand { Id = id });
            return HandleResult(result);
        }
        [HttpGet]
        [Authorize]
        public async Task<IActionResult> GetAllSpecializations([FromQuery] GetAllWorkshopServicesQuery query)
        {
            var result = await Mediator.Send(query);
            return HandleGenericResult(result);
        }
        [HttpGet("{id:int}")]
        [Authorize]
        public async Task<IActionResult> GetSpecializationById([FromRoute] int id)
        {


            var result = await Mediator.Send(new GetSpecializationByIdQuery { Id = id });
            return HandleGenericResult(result);
        }
    }
}
