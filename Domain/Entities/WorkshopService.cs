using Domain.Common;

namespace Domain.Entities
{
    public class WorkshopService : BaseEntity
    {
        public string NameEn { get; private set; } = default!;
        public string NameAr { get; private set; } = default!;
        public decimal MinPrice { get; private set; } 
        public decimal MaxPrice { get; private set; } 
        public int WorkshopId { get;private set; }
        public int ServiceCategoryId { get;private set; }
        public ServiceCategory ServiceCategory { get;private set; }
        public Workshop Workshop { get;private set; }
        public int DurationMin { get; private set; } 
        public bool IsVisible { get; private set; } = true;
        public string DescriptionEn { get; private set; }
        public string DescriptionAr { get; private set; }

        protected WorkshopService() { }

        public static WorkshopService Create(string nameEn,string nameAr,decimal minPrice, decimal maxPrice, int durationMin, int workshopId, int serviceCategoryId, string descriptionEn, string descriptionAr)
        {
            if (string.IsNullOrEmpty(nameEn))
             throw new DomainException("Name cannot be null or empty.");
            if (string.IsNullOrEmpty(nameAr))
                throw new DomainException("Name cannot be null or empty.");
            if (minPrice < 0)
                throw new DomainException("MinPrice cannot be negative.");
            if (maxPrice < 0)
                throw new DomainException("MaxPrice cannot be negatice.");
            if (minPrice > maxPrice)
                throw new DomainException("MinPrice cannot be greater than Max Price");
            if (durationMin <= 0)
                throw new DomainException("Duration minutes cannot be negative or zero");

            return new WorkshopService
            {
      
                NameEn = nameEn,
                NameAr = nameAr,
                DescriptionEn = descriptionEn,
                DescriptionAr = descriptionAr,
                ServiceCategoryId = serviceCategoryId,
                WorkshopId = workshopId,
                MinPrice = minPrice,
                MaxPrice = maxPrice,
                DurationMin = durationMin,
                
            };

        }
        public void UpdateData(string nameEn,string nameAr,decimal minPrice,decimal maxPrice,string descriptionEn, string descriptionAr, int duration,int serviceCategoryId)
        {
            if (string.IsNullOrEmpty(nameEn))
                throw new DomainException("Name cannot be null or empty.");
            if (string.IsNullOrEmpty(nameAr))
                throw new DomainException("Name cannot be null or empty.");
            if (minPrice < 0)
                throw new DomainException("MinPrice cannot be negative.");
            if (maxPrice < 0)
                throw new DomainException("MaxPrice cannot be negatice.");
            if (minPrice > maxPrice)
                throw new DomainException("MinPrice cannot be greater than Max Price");
            if (duration <= 0)
                throw new DomainException("Duration minutes cannot be negative or zero");
            NameEn =  nameEn;
            NameAr =  nameAr;
            MinPrice = minPrice;
            MaxPrice = maxPrice;
            DescriptionEn = descriptionEn;
            DescriptionAr = descriptionAr;
            DurationMin = duration;
            ServiceCategoryId = serviceCategoryId;
            UpdatedAt = DateTime.UtcNow;
        }

        public void ToggleVisiblity()
        {
            IsVisible = !IsVisible;
            UpdatedAt = DateTime.UtcNow;

        }
    }
}
