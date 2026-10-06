using Application.Features.Booking.Commands.CancelByWorkshop;
using Application.Features.Booking.Commands.ClearWorkshopBusy;
using Application.Features.Booking.Commands.Notification;
using Application.Features.Booking.Commands.Quote;
using Application.Features.Booking.Commands.UpdateBookingStatus;
using Application.Features.Booking.Commands.WorkshopBusy;
using Application.Features.Booking.Queries.GetBookingDetails;
using Application.Features.Booking.Queries.GetJobsBoard;
using Application.Features.Booking.Queries.GetNotifications;
using Application.Features.Booking.Queries.GetWorkshopBookings;
using Application.Features.Booking.Queries.GetWorkshopQuotes;
using Application.Features.Client.Cars.Commands.AddCar;
using Application.Features.Loyalty.Commands.UseVoucher;
using Application.Features.Workshop.Commands.Admin.Unverify;
using Application.Features.Workshop.Commands.Admin.Verify;
using Application.Features.Workshop.Commands.Offers.CreateOffer;
using Application.Features.Workshop.Commands.Services.AddWorkshopService;
using Application.Features.Workshop.Commands.Services.DeleteWorkshopService;
using Application.Features.Workshop.Commands.Services.ToggleServiceVisibility;
using Application.Features.Workshop.Commands.Services.UpdateWorkshopService;
using Application.Features.Workshop.Commands.Specializations.AddSpecialization;
using Application.Features.Workshop.Commands.Specializations.RemoveSpecialization;
using Application.Features.Workshop.Commands.UpdateProfile;
using Application.Features.Workshop.Commands.UpdateSettings;
using Application.Features.Workshop.Queries.ServiceCategory.GetAll;
using Application.Features.Workshop.Queries.Services.GetAllServices;
using Application.Features.Workshop.Queries.Services.GetServiceById;
using Domain.Constants;
using Domain.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace WebApi.Controllers.Api.V1
{
    [Route("api/v1/[controller]")]
    [ApiController]
    public class WorkshopController : ApiControllerBase
    {

        
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
        [HttpPost("offer")]
        [Authorize(Roles = $"{Roles.Workshop}")]
        public async Task<IActionResult> AddOffer([FromBody] CreateOfferCommand command)
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
        [HttpPatch("vouchers/{code}/use")]
        [Authorize(Roles = $"{Roles.SuperAdmin},{Roles.Workshop}")]
        public async Task<IActionResult> UseVoucher(
       string code, [FromBody] int bookingId, CancellationToken ct)
       => HandleResult(await Mediator.Send(new UseVoucherCommand(code, bookingId), ct));
        [HttpGet("bookings")]
        public async Task<IActionResult> GetBookings(
       [FromQuery] int page = 1,
       [FromQuery] int pageSize = 20,
       CancellationToken ct = default)
       => HandleGenericResult(await Mediator.Send(
           new GetWorkshopBookingsQuery(page, pageSize), ct));

        [HttpGet("bookings/{id:int}")]
        public async Task<IActionResult> GetBookingDetails(int id, CancellationToken ct)
            => HandleGenericResult(await Mediator.Send(new GetBookingDetailsQuery(id), ct));

        [HttpPatch("bookings/{id:int}/status")]
        public async Task<IActionResult> UpdateStatus(
            int id, [FromBody] JobStatus newStatus, CancellationToken ct)
            => HandleResult(await Mediator.Send(
                new UpdateBookingStatusCommand(id, newStatus), ct));

        [HttpPatch("bookings/{id:int}/cancel")]
        public async Task<IActionResult> CancelBooking(
            int id, [FromBody] string? reason, CancellationToken ct)
            => HandleResult(await Mediator.Send(
                new CancelBookingByWorkshopCommand(id, reason), ct));

        [HttpGet("jobs-board")]
        public async Task<IActionResult> GetJobsBoard(CancellationToken ct)
            => HandleGenericResult(await Mediator.Send(new GetJobsBoardQuery(), ct));

        [HttpPost("bookings/{id:int}/quotes")]
        public async Task<IActionResult> CreateQuote(
            int id, CreateQuoteCommand command, CancellationToken ct)
            => HandleGenericResult(await Mediator.Send(command with { BookingId = id }, ct));

        [HttpGet("quotes")]
        public async Task<IActionResult> GetQuotes(CancellationToken ct)
            => HandleGenericResult(await Mediator.Send(new GetWorkshopQuotesQuery(), ct));

        [HttpPost("busy")]
        public async Task<IActionResult> SetBusy(
            SetWorkshopBusyCommand command, CancellationToken ct)
            => HandleResult(await Mediator.Send(command, ct));

        [HttpDelete("busy")]
        public async Task<IActionResult> ClearBusy(CancellationToken ct)
            => HandleResult(await Mediator.Send(new ClearWorkshopBusyCommand(), ct));

        [HttpGet("notifications")]
        public async Task<IActionResult> GetNotifications(
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 20,
            CancellationToken ct = default)
            => HandleGenericResult(await Mediator.Send(
                new GetNotificationsQuery(page, pageSize), ct));

        [HttpPatch("notifications/{id:int}/read")]
        public async Task<IActionResult> MarkRead(
            int id, CancellationToken ct)
            => HandleResult(await Mediator.Send(
                new MarkNotificationReadCommand(id), ct));
        [HttpGet("service-categories")]
        public async Task<IActionResult> GetServiceCategories(
           CancellationToken ct = default)
           => HandleGenericResult(await Mediator.Send(
               new GetAllServiceCategoriesQuery(), ct));
    }
}
