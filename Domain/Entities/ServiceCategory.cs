using Domain.Common;

namespace Domain.Entities
{
    public class ServiceCategory : BaseEntity
    {
        public string Name { get; private set; } = default!;
        public string Icon { get; private set; } = default!;
        protected ServiceCategory() { }


        public static ServiceCategory Create(string name,string icon)
        {
            if (string.IsNullOrEmpty(name))
            {
                throw new DomainException("Name cannot be empty");
            }
            return new ServiceCategory
            {
                Name = name,
                Icon = icon
            };
        }
    }
}
