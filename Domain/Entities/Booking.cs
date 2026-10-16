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
        public decimal ServicesAmount { get; private set; }
        public BookingStatus BookingStatus { get; private set; }
        public string? CustomerNotes { get; private set; }
        public DateTime? ConfirmedAt { get; private set; }
        public DateTime? CompletedAt { get; private set; }
        public DateTime? CancelledAt { get; private set; }
        public string? CancellationReason { get; private set; }
        public JobStatus JobStatus { get; private set; } = JobStatus.New;
        public DateTime? ResponseDeadline { get; private set; }
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

            if (paymentType == PaymentType.Online)
            {
                booking.BookingStatus = BookingStatus.PendingPaymentAuth;
            }
            else
            {
                booking.BookingStatus = BookingStatus.PendingApproval;
                booking.ResponseDeadline = DateTime.UtcNow.AddMinutes(5);
            }

            booking.AddDomainEvent(new BookingCreatedEvent(
    booking.Id, workshopId, clientId, bookingNumber, paymentType));

            return booking;
        }

        public void AddService(int workshopServiceId, string serviceName, decimal price)
        {
            if (BookingStatus != BookingStatus.PendingPaymentAuth &&
        BookingStatus != BookingStatus.PendingApproval)
            {
                throw new DomainException(
                    "Services cannot be modified in the current booking state.");
            }

            var service = BookingService.Create(
                workshopServiceId,
                serviceName,
                price);

            _items.Add(service);

            ServicesAmount += price;
            TotalAmount = ServicesAmount + ConfirmationFeeAmount;

            UpdatedAt = DateTime.UtcNow;
        }
        public void SetPlatformFees(
    decimal confirmationFeePct,
    decimal commissionPct,
    decimal workshopCancellationFee)
        {
            if (confirmationFeePct < 0 || confirmationFeePct > 100)
                throw new DomainException(
                    "Confirmation fee percentage must be between 0 and 100.");

            if (commissionPct < 0 || commissionPct > 100)
                throw new DomainException(
                    "Commission percentage must be between 0 and 100.");

            if (workshopCancellationFee < 0)
                throw new DomainException(
                    "Workshop cancellation fee cannot be negative.");

            ConfirmationFeeAmount = Math.Round(
                ServicesAmount * confirmationFeePct / 100m,
                2,
                MidpointRounding.AwayFromZero);

            CommissionAmount = Math.Round(
                ServicesAmount * commissionPct / 100m,
                2,
                MidpointRounding.AwayFromZero);

            TotalAmount = ServicesAmount + ConfirmationFeeAmount;

            WorkshopCancellationFee = workshopCancellationFee;
            UpdatedAt = DateTime.UtcNow;
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
            if (PaymentType != PaymentType.Online)
                throw new DomainException(
                    "Cash bookings cannot create online payment transactions.");

            if (_paymentTransactions.Any(t =>
                    t.Status is PaymentTransactionStatus.Pending
                        or PaymentTransactionStatus.Authorized
                        or PaymentTransactionStatus.Captured))
            {
                throw new DomainException(
                    "An active payment transaction already exists.");
            }
            var transaction = PaymentTransaction.CreatePending(Id, amount, method);
            _paymentTransactions.Add(transaction);
            return transaction;
        }

        public void MarkAuthorizationSucceeded(string authorizationId)
        {
            if (PaymentType != PaymentType.Online)
                throw new DomainException(
                    "This booking does not use online payment.");

            if (BookingStatus != BookingStatus.PendingPaymentAuth)
                throw new DomainException(
                    "Booking is not awaiting payment authorization.");

            var transaction = _paymentTransactions
                .FirstOrDefault(t =>
                    t.Status == PaymentTransactionStatus.Pending)
                ?? throw new DomainException(
                    "No pending payment transaction found.");

            transaction.MarkAuthorized(authorizationId);

            BookingStatus = BookingStatus.PendingApproval;
            ResponseDeadline = DateTime.UtcNow.AddMinutes(5);
            UpdatedAt = DateTime.UtcNow;
            AddDomainEvent(new BookingAuthSucceededEvent(
      Id, WorkshopId, ClientId, authorizationId));
        }
      
        public void FailOnlinePayment(string reason)
        {
            if (BookingStatus != BookingStatus.PendingPaymentAuth)
                throw new DomainException(
                    "Booking is not awaiting payment authorization.");

            var transaction = _paymentTransactions.FirstOrDefault(
                t => t.Status == PaymentTransactionStatus.Pending);

            transaction?.MarkFailed(reason);

            BookingStatus = BookingStatus.PaymentFailed;
            UpdatedAt = DateTime.UtcNow;

            AddDomainEvent(new BookingPaymentFailedEvent(Id, ClientId));
        }

        public string? AcceptByWorkshop()
        {
            EnsureWorkshopCanRespond();

            if (PaymentType == PaymentType.Online)
            {
                var transaction = _paymentTransactions.FirstOrDefault(
                    t => t.Status == PaymentTransactionStatus.Authorized)
                    ?? throw new DomainException(
                        "No authorized payment was found.");

                if (string.IsNullOrWhiteSpace(transaction.AuthorizationId))
                    throw new DomainException(
                        "Authorization ID is missing.");

          
                BookingStatus = BookingStatus.PendingCapture;
                UpdatedAt = DateTime.UtcNow;

                return transaction.AuthorizationId;
            }

            BookingStatus = BookingStatus.Confirmed;
            ConfirmedAt = DateTime.UtcNow;
            UpdatedAt = DateTime.UtcNow;

            RegisterCashFees();

            AddDomainEvent(new BookingAcceptedByWorkshopEvent(
                Id,
                WorkshopId,
                ClientId,
                TotalAmount,
                CommissionAmount));

            return null;
        }
        public void ConfirmAfterCapture(string captureReference)
        {
            if (PaymentType != PaymentType.Online)
                throw new DomainException(
                    "Only online bookings can be confirmed after capture.");

            if (BookingStatus != BookingStatus.PendingCapture)
                throw new DomainException(
                    "Booking is not awaiting payment capture.");

            if (string.IsNullOrWhiteSpace(captureReference))
                throw new DomainException(
                    "Capture reference cannot be empty.");

            var transaction = _paymentTransactions.FirstOrDefault(
                t => t.Status == PaymentTransactionStatus.Authorized)
                ?? throw new DomainException(
                    "No authorized transaction was found.");

            transaction.MarkCaptured(captureReference);

            BookingStatus = BookingStatus.Confirmed;
            ConfirmedAt = DateTime.UtcNow;
            UpdatedAt = DateTime.UtcNow;

            RegisterOnlineFees();

            AddDomainEvent(new BookingAcceptedByWorkshopEvent(
                Id,
                WorkshopId,
                ClientId,
                TotalAmount,
                CommissionAmount));
        }
        private void RegisterOnlineFees()
        {
            AddFeeIfPositive(
                BookingFeeType.ClientConfirmation,
                ConfirmationFeeAmount,
                markPaid: true);

            AddFeeIfPositive(
                BookingFeeType.WorkshopCommission,
                CommissionAmount);

            AddFeeIfPositive(
                BookingFeeType.WorkshopPayout,
                TotalAmount - CommissionAmount);
        }

        public void RegisterCashFees()
        {
            if (PaymentType == PaymentType.Online)
                throw new DomainException(
                    "This is not a cash booking.");

            if (BookingStatus != BookingStatus.Confirmed)
                throw new DomainException(
                    "Cash fees can only be registered for a confirmed booking.");

            AddFeeIfPositive(
                BookingFeeType.ClientConfirmation,
                ConfirmationFeeAmount);

            AddFeeIfPositive(
                BookingFeeType.WorkshopCommission,
                CommissionAmount);
        }
        private void AddFeeIfPositive(
         BookingFeeType feeType,
         decimal amount,
         bool markPaid = false)
        {
            if (amount <= 0)
                return;

            if (_fees.Any(f => f.FeeType == feeType))
                return;

            var fee = BookingFee.Create(Id, feeType, amount);

            if (markPaid)
                fee.MarkPaid();

            _fees.Add(fee);
        }
        public string? RejectByWorkshop(
    string? reason,
    WorkshopActionDecision decision)
        {
            EnsureWorkshopCanRespond();

            var authorizationId = GetAuthorizedPaymentId();

            BookingStatus = BookingStatus.Rejected;
            CancelledAt = DateTime.UtcNow;
            CancellationReason = reason;
            UpdatedAt = DateTime.UtcNow;

            ApplyWorkshopPenalty(decision);

            AddDomainEvent(new BookingRejectedByWorkshopEvent(
                Id,
                ClientId,
                WorkshopId,
                decision.PenaltyApplied,
                decision.PenaltyAmount));

            return authorizationId;
        }

        public string? AutoRejectDueToTimeout(
            WorkshopActionDecision decision)
        {
            if (BookingStatus != BookingStatus.PendingApproval)
                return null;

            if (ResponseDeadline is null ||
                DateTime.UtcNow < ResponseDeadline.Value)
            {
                throw new DomainException(
                    "The workshop response deadline has not expired.");
            }

            var authorizationId = GetAuthorizedPaymentId();

            BookingStatus = BookingStatus.Rejected;
            CancelledAt = DateTime.UtcNow;
            CancellationReason =
                "Workshop did not respond within the allowed time.";

            UpdatedAt = DateTime.UtcNow;

            ApplyWorkshopPenalty(decision);

            AddDomainEvent(new BookingRejectedByWorkshopEvent(
                Id,
                ClientId,
                WorkshopId,
                decision.PenaltyApplied,
                decision.PenaltyAmount));

            return authorizationId;
        }

        private void EnsureWorkshopCanRespond()
        {
            if (BookingStatus != BookingStatus.PendingApproval)
                throw new DomainException(
                    "Booking is not awaiting workshop approval.");

            if (ResponseDeadline is null ||
                DateTime.UtcNow >= ResponseDeadline.Value)
            {
                throw new DomainException(
                    "The workshop response deadline has expired.");
            }
        }
        private string? GetAuthorizedPaymentId()
        {
            if (PaymentType != PaymentType.Online)
                return null;

            var transaction = _paymentTransactions.FirstOrDefault(
                t => t.Status == PaymentTransactionStatus.Authorized)
                ?? throw new DomainException(
                    "No authorized payment was found.");

            if (string.IsNullOrWhiteSpace(transaction.AuthorizationId))
                throw new DomainException(
                    "Authorization ID is missing.");

            return transaction.AuthorizationId;
        }
        private void ApplyWorkshopPenalty(
    WorkshopActionDecision decision)
        {
            if (!decision.PenaltyApplied)
                return;

            if (decision.PenaltyAmount <= 0)
                throw new DomainException(
                    "Penalty amount must be positive.");

            AddFeeIfPositive(
                BookingFeeType.WorkshopCancellationPenalty,
                decision.PenaltyAmount);
        }
        public void MarkVoided(string authorizationId)
        {
            var transaction = _paymentTransactions.FirstOrDefault(
                t => t.AuthorizationId == authorizationId &&
                     t.Status == PaymentTransactionStatus.Authorized);

            if (transaction is null)
                return;

            transaction.MarkVoided();
            UpdatedAt = DateTime.UtcNow;
        }

        public void CancelByWorkshop(
            string? reason,
            WorkshopActionDecision decision)
        {
            if (BookingStatus != BookingStatus.Confirmed)
                throw new DomainException(
                    "Only confirmed bookings can be cancelled by the workshop.");

            BookingStatus = BookingStatus.Cancelled;
            CancelledAt = DateTime.UtcNow;
            CancellationReason = reason;
            UpdatedAt = DateTime.UtcNow;

            ApplyWorkshopPenalty(decision);

            var refundAmount = 0m;

            if (PaymentType == PaymentType.Online &&
                _paymentTransactions.Any(
                    t => t.Status == PaymentTransactionStatus.Captured))
            {
                refundAmount = TotalAmount ;
            }

            AddDomainEvent(new BookingCancelledByWorkshopEvent(
                Id,
                ClientId,
                WorkshopId,
                refundAmount,
                PaymentType,
                decision.PenaltyApplied,
                decision.PenaltyAmount));
        }

        public void CancelByClient(string? reason = null)
        {
            if (BookingStatus is BookingStatus.Completed
                or BookingStatus.Cancelled
                or BookingStatus.Rejected
                or BookingStatus.PaymentFailed
                or BookingStatus.PendingCapture)
            {
                throw new DomainException(
                    "Cannot cancel this booking in its current state.");
            }
            if (JobStatus != JobStatus.New)
            {
                throw new DomainException(
                    "Booking cannot be cancelled after work has started.");
            }
            //var authorizedTransaction = _paymentTransactions.FirstOrDefault(
            //    t => t.Status == PaymentTransactionStatus.Authorized);

            //if (authorizedTransaction is not null)
            //{
            //    AddDomainEvent(new BookingAuthorizationVoidRequestedEvent(
            //        Id,
            //        authorizedTransaction.AuthorizationId!));
            //}

            BookingStatus = BookingStatus.Cancelled;
            CancelledAt = DateTime.UtcNow;
            CancellationReason = reason;
            UpdatedAt = DateTime.UtcNow;

            if (PaymentType == PaymentType.Online)
            {
                //_fees.FirstOrDefault(
                //    f => f.FeeType == BookingFeeType.ClientConfirmation)
                //    ?.MarkRefunded();

                _fees.FirstOrDefault(
                    f => f.FeeType == BookingFeeType.WorkshopPayout)
                    ?.Waive();

                _fees.FirstOrDefault(
                    f => f.FeeType == BookingFeeType.WorkshopCommission)
                    ?.Waive();
            }

            AddDomainEvent(new BookingCancelledByClientEvent(
                Id,
                ClientId,
                WorkshopId,
                ConfirmationFeeAmount));
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
