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
            if (car is null || car.ClientId != client.Id ||
    car.IsDeleted)
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
                command.ScheduledAt, command.PaymentType, command.CustomerNotes
                 );


            foreach (var service in services)
                booking.AddService(service.Id, service.NameEn, service.MinPrice);

            booking.SetPlatformFees(
     feeConfig.ClientBookingFee,
     feeConfig.CommissionPct,
     feeConfig.WorkshopCancellationFee);
            var payableAmount =
           booking.TotalAmount ;
            if (payableAmount <= 0)
            {
                return Result<InitiateBookingResult>.Failure(
                    "Booking amount must be greater than zero.");
            }
            await _unitOfWork.Bookings.AddAsync(booking, ct);
            await _unitOfWork.SaveChangesAsync(ct);

            if (command.PaymentType == PaymentType.CashOnDelivery)
            {
                booking.RegisterCashFees();
                await _unitOfWork.SaveChangesAsync(ct);

                return Result<InitiateBookingResult>.Success(new InitiateBookingResult(
                    booking.Id, booking.BookingNumber, null,
                    payableAmount, booking.ConfirmationFeeAmount, "Cash"),
                    "Booking confirmed. Pay total amount at the workshop.");
            }
            var clientName = (client.Name ?? string.Empty)
            .Split(
                ' ',
                StringSplitOptions.RemoveEmptyEntries |
                StringSplitOptions.TrimEntries);

            var firstName = clientName.Length > 0
                ? clientName[0]
                : "Warsha";

            var lastName = clientName.Length > 1
                ? string.Join(" ", clientName.Skip(1))
                : "Client";

            var paymentRequest = new CreatePaymentRequest(
                booking.BookingNumber,
                payableAmount,
                string.IsNullOrWhiteSpace(client.Email)
                    ? "noemail@warsha.app"
                    : client.Email,
                client.PhoneNumber ?? string.Empty,
                firstName,
                lastName);

            CreatePaymentResult paymentRes;


            try
            {
                paymentRes = await _paymentGateway.CreatePaymentAsync(
                    paymentRequest,
                    ct);
            }
            catch (OperationCanceledException) when (ct.IsCancellationRequested)
            {
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "Payment initialization outcome is uncertain for booking {BookingNumber}",
                    booking.BookingNumber);

                return Result<InitiateBookingResult>.Failure(
                    "Payment initialization could not be confirmed. Please check booking status before retrying.");
            }

            if (!paymentRes.IsSuccess ||
            string.IsNullOrWhiteSpace(paymentRes.ClientSecret) ||
            string.IsNullOrWhiteSpace(paymentRes.PaymentOrderId))
            {
                _logger.LogError("Paymob failed for {BookingNumber}: {Error}",
                    booking.BookingNumber, paymentRes.ErrorMessage);
                if (!string.IsNullOrWhiteSpace(paymentRes.ErrorMessage) &&
               paymentRes.ErrorMessage.Contains(
                   "failed",
                   StringComparison.OrdinalIgnoreCase))
                {
                    booking.FailOnlinePayment(
                        paymentRes.ErrorMessage);

                    await _unitOfWork.SaveChangesAsync(ct);
                }

                return Result<InitiateBookingResult>.Failure(
                    "Payment initialization failed. Please try again.");
            }
            booking.SetPaymentOrderId(paymentRes.PaymentOrderId);

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
            payableAmount,
            PaymentMethodType.Visa);

            await _unitOfWork.SaveChangesAsync(ct);

            return Result<InitiateBookingResult>.Success(new InitiateBookingResult(
                booking.Id,
                booking.BookingNumber,
                paymentRes.ClientSecret,
                payableAmount,
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
