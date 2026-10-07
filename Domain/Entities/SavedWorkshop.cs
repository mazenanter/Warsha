using Domain.Common;

namespace Domain.Entities
{
    public class SavedWorkshop : BaseEntity
    {
        public int ClientId { get; private set; }
        public Client Client { get; private set; } = default!;

        public int WorkshopId { get; private set; }
        public Workshop Workshop { get; private set; } = default!;

        protected SavedWorkshop() { }

        public static SavedWorkshop Create(int clientId, int workshopId)
        {
            return new SavedWorkshop
            {
                ClientId = clientId,
                WorkshopId = workshopId
            };
        }
    }
}