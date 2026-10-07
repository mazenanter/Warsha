using Domain.Common;
using Domain.Enums;

namespace Domain.Entities
{
    public class Quote : BaseEntity
    {
        public int BookingId { get; private set; }
        public QuoteStatus Status { get; private set; } = QuoteStatus.Pending;
        public decimal TotalAmount { get; private set; }
        public string? WorkshopNote { get; private set; }

        private readonly List<QuoteItem> _items = [];
        public IReadOnlyCollection<QuoteItem> Items => _items;

        protected Quote() { }

        public static Quote Create(
            int bookingId,
            List<(string Description, decimal Price)> items,
            string? workshopNote = null)
        {
            if (items == null || items.Count == 0)
                throw new DomainException("Quote must have at least one item");

            var quote = new Quote
            {
                BookingId = bookingId,
                WorkshopNote = workshopNote
            };

            foreach (var (desc, price) in items)
            {
                var item = QuoteItem.Create(desc, price);
                quote._items.Add(item);
                quote.TotalAmount += price;
            }

            return quote;
        }

        public void Approve()
        {
            if (Status != QuoteStatus.Pending)
                throw new DomainException("Only pending quotes can be approved");

            Status = QuoteStatus.Approved;
            UpdatedAt = DateTime.UtcNow;
        }

        public void Decline()
        {
            if (Status != QuoteStatus.Pending)
                throw new DomainException("Only pending quotes can be declined");

            Status = QuoteStatus.Declined;
            UpdatedAt = DateTime.UtcNow;
        }
    }
}
