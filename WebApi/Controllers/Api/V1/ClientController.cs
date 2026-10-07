using Application.Features.Client.Cars.Commands.AddCar;
using Application.Features.Client.Cars.Commands.UpdateCar;
using Application.Features.Client.Cars.Queries.GetAll;
using Application.Features.Client.Cars.Queries.GetById;

using Application.Features.Client.Reminders.Commands.AddReminder;
using Application.Features.Client.Reminders.Commands.DeleteReminder;
using Application.Features.Client.Reminders.Commands.UpdateReminder;
using Application.Features.Client.Reminders.Queries.GetMyReminders;

using Application.Features.Client.SavedWorkshops.Queries.GetSavedWorkshops;

using Application.Features.Client.Workshops.Commands.SaveWorkshop;
using Application.Features.Client.Workshops.Commands.UnsaveWorkshop;
using Application.Features.Client.Workshops.Queries.GetAll;
using Application.Features.Client.Workshops.Queries.GetById;
using Application.Features.Client.Workshops.Queries.GetRecommendedWorkshops;

using Domain.Constants;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace WebApi.Controllers.Api.V1
{
    [Route("api/v1/[controller]")]
    [ApiController]
    public class ClientController : ApiControllerBase
    {
        // =========================
        // Cars
        // =========================

        [HttpPost("cars")]
        [Authorize(Roles = $"{Roles.Client}")]
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
            var result = await Mediator.Send(new GetCarByIdQuery
            {
                CarId = id
            });

            return HandleGenericResult(result);
        }

        [HttpPut("cars")]
        [Authorize(Roles = $"{Roles.Client}")]
        public async Task<IActionResult> UpdateCar([FromBody] UpdateCarCommand command)
        {
            var result = await Mediator.Send(command);
            return HandleResult(result);
        }


        // =========================
        // Workshops
        // =========================

        [HttpGet("workshops")]
        [Authorize(Roles = $"{Roles.Client}")]
        public async Task<IActionResult> GetWorkshopsWithFilter(
            [FromQuery] GetWorkshopsQuery query)
        {
            var result = await Mediator.Send(query);
            return HandleGenericResult(result);
        }

        [HttpGet("workshops/{id:int}")]
        public async Task<IActionResult> GetWorkshopById(
            [FromRoute] int id,
            [FromQuery] GetWorkshopByIdQuery query)
        {
            query.WorkshopId = id;

            var result = await Mediator.Send(query);
            return HandleGenericResult(result);
        }

        [HttpGet("workshops/recommended")]
        public async Task<IActionResult> GetRecommendedWorkshops(
            [FromQuery] GetRecommendedWorkshopsQuery query)
        {
            var result = await Mediator.Send(query);
            return HandleGenericResult(result);
        }


        // =========================
        // Saved Workshops
        // =========================

        [HttpPost("saved-workshops/{workshopId:int}")]
        public async Task<IActionResult> SaveWorkshop(
            [FromRoute] int workshopId)
        {
            var command = new SaveWorkshopCommand
            {
                WorkshopId = workshopId
            };

            var result = await Mediator.Send(command);
            return HandleResult(result);
        }

        [HttpDelete("saved-workshops/{workshopId:int}")]
        public async Task<IActionResult> UnsaveWorkshop(
            [FromRoute] int workshopId)
        {
            var command = new UnsaveWorkshopCommand
            {
                WorkshopId = workshopId
            };

            var result = await Mediator.Send(command);
            return HandleResult(result);
        }

        [HttpGet("saved-workshops")]
        public async Task<IActionResult> GetSavedWorkshops(
            [FromQuery] GetSavedWorkshopsQuery query)
        {
            var result = await Mediator.Send(query);
            return HandleGenericResult(result);
        }


        // =========================
        // Reminders
        // =========================

        [HttpPost("reminders")]
        public async Task<IActionResult> AddReminder(
            [FromBody] AddReminderCommand command)
        {
            var result = await Mediator.Send(command);
            return HandleResult(result);
        }

        [HttpDelete("reminders/{id:int}")]
        public async Task<IActionResult> DeleteReminder(
            [FromRoute] int id)
        {
            var command = new DeleteReminderCommand
            {
                Id = id
            };

            var result = await Mediator.Send(command);
            return HandleResult(result);
        }

        [HttpPut("reminders/{id:int}")]
        public async Task<IActionResult> UpdateReminder(
            [FromRoute] int id,
            [FromBody] UpdateReminderCommand command)
        {
            command.Id = id;

            var result = await Mediator.Send(command);
            return HandleResult(result);
        }

        [HttpGet("reminders")]
        public async Task<IActionResult> GetMyReminders(
            [FromQuery] GetMyRemindersQuery query)
        {
            var result = await Mediator.Send(query);
            return HandleGenericResult(result);
        }
    }
}