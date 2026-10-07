using Domain.Common;
using Domain.Enums;
using Domain.Events;

namespace Domain.Entities
{
    public class Booking : BaseAggregateRoot
    {
        public string BookingNumber { get; private set; } = default!;
        public int ClientId { get;private set; }
        public Client Client { get;private set; }
        public int WorkshopId { get;private set; }
        public Workshop Workshop { get;private set; }
        public int CarId { get;private set; }
        public Car Car { get;private set; }
        public DateTime ScheduledAt { get;private set; }
        public decimal TotalAmount { get;private set; }
        public decimal ConfirmationFeeAmount { get;private set; }
        public decimal CommissionAmount { get;private set; }
        public decimal WorkshopCancellationFee { get; private set; }
        public PaymentType PaymentType { get;private set; }
        public string? PaymentOrderId { get; private set; }

        public BookingStatus BookingStatus { get; private set; } = BookingStatus.Pending;
        public string? CustomerNotes { get; private set; }
        public DateTime? ConfirmedAt { get; private set; }
        public DateTime? CompletedAt { get; private set; }
        public DateTime? CancelledAt { get; private set; }
        public string? CancellationReason { get; private set; }
        public JobStatus JobStatus { get; private set; } = JobStatus.New;
        private readonly List<BookingService> _items = [];
        public IReadOnlyCollection<BookingService> Items => _items;
        private readonly List<Quote> _quotes = [];
        private readonly List<BookingFee> _fees = [];
        public IReadOnlyCollection<BookingFee> Fees => _fees;

        public IReadOnlyCollection<Quote> Quotes => _quotes;
        private readonly List<PaymentTransaction> _paymentTransactions = [];
        public IReadOnlyCollection<PaymentTransaction> PaymentTransactions => _paymentTransactions;

        protected Booking() { }

        public static Booking Create(string bookingNumber,int clientId,int workshopId,int carId, DateTime scheduledAt, PaymentType paymentType,  string? customerNotes)
        {
            if(string.IsNullOrWhiteSpace(bookingNumber))
            {
                throw new DomainException("Booking number cannot be null or empty.");
            }
            if(clientId <= 0)
            {
                throw new DomainException("Client ID must be a positive integer.");
            }
            if(workshopId <= 0)
            {
                throw new DomainException("Workshop ID must be a positive integer.");
            }
            if (carId <=0)
            {
                throw new DomainException("Car ID must be a positive integer.");
            }
            if(scheduledAt <= DateTime.UtcNow)
            {
                throw new DomainException("Scheduled date must be in the future.");
            }
           

            var booking = new Booking
            {
                BookingNumber = bookingNumber,
                ClientId = clientId,
                WorkshopId = workshopId,
                CarId = carId,
                ScheduledAt = scheduledAt,
                PaymentType = paymentType,
                CustomerNotes = customerNotes
            };

            if (paymentType == PaymentType.CashOnDelivery)
            {
                booking.BookingStatus = BookingStatus.Confirmed;
                booking.ConfirmedAt = DateTime.UtcNow;
            }

            booking.AddDomainEvent(new BookingCreatedEvent(
    booking.Id, workshopId, clientId, bookingNumber, paymentType));

            return booking;
        }

        public void AddService(int workshopServiceId, string serviceName, decimal price)
        {
            var item = BookingService.Create(workshopServiceId, serviceName, price);
            _items.Add(item);
            TotalAmount += price;
        }
        public void SetPlatformFees(
    decimal confirmationFeePct,
    decimal commissionPct,
    decimal workshopCancellationFee)
        {
            if (confirmationFeePct < 0 || confirmationFeePct > 100)
                throw new DomainException(
                    "Confirmation fee percentage must be between 0 and 100");

            if (commissionPct < 0 || commissionPct > 100)
                throw new DomainException(
                    "Commission must be between 0 and 100");

            if (workshopCancellationFee < 0)
                throw new DomainException(
                    "Cancellation fee cannot be negative");

            ConfirmationFeeAmount =
                Math.Round(TotalAmount * confirmationFeePct / 100, 2);

            CommissionAmount =
                Math.Round(TotalAmount * commissionPct / 100, 2);

            WorkshopCancellationFee = workshopCancellationFee;
        }
        //public void SetCommission(decimal commissionPct, decimal workshopCancellationFee)
        //{
        //    if (commissionPct < 0 || commissionPct > 100)
        //        throw new DomainException("Commission must be between 0 and 100");

        //    CommissionAmount = Math.Round(TotalAmount * commissionPct / 100, 2);
        //    WorkshopCancellationFee = workshopCancellationFee;
        //}
        public void SetPaymentOrderId(string orderId)
        {
            PaymentOrderId = orderId;
            UpdatedAt = DateTime.UtcNow;
        }

