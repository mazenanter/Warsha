using Domain.Common;
using Domain.Enums;

namespace Domain.Entities
{
    public class BookingFee : BaseEntity
    {
        public int BookingId { get; private set; }
        public BookingFeeType FeeType { get; private set; }
        public decimal Amount { get; private set; }
        public BookingFeeStatus Status { get; private set; }
        public DateTime? PaidAt { get; private set; }

        protected BookingFee() { }

        public static BookingFee Create(
            int bookingId, BookingFeeType feeType, decimal amount)
        {
            if (amount <= 0)
                throw new DomainException("Fee amount must be positive");

            return new BookingFee
            {
                BookingId = bookingId,
                FeeType = feeType,
                Amount = amount,
                Status = BookingFeeStatus.Pending
            };
        }

        public void MarkPaid()
        {
            Status = BookingFeeStatus.Paid;
            PaidAt = DateTime.UtcNow;
        }

        public void MarkRefunded()
        {
            Status = BookingFeeStatus.Refunded;
            PaidAt = DateTime.UtcNow;
        }

        public void Waive()
        {
            Status = BookingFeeStatus.Waived;
            PaidAt = DateTime.UtcNow;
        }
    }
}
