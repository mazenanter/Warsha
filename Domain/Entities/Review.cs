using Domain.Common;

namespace Domain.Entities
{
    public class Review
    {
        public int Id { get; private set; } 
        public int WorkshopId { get; private set; }
        public Workshop Workshop { get; private set; } = default!;

        public int ClientId { get; private set; }
        public Client Client { get; private set; } = default!;

        public int Rating { get; private set; }
        public string? Comment { get; private set; }
        public DateTime CreatedAt { get; set; }


        protected Review() { }

        public static Review Create(int workshopId, int clientId, int rating, string? comment)
        {
            if (rating < 1 || rating > 5)
                throw new DomainException("Rating must be between 1 and 5.");

            return new Review
            {
                WorkshopId = workshopId,
                ClientId = clientId,
                Rating = rating,
                Comment = comment,
                CreatedAt = DateTime.UtcNow
            };
        }

    }
}
