using Application.Features.CarCatalog.Commands.AddCarBrand;
using Application.Features.CarCatalog.Commands.AddCarModel;
using Application.Features.CarCatalog.Queries.GetCarBrands;
using Application.Features.CarCatalog.Queries.GetCarModels;
using Application.Features.Client.Cars.Commands.CompleteTrip;
using Application.Features.Client.Cars.Commands.CorrectOdometer;
using Application.Features.Client.Cars.Commands.StartTrip;
using Application.Features.Client.Cars.Queries.GetCarMileage;
using Domain.Constants;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace WebApi.Controllers.Api.V1
{
    [Route("api/v1/[controller]")]
    [ApiController]
    public class CarCatalogController : ApiControllerBase
    {
        [HttpGet("car-brands")]
        [Authorize]
        public async Task<IActionResult> GetAllCarBrands()
        {
            var result = await Mediator.Send(new GetCarBrandsQuery());
            return HandleGenericResult(result);
        }
        [HttpGet("car-brands/{id:int}/models")]
        [Authorize]
        public async Task<IActionResult> GetCarModels([FromRoute] int id)
        {
            var result = await Mediator.Send(new GetCarModelsQuery { CarBrandId = id });
            return HandleGenericResult(result);
        }
        [HttpPost("brands")]
        [Authorize(Roles = $"{Roles.Admin}, {Roles.SuperAdmin}")]
        public async Task<IActionResult> AddCarBrand([FromBody] AddCarBrandCommand command)
        {
            var result = await Mediator.Send(command);
            return HandleResult(result);
        }
        [HttpPost("models")]
        [Authorize(Roles = $"{Roles.Admin}, {Roles.SuperAdmin}")]
        public async Task<IActionResult> AddCarModel([FromBody] AddCarModelCommand command)
        {
            var result = await Mediator.Send(command);
            return HandleResult(result);
        }
        [HttpPost("{carId:int}/trips/start")]
        public async Task<IActionResult> StartTrip(
       [FromBody] StartTripCommand command
       )
        => HandleGenericResult(await Mediator.Send(
           command));


        [HttpPost("trips/{tripId:int}/complete")]
        public async Task<IActionResult> CompleteTrip(
          [FromBody] CompleteTripCommand command)
            => HandleGenericResult(await Mediator.Send(
               command));

        [HttpGet("{carId:int}/mileage")]
        public async Task<IActionResult> GetMileage(int carId, CancellationToken ct)
            => HandleGenericResult(await Mediator.Send(new GetCarMileageQuery(carId), ct));

        [HttpPost("{carId:int}/mileage/correction")]
        public async Task<IActionResult> CorrectOdometer(
           [FromBody] CorrectOdometerCommand command)
            => HandleResult(await Mediator.Send(
                command));
    }
}
