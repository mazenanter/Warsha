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
using Application.Features.Loyalty.Commands.RedeemReward;
using Application.Features.Loyalty.Queries.GetAvailableRewards;
using Application.Features.Loyalty.Queries.GetMyWallet;
using Application.Features.Loyalty.Queries.GetTransactionHistory;
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
        [HttpGet("loyalty/wallet")]
        [Authorize(Roles = $"{Roles.Client}")]
        public async Task<IActionResult> GetWallet(CancellationToken ct)
        {
            var result = await Mediator.Send(new GetMyWalletQuery(), ct);
            return HandleGenericResult(result);
        }

        [HttpGet("loyalty/transactions")]
        [Authorize(Roles = $"{Roles.Client}")]
        public async Task<IActionResult> GetTransactions(
            [FromQuery] int page = 1, [FromQuery] int pageSize = 20,
            CancellationToken ct = default)
            => HandleGenericResult(await Mediator.Send(
                new GetTransactionHistoryQuery(page, pageSize), ct));

        [HttpGet("loyalty/rewards")]
        [Authorize(Roles = $"{Roles.Client}")]
        public async Task<IActionResult> GetRewards(CancellationToken ct)
            => HandleGenericResult(await Mediator.Send(new GetAvailableRewardsQuery(), ct));

        [HttpPost("loyalty/rewards/{rewardId:int}/redeem")]
        [Authorize(Roles = $"{Roles.Client}")]
        public async Task<IActionResult> Redeem(int rewardId, CancellationToken ct)
            => HandleGenericResult(await Mediator.Send(new RedeemRewardCommand(rewardId), ct));

        [HttpPost("bookings")]
        public async Task<IActionResult> InitiateBooking(
       InitiateBookingCommand command, CancellationToken ct)
       => HandleGenericResult(await Mediator.Send(command, ct));

        [HttpGet("bookings")]
        public async Task<IActionResult> GetMyBookings(
            [FromQuery] string? status,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10,
            CancellationToken ct = default)
            => HandleGenericResult(await Mediator.Send(
                new GetMyBookingsQuery(status, page, pageSize), ct));

        [HttpGet("bookings/{id:int}")]
        public async Task<IActionResult> GetBookingDetails(int id, CancellationToken ct)
            => HandleGenericResult(await Mediator.Send(new GetBookingDetailsQuery(id), ct));

        [HttpPatch("bookings/{id:int}/cancel")]
        public async Task<IActionResult> CancelBooking(
            int id, [FromBody] string? reason, CancellationToken ct)
            => HandleResult(await Mediator.Send(
                new CancelBookingByClientCommand(id, reason), ct));

        [HttpPatch("bookings/{bookingId:int}/quotes/{quoteId:int}/approve")]
        public async Task<IActionResult> ApproveQuote(
            int bookingId, int quoteId, CancellationToken ct)
            => HandleResult(await Mediator.Send(
                new ApproveQuoteCommand(bookingId, quoteId), ct));

        [HttpPatch("bookings/{bookingId:int}/quotes/{quoteId:int}/decline")]
        public async Task<IActionResult> DeclineQuote(
            int bookingId, int quoteId, CancellationToken ct)
            => HandleResult(await Mediator.Send(
                new DeclineQuoteCommand(bookingId, quoteId), ct));
        [HttpPost("device-token")]
        public async Task<IActionResult> RegisterToken(
       RegisterDeviceTokenCommand command, CancellationToken ct)
       => HandleResult(await Mediator.Send(command, ct));

        [HttpGet("notifications")]
        public async Task<IActionResult> GetNotifications(
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 20,
            CancellationToken ct = default)
            => HandleGenericResult(await Mediator.Send(
                new GetNotificationsQuery(page, pageSize), ct));

        [HttpPatch("notifications/{id:int}/read")]
        public async Task<IActionResult> MarkRead(int id, CancellationToken ct)
            => HandleResult(await Mediator.Send(
                new MarkNotificationReadCommand(id), ct));
    }
}
