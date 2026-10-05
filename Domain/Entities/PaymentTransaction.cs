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
        public string? ProviderReference { get; private set; } 
        public string? FailureReason { get; private set; }
        public DateTime? ProcessedAt { get; private set; }

        protected PaymentTransaction() { }

        public static PaymentTransaction CreatePending(
            int bookingId, decimal amount, PaymentMethodType method)
        {
            if (amount <= 0)
                throw new DomainException("Amount must be positive");

            return new PaymentTransaction
            {
                BookingId = bookingId,
                Amount = amount,
                PaymentMethod = method,
                Status = PaymentTransactionStatus.Pending
            };
        }

        public void MarkSuccessful(string providerReference)
        {
            Status = PaymentTransactionStatus.Successful;
            ProviderReference = providerReference;
            ProcessedAt = DateTime.UtcNow;
        }

        public void MarkFailed(string reason)
        {
            Status = PaymentTransactionStatus.Failed;
            FailureReason = reason;
            ProcessedAt = DateTime.UtcNow;
        }

        public void MarkRefunded()
        {
            Status = PaymentTransactionStatus.Refunded;
            ProcessedAt = DateTime.UtcNow;
        }
    }
}
