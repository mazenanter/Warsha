using Domain.Common;

namespace Domain.Entities
{
    public class WorkshopCarBrand : BaseEntity
    {
        public int WorkshopId { get; private set; }
        public int CarBrandId { get; private set; }

        public Workshop Workshop { get; private set; } = null!;
        public CarBrand CarBrand { get; private set; } = null!;

        protected WorkshopCarBrand() { }

        internal static WorkshopCarBrand Create(
            int workshopId,
            int carBrandId)
        {
            return new WorkshopCarBrand
            {
                WorkshopId = workshopId,
                CarBrandId = carBrandId
            };
        }
    }
}
