using Domain.Common;

namespace Domain.Entities
{
    public class ServiceCategory : BaseEntity
    {
        public string NameEn { get; private set; } = default!;
        public string NameAr { get; private set; } = default!;
        public string Icon { get; private set; } = default!;
        protected ServiceCategory() { }


        public static ServiceCategory Create(string nameEn,string nameAr,string icon)
        {
            if (string.IsNullOrEmpty(nameEn))
            {
                throw new DomainException("Name (English) cannot be empty");
            }
            if (string.IsNullOrEmpty(nameAr))
            {
                throw new DomainException("Name (Arabic) cannot be empty");
            }
            return new ServiceCategory
            {
                NameEn = nameEn,
                NameAr = nameAr,
                Icon = icon
            };
        }
    }
}
