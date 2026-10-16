namespace Domain.Enums
{
    public enum BookingStatus
    {
        PendingPaymentAuth = 1,
        PendingApproval = 2,
        PendingCapture = 3,
        Confirmed = 4,
        Rejected = 5,
        Cancelled = 6,
        Completed = 7,
        PaymentFailed = 8

    }
}
