using Domain.Common;

namespace Domain.Entities
{
    public class WorkshopSpecialization : BaseEntity
    {
        public int WorkshopId { get; private set; }
        public int SpecializationId { get; private set; }
        public Workshop Workshop { get; private set; }
        public Specialization Specialization { get; private set; }
        protected WorkshopSpecialization() { }


        internal static WorkshopSpecialization Create(int specializationId, int workshopId)
        {
            return new WorkshopSpecialization
            {
                SpecializationId = specializationId,
                WorkshopId = workshopId
            };
        }
    }
}
