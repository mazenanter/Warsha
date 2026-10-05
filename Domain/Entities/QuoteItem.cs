using Domain.Common;

namespace Domain.Entities
{
    public class QuoteItem : BaseEntity
    {
        public int QuoteId { get; private set; }
        public string Description { get; private set; } = default!;
        public decimal Price { get; private set; }

        protected QuoteItem() { }

        public static QuoteItem Create(string description, decimal price)
        {
            if (string.IsNullOrWhiteSpace(description))
                throw new DomainException("Description is required");

            if (price <= 0)
                throw new DomainException("Price must be positive");

            return new QuoteItem { Description = description, Price = price };
        }
    }
}
