using Application.Features.Booking.Commands.ApproveQuote;
using Application.Features.Booking.Commands.CancelBooking;
using Application.Features.Booking.Commands.DeclineQuote;
using Application.Features.Booking.Commands.DeviceToken;
using Application.Features.Booking.Commands.InitiateBooking;
using Application.Features.Booking.Commands.Notification;
using Application.Features.Booking.Queries.GetBookingDetails;
using Application.Features.Booking.Queries.GetMyBookings;
using Application.Features.Booking.Queries.GetNotifications;

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

using Application.Features.Loyalty.Commands.RedeemReward;
using Application.Features.Loyalty.Queries.GetAvailableRewards;
using Application.Features.Loyalty.Queries.GetMyVouchers;
using Application.Features.Loyalty.Queries.GetMyWallet;
using Application.Features.Loyalty.Queries.GetTransactionHistory;

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
        public async Task<IActionResult> AddCar(
            [FromBody] AddCarCommand command)
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
        public async Task<IActionResult> GetCarById(
            [FromRoute] int id)
        {
            var result = await Mediator.Send(
                new GetCarByIdQuery
                {
                    CarId = id
                });

            return HandleGenericResult(result);
        }

        [HttpPut("cars")]
        [Authorize(Roles = $"{Roles.Client}")]
        public async Task<IActionResult> UpdateCar(
            [FromBody] UpdateCarCommand command)
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


        // =========================
        // Loyalty
        // =========================

        [HttpGet("loyalty/wallet")]
        [Authorize(Roles = $"{Roles.Client}")]
        public async Task<IActionResult> GetWallet(
            CancellationToken ct)
        {
            var result = await Mediator.Send(
                new GetMyWalletQuery(),
                ct);

            return HandleGenericResult(result);
        }

        [HttpGet("loyalty/transactions")]
        [Authorize(Roles = $"{Roles.Client}")]
        public async Task<IActionResult> GetTransactions(
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 20,
            CancellationToken ct = default)
        {
            return HandleGenericResult(
                await Mediator.Send(
                    new GetTransactionHistoryQuery(page, pageSize),
                    ct));
        }

        [HttpGet("loyalty/rewards")]
        [Authorize(Roles = $"{Roles.Client}")]
        public async Task<IActionResult> GetRewards(
            CancellationToken ct)
        {
            return HandleGenericResult(
                await Mediator.Send(
                    new GetAvailableRewardsQuery(),
                    ct));
        }

        [HttpPost("loyalty/rewards/{rewardId:int}/redeem")]
        [Authorize(Roles = $"{Roles.Client}")]
        public async Task<IActionResult> Redeem(
            [FromRoute] int rewardId,
            CancellationToken ct)
        {
            return HandleGenericResult(
                await Mediator.Send(
                    new RedeemRewardCommand(rewardId),
                    ct));
        }

        [HttpGet("loyalty/vouchers")]
        public async Task<IActionResult> GetMyVouchers(
            [FromQuery] string? status,
            CancellationToken ct = default)
        {
            return HandleGenericResult(
                await Mediator.Send(
                    new GetMyVouchersQuery(status),
                    ct));
        }


        // =========================
        // Bookings
        // =========================

        [HttpPost("bookings")]
        public async Task<IActionResult> InitiateBooking(
            InitiateBookingCommand command,
            CancellationToken ct)
        {
            return HandleGenericResult(
                await Mediator.Send(command, ct));
        }

        [HttpGet("bookings")]
        public async Task<IActionResult> GetMyBookings(
            [FromQuery] string? status,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10,
            CancellationToken ct = default)
        {
            return HandleGenericResult(
                await Mediator.Send(
                    new GetMyBookingsQuery(status, page, pageSize),
                    ct));
        }

        [HttpGet("bookings/{id:int}")]
        public async Task<IActionResult> GetBookingDetails(
            [FromRoute] int id,
            CancellationToken ct)
        {
            return HandleGenericResult(
                await Mediator.Send(
                    new GetBookingDetailsQuery(id),
                    ct));
        }

        [HttpPatch("bookings/{id:int}/cancel")]
        public async Task<IActionResult> CancelBooking(
            [FromRoute] int id,
            [FromBody] string? reason,
            CancellationToken ct)
        {
            return HandleResult(
                await Mediator.Send(
                    new CancelBookingByClientCommand(id, reason),
                    ct));
        }

        [HttpPatch(
            "bookings/{bookingId:int}/quotes/{quoteId:int}/approve")]
        public async Task<IActionResult> ApproveQuote(
            [FromRoute] int bookingId,
            [FromRoute] int quoteId,
            CancellationToken ct)
        {
            return HandleResult(
                await Mediator.Send(
                    new ApproveQuoteCommand(bookingId, quoteId),
                    ct));
        }

        [HttpPatch(
            "bookings/{bookingId:int}/quotes/{quoteId:int}/decline")]
        public async Task<IActionResult> DeclineQuote(
            [FromRoute] int bookingId,
            [FromRoute] int quoteId,
            CancellationToken ct)
        {
            return HandleResult(
                await Mediator.Send(
                    new DeclineQuoteCommand(bookingId, quoteId),
                    ct));
        }


        // =========================
        // Notifications
        // =========================

        [HttpPost("device-token")]
        public async Task<IActionResult> RegisterToken(
            RegisterDeviceTokenCommand command,
            CancellationToken ct)
        {
            return HandleResult(
                await Mediator.Send(command, ct));
        }

        [HttpGet("notifications")]
        public async Task<IActionResult> GetNotifications(
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 20,
            CancellationToken ct = default)
        {
            return HandleGenericResult(
                await Mediator.Send(
                    new GetNotificationsQuery(page, pageSize),
                    ct));
        }

        [HttpPatch("notifications/{id:int}/read")]
        public async Task<IActionResult> MarkRead(
            [FromRoute] int id,
            CancellationToken ct)
        {
            return HandleResult(
                await Mediator.Send(
                    new MarkNotificationReadCommand(id),
                    ct));
        }
    }
}