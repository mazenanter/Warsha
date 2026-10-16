using Domain.Common;
using Domain.Enums;

namespace Domain.Entities
{
    public class PaymentTransaction : BaseEntity
    {
        public int BookingId { get; private set; }
        public decimal Amount { get; private set; }
        public PaymentMethodType PaymentMethod { get; private set; }
        public PaymentTransactionStatus Status { get; private set; }
        public string? AuthorizationId { get; private set; }
        public string? ProviderReference { get; private set; } 
        public string? FailureReason { get; private set; }
        public DateTime? ProcessedAt { get; private set; }

        protected PaymentTransaction() { }

        public static PaymentTransaction CreatePending(
            int bookingId, decimal amount, PaymentMethodType method)
        {
            if (bookingId <= 0)
                throw new DomainException("Booking ID must be positive.");

            if (amount <= 0)
                throw new DomainException("Amount must be positive.");

            return new PaymentTransaction
            {
                BookingId = bookingId,
                Amount = amount,
                PaymentMethod = method,
                Status = PaymentTransactionStatus.Pending
            };
        }

        public void MarkAuthorized(string authorizationId)
        {
            if (Status != PaymentTransactionStatus.Pending)
                throw new DomainException(
                    "Only pending transactions can be authorized.");

            if (string.IsNullOrWhiteSpace(authorizationId))
                throw new DomainException(
                    "Authorization ID is required.");

            Status = PaymentTransactionStatus.Authorized;
            AuthorizationId = authorizationId;
            ProcessedAt = DateTime.UtcNow;
        }

        public void MarkCaptured(string providerReference)
        {
            if (Status != PaymentTransactionStatus.Authorized)
                throw new DomainException(
                    "Only authorized transactions can be captured.");

            if (string.IsNullOrWhiteSpace(providerReference))
                throw new DomainException(
                    "Capture reference is required.");

            Status = PaymentTransactionStatus.Captured;
            ProviderReference = providerReference;
            ProcessedAt = DateTime.UtcNow;
        }

        public void MarkVoided()
        {
            if (Status != PaymentTransactionStatus.Authorized)
                throw new DomainException(
                    "Only authorized transactions can be voided.");

            Status = PaymentTransactionStatus.Voided;
            ProcessedAt = DateTime.UtcNow;
        }

        public void MarkFailed(string reason)
        {
            if (Status != PaymentTransactionStatus.Pending)
                throw new DomainException(
                    "Only pending transactions can be marked as failed.");

            Status = PaymentTransactionStatus.Failed;
            FailureReason = reason;
            ProcessedAt = DateTime.UtcNow;
        }

        public void MarkRefunded()
        {
            if (Status != PaymentTransactionStatus.Captured)
                throw new DomainException(
                    "Only captured transactions can be refunded.");

            Status = PaymentTransactionStatus.Refunded;
            ProcessedAt = DateTime.UtcNow;
        }

    }
}
