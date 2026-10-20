using Application.Features.Booking.Commands.AcceptBooking;
using Application.Features.Booking.Commands.CancelByWorkshop;
using Application.Features.Booking.Commands.ClearWorkshopBusy;
using Application.Features.Booking.Commands.Notification;
using Application.Features.Booking.Commands.Quote;
using Application.Features.Booking.Commands.RejectBooking;
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
using Application.Features.Workshop.Commands.Offers.DeleteOffer;
using Application.Features.Workshop.Commands.Services.AddWorkshopService;
using Application.Features.Workshop.Commands.Services.DeleteWorkshopService;
using Application.Features.Workshop.Commands.Services.ToggleServiceVisibility;
using Application.Features.Workshop.Commands.Services.UpdateWorkshopService;
using Application.Features.Workshop.Commands.Specializations.AddSpecialization;
using Application.Features.Workshop.Commands.Specializations.RemoveSpecialization;
using Application.Features.Workshop.Commands.UpdateProfile;
using Application.Features.Workshop.Commands.UpdateSettings;
using Application.Features.Workshop.Commands.UpdateWorkshopImage;
using Application.Features.Workshop.Queries.GetDashboardRevenue;
using Application.Features.Workshop.Queries.GetDashboardStats;
using Application.Features.Workshop.Queries.Offers.GetMyOffers;
using Application.Features.Workshop.Queries.Profile;
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


        [HttpPost("update-profile")]
        [Authorize(Roles = $"{Roles.SuperAdmin},{Roles.Workshop}")]
        public async Task<IActionResult> UpdateWorkshopProfile([FromBody] UpdateWorkshopProfileCommand command)
        {
           
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
        public async Task<IActionResult> AddService([FromBody] AddWorkshopServiceCommand command)
        {
            var result = await Mediator.Send(command);
            return HandleResult(result);
        }
        [HttpPost("me/car-brands")]
        [Authorize(Roles = $"{Roles.Workshop}")]
        public async Task<IActionResult> AddCarBrand([FromBody] AddWorkshopCarBrandCommand command)
        {
            var result = await Mediator.Send(command);
            return HandleResult(result);
        }
        [HttpPost("offer")]
        [Authorize(Roles = $"{Roles.Workshop}")]
        public async Task<IActionResult> AddOffer([FromBody] CreateOfferCommand command)
        {
            var result = await Mediator.Send(command);
            return HandleGenericResult(result);
        }
        [HttpDelete("me/car-brands/{carBrandId:int}")]
        [Authorize(Roles = Roles.Workshop)]
        public async Task<IActionResult> RemoveCarBrand(
    int carBrandId)
        {
            var command = new RemoveWorkshopCarBrandCommand
            {
                CarBrandId = carBrandId
            };

            var result = await Mediator.Send(command);

            return HandleResult(result);
        }
        [HttpPut("service/{id:int}")]
        [Authorize(Roles = $"{Roles.SuperAdmin},{Roles.Workshop}")]
        public async Task<IActionResult> AddService([FromRoute] int id, [FromBody] UpdateWorkshopServiceCommand command)
        {
            command.WorkshopServiceId = id;

            var result = await Mediator.Send(command);
            return HandleResult(result);
        }
        [HttpDelete("service/{id:int}")]
        [Authorize(Roles = $"{Roles.SuperAdmin},{Roles.Workshop}")]
        public async Task<IActionResult> DeleteService([FromRoute] int id)
        {


            var result = await Mediator.Send(new DeleteWorkshopServiceCommand { WorkshopServiceId = id });
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
        public async Task<IActionResult> GetAllServices([FromQuery] GetAllWorkshopServicesQuery query)
        {


            var result = await Mediator.Send(query);
            return HandleGenericResult(result);
        }
        [HttpGet("service/{id:int}")]
        [Authorize(Roles = $"{Roles.SuperAdmin},{Roles.Workshop}")]
        public async Task<IActionResult> GetAllServices([FromRoute] int id)
        {


            var result = await Mediator.Send(new GetWorkshopServiceByIdQuery { WorkshopId = id });
            return HandleGenericResult(result);
        }
        [HttpPatch("vouchers/{code}/use")]
        [Authorize(Roles = $"{Roles.SuperAdmin},{Roles.Workshop}")]
        public async Task<IActionResult> UseVoucher(
       string code, [FromBody] int bookingId, CancellationToken ct)
       => HandleResult(await Mediator.Send(new UseVoucherCommand(code, bookingId), ct));
        [HttpGet("bookings")]
        [Authorize(Roles = Roles.Workshop)]
        public async Task<IActionResult> GetBookings(
       [FromQuery] int page = 1,
       [FromQuery] int pageSize = 20,
       CancellationToken ct = default)
       => HandleGenericResult(await Mediator.Send(
           new GetWorkshopBookingsQuery(page, pageSize), ct));

        [HttpGet("bookings/{id:int}")]
        [Authorize(Roles = Roles.Workshop)]
        public async Task<IActionResult> GetBookingDetails(int id, CancellationToken ct)
            => HandleGenericResult(await Mediator.Send(new GetBookingDetailsQuery(id), ct));

        [HttpPatch("bookings/{id:int}/status")]
        [Authorize(Roles = Roles.Workshop)]
        public async Task<IActionResult> UpdateStatus(
            int id, [FromBody] JobStatus newStatus, CancellationToken ct)
            => HandleResult(await Mediator.Send(
                new UpdateBookingStatusCommand(id, newStatus), ct));

        //[HttpPatch("bookings/{id:int}/cancel")]
        //[Authorize(Roles = Roles.Workshop)]
        //public async Task<IActionResult> CancelBooking(
        //    int id, [FromBody] string? reason, CancellationToken ct)
        //    => HandleResult(await Mediator.Send(
        //        new CancelBookingByWorkshopCommand(id, reason), ct));

        [HttpGet("jobs-board")]
        [Authorize(Roles = Roles.Workshop)]
        public async Task<IActionResult> GetJobsBoard(CancellationToken ct)
            => HandleGenericResult(await Mediator.Send(new GetJobsBoardQuery(), ct));

        [HttpPost("bookings/{id:int}/quotes")]
        [Authorize(Roles = Roles.Workshop)]
        public async Task<IActionResult> CreateQuote(
            int id, CreateQuoteCommand command, CancellationToken ct)
            => HandleGenericResult(await Mediator.Send(command with { BookingId = id }, ct));

        [HttpGet("quotes")]
        [Authorize(Roles = Roles.Workshop)]
        public async Task<IActionResult> GetQuotes(CancellationToken ct)
            => HandleGenericResult(await Mediator.Send(new GetWorkshopQuotesQuery(), ct));

        [HttpPost("busy")]
        [Authorize(Roles = Roles.Workshop)]
        public async Task<IActionResult> SetBusy(
            SetWorkshopBusyCommand command, CancellationToken ct)
            => HandleResult(await Mediator.Send(command, ct));

        [HttpDelete("busy")]
        [Authorize(Roles = Roles.Workshop)]
        public async Task<IActionResult> ClearBusy(CancellationToken ct)
            => HandleResult(await Mediator.Send(new ClearWorkshopBusyCommand(), ct));

        [HttpGet("notifications")]
        [Authorize(Roles = Roles.Workshop)]
        public async Task<IActionResult> GetNotifications(
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 20,
            CancellationToken ct = default)
            => HandleGenericResult(await Mediator.Send(
                new GetNotificationsQuery(page, pageSize), ct));

        [HttpPatch("notifications/{id:int}/read")]
        [Authorize(Roles = Roles.Workshop)]
        public async Task<IActionResult> MarkRead(
            int id, CancellationToken ct)
            => HandleResult(await Mediator.Send(
                new MarkNotificationReadCommand(id), ct));
        [HttpGet("service-categories")]
        [Authorize(Roles = Roles.Workshop)]
        public async Task<IActionResult> GetServiceCategories(
           CancellationToken ct = default)
           => HandleGenericResult(await Mediator.Send(
               new GetAllServiceCategoriesQuery(), ct));

        [HttpGet("profile")]
        [Authorize(Roles = $"{Roles.SuperAdmin},{Roles.Workshop}")]
        public async Task<IActionResult> GetWorkshopProfile(CancellationToken ct)
        {
            var result = await Mediator.Send(new GetWorkshopProfileQuery(), ct);
            return HandleGenericResult(result);
        }
        [HttpPut("image")]
        [Authorize(Roles = Roles.Workshop)]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> UpdateImage(
      IFormFile image,
     CancellationToken cancellationToken)
        {
            var command = new UpdateWorkshopImageCommand(image);

            var result = await Mediator.Send(command, cancellationToken);

            return HandleGenericResult(result);
        }
        [Authorize(Roles = Roles.Workshop)]
        [HttpGet("offers")]
        public async Task<IActionResult> GetMyOffers(
    CancellationToken cancellationToken)
        {
            return HandleGenericResult(
                await Mediator.Send(
                    new GetMyOffersQuery(),
                    cancellationToken));
        }
        [HttpDelete("offers/{offerId:int}")]
        [Authorize(Roles = Roles.Workshop)]
        public async Task<IActionResult> DeleteOffer(
    int offerId,
    CancellationToken cancellationToken)
        {
            return HandleResult(
                await Mediator.Send(
                    new DeleteOfferCommand(offerId),
                    cancellationToken));
        }
        [HttpPatch("bookings/{id:int}/accept")]
        public async Task<IActionResult> AcceptBooking(int id, CancellationToken ct)
    => HandleResult(await Mediator.Send(new AcceptBookingCommand(id), ct));

        [HttpPatch("bookings/{id:int}/reject")]
        public async Task<IActionResult> RejectBooking(
            int id, [FromBody] string? reason, CancellationToken ct)
            => HandleResult(await Mediator.Send(new RejectBookingCommand(id, reason), ct));

        [HttpGet("dashboard/stats")]
        [Authorize(Roles = Roles.Workshop)]
        public async Task<IActionResult> GetDashboardStats()
        {
            var result = await Mediator.Send(new GetDashboardStatsQuery());
            return HandleGenericResult(result);
        }

        [HttpGet("dashboard/revenue")]
        [Authorize(Roles = Roles.Workshop)]
        public async Task<IActionResult> GetDashboardRevenue([FromQuery] GetDashboardRevenueQuery query)
        {
            var result = await Mediator.Send(query);
            return HandleGenericResult(result);
        }
    }
}
