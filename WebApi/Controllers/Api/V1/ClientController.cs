using Application.Features.Client.Cars.Commands.AddCar;
using Application.Features.Client.Cars.Commands.UpdateCar;
using Application.Features.Client.Cars.Queries.GetAll;
using Application.Features.Client.Cars.Queries.GetById;
using Application.Features.Client.Workshops.Queries.GetAll;
using Application.Features.Client.Workshops.Queries.GetById;
using Domain.Constants;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace WebApi.Controllers.Api.V1
{
    [Route("api/v1/[controller]")]
    [ApiController]
    public class ClientController : ApiControllerBase
    {

        [HttpPost("cars")]
        [Authorize(Roles =$"{Roles.Client}")]
        public async Task<IActionResult> AddCar([FromBody] AddCarCommand command)
        {
            var result = await Mediator.Send(command);
            return HandleResult(result);
        }
        [HttpGet("cars")]
        [Authorize(Roles = $"{Roles.Client}")]
        public async Task<IActionResult> GetAllCars()
        {
            var result = await Mediator.Send(new GetAllCarsQuery());
            return HandleGenericResult(result);
        }
        
        [HttpGet("cars/{id:int}")]
        [Authorize(Roles = $"{Roles.Client}")]
        public async Task<IActionResult> GetCarById([FromRoute] int id)
        {
            var result = await Mediator.Send(new GetCarByIdQuery { CarId = id });
            return HandleGenericResult(result);
        }
        [HttpPut("cars")]
        [Authorize(Roles = $"{Roles.Client}")]
        public async Task<IActionResult> UpdateCar([FromBody] UpdateCarCommand command)
        {
            var result = await Mediator.Send(command);
            return HandleResult(result);
        }

        [HttpGet]
        [Authorize(Roles =$"{Roles.Client}")]
        public async Task<IActionResult> GetWorkshopsWithFilter([FromQuery] GetWorkshopsQuery query)
        {
            var result = await Mediator.Send(query);
            return HandleGenericResult(result);
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetWorkshopById([FromRoute] int id, [FromQuery] GetWorkshopByIdQuery query)
        {
            query.WorkshopId = id;
            var result = await Mediator.Send(query);
            return HandleGenericResult(result);
        }
    }
}
