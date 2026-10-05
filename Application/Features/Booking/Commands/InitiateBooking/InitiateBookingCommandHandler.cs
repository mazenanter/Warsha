using Application.Features.Booking.DTOs;
using Application.Interfaces;
using Domain.Common;
using Domain.Enums;
using MediatR;
using Microsoft.Extensions.Logging;


namespace Application.Features.Booking.Commands.InitiateBooking
{
    public class InitiateBookingCommandHandler
       : IRequestHandler<InitiateBookingCommand, Result<InitiateBookingResult>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICurrentUserService _currentUser;
        private readonly IPaymentGatewayService _paymentGateway;
        private readonly ILogger<InitiateBookingCommandHandler> _logger;

        public InitiateBookingCommandHandler(
            IUnitOfWork unitOfWork,
            ICurrentUserService currentUser,
            IPaymentGatewayService paymentGateway,
            ILogger<InitiateBookingCommandHandler> logger)
        {
            _unitOfWork = unitOfWork;
            _currentUser = currentUser;
            _paymentGateway = paymentGateway;
            _logger = logger;
        }

        public async Task<Result<InitiateBookingResult>> Handle(
            InitiateBookingCommand command, CancellationToken ct)
        {
            var userId =  _currentUser.UserId;


            var client = await _unitOfWork.Clients.FindAsync(x=>x.UserId == userId);
            if (client is null) return Result<InitiateBookingResult>.Failure("Client not found");

            var car = await _unitOfWork.Cars.GetByIdAsync(command.CarId, ct);
            if (car is null || car.ClientId != client.Id)
                return Result<InitiateBookingResult>.Failure("Car not found");

            var workshop = await _unitOfWork.Workshops
                .GetByIdWithServicesAsync(command.WorkshopId, ct);
            if (workshop is null)
                return Result<InitiateBookingResult>.Failure("Workshop not found");

            if (!workshop.CanAcceptBooking())
                return Result<InitiateBookingResult>.Failure(
                    workshop.IsBusy
                        ? $"Workshop is busy until {workshop.BusyUntil:g}"
                        : "Workshop is not accepting bookings");

           

            var services = workshop.Services
                .Where(s => command.ServiceIds.Contains(s.Id) && s.IsVisible)
                .ToList();

            if (services.Count != command.ServiceIds.Distinct().Count())
                return Result<InitiateBookingResult>.Failure(
                    "One or more services not found or unavailable");

            var feeConfig = await _unitOfWork.PlatformFeeConfigs.GetCurrentAsync(ct);
            if (feeConfig is null)
                return Result<InitiateBookingResult>.Failure("Platform fee config not found");

            var bookingNumber = GenerateBookingNumber();
            var booking = Domain.Entities.Booking.Create(
                bookingNumber, client.Id, command.WorkshopId, command.CarId,
                command.ScheduledAt, command.PaymentType,
                feeConfig.ClientBookingFee, command.CustomerNotes);

            foreach (var service in services)
                booking.AddService(service.Id, service.NameEn, service.MinPrice);

            booking.SetCommission(feeConfig.CommissionPct, feeConfig.WorkshopCancellationFee);

            await _unitOfWork.Bookings.AddAsync(booking, ct);
            await _unitOfWork.SaveChangesAsync(ct);

            if (command.PaymentType == PaymentType.CashOnDelivery)
            {
                booking.RegisterCashFees();
                await _unitOfWork.SaveChangesAsync(ct);

                return Result<InitiateBookingResult>.Success(new InitiateBookingResult(
                    booking.Id, booking.BookingNumber, null,
                    booking.TotalAmount, booking.ConfirmationFeeAmount, "Cash"),
                    "Booking confirmed. Pay total amount at the workshop.");
            }

            var paymentReq = new CreatePaymentRequest(
                booking.BookingNumber,
                booking.TotalAmount + booking.ConfirmationFeeAmount,
                client.Email ?? "noemail@warsha.app",
                client.PhoneNumber,
                client.Name.Split(' ').First(),
                client.Name.Split(' ').LastOrDefault() ?? ".");

            var paymentRes = await _paymentGateway.CreatePaymentAsync(paymentReq, ct);
            if (!paymentRes.IsSuccess)
            {
                _logger.LogError("Paymob failed for {BookingNumber}: {Error}",
                    booking.BookingNumber, paymentRes.ErrorMessage);
                return Result<InitiateBookingResult>.Failure(
                    "Payment initialization failed. Please try again.");
            }
            if (string.IsNullOrWhiteSpace(
                    paymentRes.PaymentOrderId))
            {
                _logger.LogError(
                    "Paymob returned no payment order ID for {BookingNumber}",
                    booking.BookingNumber);

                return Result<InitiateBookingResult>.Failure(
                    "Payment initialization failed. Please try again.");
            }
            booking.SetPaymentOrderId(paymentRes.PaymentOrderId!);
            booking.AddPendingTransaction(
                booking.TotalAmount + booking.ConfirmationFeeAmount,
                PaymentMethodType.Visa);

            await _unitOfWork.SaveChangesAsync(ct);

            return Result<InitiateBookingResult>.Success(new InitiateBookingResult(
               booking.Id,
                    booking.BookingNumber,
                    paymentRes.ClientSecret,
                    booking.TotalAmount + booking.ConfirmationFeeAmount,
                    booking.ConfirmationFeeAmount, "Online"),
                "Please complete payment to confirm your booking.");
        }

        private static string GenerateBookingNumber()
        {
            var rnd = Random.Shared.Next(100, 999);
            return $"WRSH-{DateTime.UtcNow:yyMMdd}-{rnd}";
        }
    }

    }