        public PaymentTransaction AddPendingTransaction(decimal amount, PaymentMethodType method)
        {
            var transaction = PaymentTransaction.CreatePending(Id, amount, method);
            _paymentTransactions.Add(transaction);
            return transaction;
        }
        public void ConfirmOnlinePayment(string providerReference)
        {
            if (PaymentType != PaymentType.Online)
                throw new DomainException("This booking is not an online payment booking");

            if (BookingStatus != BookingStatus.Pending)
                throw new DomainException("Booking is not awaiting payment");

            var pendingTx = _paymentTransactions
                .FirstOrDefault(t => t.Status == PaymentTransactionStatus.Pending)
                ?? throw new DomainException("No pending transaction found");

            pendingTx.MarkSuccessful(providerReference);

            BookingStatus = BookingStatus.Confirmed;
            ConfirmedAt = DateTime.UtcNow;


            var confirmFee = BookingFee.Create(Id, BookingFeeType.ClientConfirmation, ConfirmationFeeAmount);
            _fees.Add(confirmFee);
            confirmFee.MarkPaid();

            _fees.Add(BookingFee.Create(Id, BookingFeeType.WorkshopCommission, CommissionAmount));

            var payoutAmount = TotalAmount - CommissionAmount;
            _fees.Add(BookingFee.Create(Id, BookingFeeType.WorkshopPayout, payoutAmount));

            AddDomainEvent(new BookingConfirmedEvent(
                Id, WorkshopId, ClientId, TotalAmount, CommissionAmount));
        }
        public void FailOnlinePayment(string reason)
        {
            if (BookingStatus != BookingStatus.Pending)
                throw new DomainException("Booking is not awaiting payment");

            var pendingTx = _paymentTransactions
                .FirstOrDefault(t => t.Status == PaymentTransactionStatus.Pending);

            pendingTx?.MarkFailed(reason);

            BookingStatus = BookingStatus.Failed;
            AddDomainEvent(new BookingPaymentFailedEvent(Id, ClientId));
        }
        public void RegisterCashFees()
        {
            if (PaymentType != PaymentType.CashOnDelivery)
                throw new DomainException("This is not a cash booking");
            _fees.Add(BookingFee.Create(Id, BookingFeeType.ClientConfirmation, ConfirmationFeeAmount));
            _fees.Add(BookingFee.Create(Id, BookingFeeType.WorkshopCommission, CommissionAmount));
        }
       
        public void CancelByWorkshop(string? reason = null)
        {
            if (BookingStatus is BookingStatus.Completed or BookingStatus.Cancelled)
                throw new DomainException("Cannot cancel this booking");

            BookingStatus = BookingStatus.Cancelled;
            CancelledAt = DateTime.UtcNow;
            CancellationReason = reason;

            _fees.Add(BookingFee.Create(Id, BookingFeeType.CancellationPenalty, ConfirmationFeeAmount));
            if (PaymentType == PaymentType.Online)
            {
                var fullRefund = TotalAmount + ConfirmationFeeAmount;
                _fees.Add(BookingFee.Create(
                    Id, BookingFeeType.ClientRefund, fullRefund));
            }

            AddDomainEvent(new BookingCancelledByWorkshopEvent(
       Id, ClientId, WorkshopId,
       TotalAmount + ConfirmationFeeAmount,  
       PaymentType));
        }
        private static readonly Dictionary<JobStatus, JobStatus> _validTransitions = new()
        {
            [JobStatus.New] = JobStatus.Diagnosing,
            [JobStatus.Diagnosing] = JobStatus.InProgress,
            [JobStatus.InProgress] = JobStatus.Ready,
            [JobStatus.Ready] = JobStatus.Completed
        };
        public void UpdateJobStatus(JobStatus newStatus)
        {
            if (BookingStatus != BookingStatus.Confirmed)
                throw new DomainException("Can only update status of confirmed bookings");

            if (!_validTransitions.TryGetValue(JobStatus, out var expectedNext) || expectedNext != newStatus)
                throw new DomainException($"Invalid transition: {JobStatus} → {newStatus}");

            JobStatus = newStatus;

            if (newStatus == JobStatus.Completed)
            {
                BookingStatus = BookingStatus.Completed;
                CompletedAt = DateTime.UtcNow;

                if (PaymentType == PaymentType.Online)
                {
                    var payout = _fees
                        .FirstOrDefault(f => f.FeeType == BookingFeeType.WorkshopPayout);
                }

                AddDomainEvent(new BookingCompletedEvent(Id, ClientId, WorkshopId, CarId));
            }

            AddDomainEvent(new JobStatusUpdatedEvent(Id, ClientId, newStatus));
        }
        public Quote CreateQuote(
       List<(string Description, decimal Price)> items,
       string? note = null)
        {
            if (JobStatus != JobStatus.InProgress)
                throw new DomainException("Can only create quotes for in-progress jobs");

            if (_quotes.Any(q => q.Status == QuoteStatus.Pending))
                throw new DomainException("There is already a pending quote");

            var quote = Quote.Create(Id, items, note);
            _quotes.Add(quote);

            AddDomainEvent(new QuoteCreatedEvent(Id, ClientId, quote.TotalAmount));
            return quote;
        }
        public void CancelByClient(string? reason = null)
        {
            if (BookingStatus is BookingStatus.Completed or BookingStatus.Cancelled)
                throw new DomainException("Cannot cancel this booking");


            BookingStatus = BookingStatus.Cancelled;
            CancelledAt = DateTime.UtcNow;
            CancellationReason = reason;

            if ( PaymentType == PaymentType.Online)
            {
                _fees.FirstOrDefault(f => f.FeeType == BookingFeeType.ClientConfirmation)?.MarkRefunded();
                _fees.FirstOrDefault(f => f.FeeType == BookingFeeType.WorkshopPayout)?.Waive();
                _fees.FirstOrDefault(f => f.FeeType == BookingFeeType.WorkshopCommission)?.Waive();
            }

            AddDomainEvent(new BookingCancelledByClientEvent(
                Id, ClientId, WorkshopId, ConfirmationFeeAmount));
        }
        public void ApproveQuote(int quoteId)
        {
            var quote = _quotes.FirstOrDefault(q => q.Id == quoteId)
                ?? throw new DomainException("Quote not found");

            quote.Approve();

            AddDomainEvent(new QuoteRespondedEvent(Id, WorkshopId, quoteId, QuoteStatus.Approved));
        }

        public void DeclineQuote(int quoteId)
        {
            var quote = _quotes.FirstOrDefault(q => q.Id == quoteId)
                ?? throw new DomainException("Quote not found");

            quote.Decline();

            AddDomainEvent(new QuoteRespondedEvent(Id, WorkshopId, quoteId, QuoteStatus.Declined));
        }
    }
}
