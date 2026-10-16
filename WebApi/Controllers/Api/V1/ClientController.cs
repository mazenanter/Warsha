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
using Application.Features.Client.Cars.Commands.DeleteCar;
using Application.Features.Client.Cars.Commands.UpdateCar;
using Application.Features.Client.Cars.Queries.GetAll;
using Application.Features.Client.Cars.Queries.GetById;
using Application.Features.Client.Profile.Commands.UpdateProfile;
using Application.Features.Client.Profile.Queries.GetMyProfile;
using Application.Features.Client.Reminders.Commands.AddReminder;
using Application.Features.Client.Reminders.Commands.DeleteReminder;
using Application.Features.Client.Reminders.Commands.UpdateReminder;
using Application.Features.Client.Reminders.Queries.GetMyReminders;
using Application.Features.Client.Reviews.Commands.AddReview;
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
        /// <summary>
        /// Get workshops with optional filters such as search term, top rated, near me, offers, and radius.
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        [HttpGet("workshops")]
        [Authorize(Roles = $"{Roles.Client}")]
        public async Task<IActionResult> GetWorkshopsWithFilter(
            [FromQuery] GetWorkshopsQuery query)
        {
            var result = await Mediator.Send(query);
            return HandleGenericResult(result);
        }
        /// <summary>
        /// Get workshop details by ID, including reviews with pagination.
        /// </summary>
        /// <param name="id"></param>
        /// <param name="query"></param>
        /// <returns></returns>
        [HttpGet("workshops/{id:int}")]
        public async Task<IActionResult> GetWorkshopById(
            [FromRoute] int id,
            [FromQuery] GetWorkshopByIdQuery query)
        {
            query.WorkshopId = id;

            var result = await Mediator.Send(query);
            return HandleGenericResult(result);
        }
        /// <summary>
        /// Get recommended workshops based on user's preferences and history, with pagination support.
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
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
        /// <summary>
        /// Save a workshop for the authenticated user, allowing them to easily access it later.
        /// </summary>
        /// <param name="workshopId"></param>
        /// <returns></returns>
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
        /// <summary>
        /// Unsave a previously saved workshop for the authenticated user, removing it from their saved list.
        /// </summary>
        /// <param name="workshopId"></param>
        /// <returns></returns>
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
        /// <summary>
        /// Get a paginated list of workshops that the authenticated user has saved, allowing them to view and manage their saved workshops.
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
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
        /// <summary>
        /// Add a new reminder for the authenticated user, allowing them to set notifications for important events or tasks.
        /// </summary>
        /// <param name="command"></param>
        /// <returns></returns>
        [HttpPost("reminders")]
        public async Task<IActionResult> AddReminder(
            [FromBody] AddReminderCommand command)
        {
            var result = await Mediator.Send(command);
            return HandleResult(result);
        }
        /// <summary>
        /// Delete a reminder by its ID for the authenticated user, removing it from their list of reminders.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
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
        /// <summary>
        /// Update an existing reminder by its ID for the authenticated user, allowing them to modify the title, notes, due date, associated car, and completion status.
        /// </summary>
        /// <param name="id"></param>
        /// <param name="command"></param>
        /// <returns></returns>
        [HttpPut("reminders/{id:int}")]
        public async Task<IActionResult> UpdateReminder(
            [FromRoute] int id,
            [FromBody] UpdateReminderCommand command)
        {
            command.Id = id;

            var result = await Mediator.Send(command);
            return HandleResult(result);
        }
        /// <summary>
        /// Get a paginated list of reminders for the authenticated user, allowing them to view and manage their reminders with optional filters such as search term, due date, and completion status.
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
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
        /// <summary>
        /// Initiate a new booking for the authenticated user, allowing them to select a workshop, car, services, schedule, and payment type, with optional customer notes.
        /// </summary>
        /// <param name="command"></param>
        /// <param name="ct"></param>
        /// <returns></returns>
        [HttpPost("bookings")]
        public async Task<IActionResult> InitiateBooking(
            InitiateBookingCommand command,
            CancellationToken ct)
        {
            return HandleGenericResult(
                await Mediator.Send(command, ct));
        }
        /// <summary>
        /// Get a paginated list of bookings for the authenticated user, with optional filtering by booking status (e.g., pending, confirmed, completed, canceled).
        /// </summary>
        /// <param name="status"></param>
        /// <param name="page"></param>
        /// <param name="pageSize"></param>
        /// <param name="ct"></param>
        /// <returns></returns>
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
        /// <summary>
        ///Get detailed information about a specific booking by its ID for the authenticated user, including workshop details, car information, services, schedule, payment status, and any associated quotes or invoices.
        /// </summary>
        /// <param name="id"></param>
        /// <param name="ct"></param>
        /// <returns></returns>
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
        /// <summary>
        /// Cancel a booking by its ID for the authenticated user, providing an optional reason for cancellation. This action will update the booking status and notify the workshop of the cancellation.
        /// </summary>
        /// <param name="id"></param>
        /// <param name="reason"></param>
        /// <param name="ct"></param>
        /// <returns></returns>
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
        /// <summary>
        /// Approve a quote for a specific booking by its ID and quote ID for the authenticated user. This action will update the quote status to approved and notify the workshop of the approval.
        /// </summary>
        /// <param name="bookingId"></param>
        /// <param name="quoteId"></param>
        /// <param name="ct"></param>
        /// <returns></returns>
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
        /// <summary>
        /// Decline a quote for a specific booking by its ID and quote ID for the authenticated user. This action will update the quote status to declined and notify the workshop of the decline.
        /// </summary>
        /// <param name="bookingId"></param>
        /// <param name="quoteId"></param>
        /// <param name="ct"></param>
        /// <returns></returns>
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
        /// <summary>
        /// Register a device token for push notifications for the authenticated user. This allows the system to send notifications to the user's device based on their preferences and activities.
        /// </summary>
        /// <param name="command"></param>
        /// <param name="ct"></param>
        /// <returns></returns>
        [HttpPost("device-token")]
        public async Task<IActionResult> RegisterToken(
            RegisterDeviceTokenCommand command,
            CancellationToken ct)
        {
            return HandleResult(
                await Mediator.Send(command, ct));
        }
        /// <summary>
        /// Get a paginated list of notifications for the authenticated user, allowing them to view recent updates, alerts, and messages related to their bookings, reminders, and other activities within the application.
        /// </summary>
        /// <param name="page"></param>
        /// <param name="pageSize"></param>
        /// <param name="ct"></param>
        /// <returns></returns>
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
        /// <summary>
        /// Mark a specific notification as read by its ID for the authenticated user. This action will update the notification status to read, allowing the user to manage their notifications and keep track of important updates.
        /// </summary>
        /// <param name="id"></param>
        /// <param name="ct"></param>
        /// <returns></returns>
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
        [HttpDelete("cars/{carId:int}")]
        [Authorize(Roles = Roles.Client)]
        public async Task<IActionResult> DeleteCar(
    int carId,
    CancellationToken cancellationToken)
        {
            var command = new DeleteCarCommand
            {
                CarId = carId
            };

            var result = await Mediator.Send(command, cancellationToken);

            return HandleResult(result);
        }

        [HttpGet("profile")]
        public async Task<IActionResult> GetMyProfile()
        {
            var result = await Mediator.Send(new GetMyProfileQuery());
            return HandleGenericResult(result);
        }

        [HttpPut("profile")]
        public async Task<IActionResult> UpdateProfile([FromBody] UpdateProfileCommand command)
        {
            var result = await Mediator.Send(command);
            return HandleResult(result);
        }


        [HttpPost("workshops/{workshopId:int}/reviews")]
        public async Task<IActionResult> AddReview([FromRoute] int workshopId, [FromBody] AddReviewCommand command)
        {
            command.WorkshopId = workshopId;
            var result = await Mediator.Send(command);
            return HandleResult(result);
        }
    }
}