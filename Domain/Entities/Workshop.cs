using Domain.Common;

namespace Domain.Entities
{
    public class Workshop : BaseAggregateRoot
    {
        public int UserId { get; private set; }
        public string Name { get; private set; } = default!;
        public string Phone { get; private set; } = default!;
        public string Address { get; private set; } = default!;
        public double Lat { get; private set; } 
        public double Lng { get; private set; }
        public double RatingAvg { get; private set; }
        public bool IsVerified { get; private set; } = default!;
        public bool AcceptOnlineBookings { get; private set; } = true;
        public bool ShowPricesToCustomers { get; private set; } = true;
        public bool AutoSendUpdates { get; private set; } = true;
        private readonly List<WorkshopService> _services = [];
        public IReadOnlyCollection<WorkshopService> Services => _services;
        private readonly List<WorkshopSpecialization> _specializations = [];
        public IReadOnlyCollection<WorkshopSpecialization> Specializations => _specializations;
        public bool EmailDailySummary { get; private set; } = true;
        public string GoogleMapsLink { get; private set; } = default!;
        public TimeOnly OpeningTime { get; private set; } = new TimeOnly(8,0);
        public TimeOnly ClosingTime { get; private set; } = new TimeOnly(8, 0);


        protected Workshop() { }

        public static Workshop Create(int userId, string name, string phone, string address)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new DomainException("Workshop name cannot be empty.");
            if (string.IsNullOrWhiteSpace(phone))
                throw new DomainException("Workshop phone cannot be empty.");
            if (string.IsNullOrWhiteSpace(address))
                throw new DomainException("Workshop address cannot be empty.");
           
            return new Workshop
            {
                UserId = userId,
                Name = name,
                Phone = phone,
                Address = address,
           
                RatingAvg = 0,

            };
        }

        public void Verify()
        {
            if (IsVerified)
                throw new DomainException("Workshop is already verified.");

            IsVerified = true;
            UpdatedAt = DateTime.UtcNow;
        }
        public void Unverify()
        {
            if (!IsVerified)
                throw new DomainException("Workshop is not verified.");
            IsVerified = false;
            UpdatedAt = DateTime.UtcNow;

        }
        public Result AddService(string nameEn,string nameAr, decimal minPrice, decimal maxPrice, string descriptionEn, string descriptionAr, int duration,int serviecCategoryId)
        {
            if (_services.Any(s => s.NameEn.Equals(nameEn, StringComparison.OrdinalIgnoreCase)))
                return Result.Failure($"Service '{nameEn}' already exists in this workshop");
            if (_services.Any(s => s.NameAr.Equals(nameAr, StringComparison.OrdinalIgnoreCase)))
                return Result.Failure($"Service '{nameAr}' already exists in this workshop");
            var service = WorkshopService.Create(nameEn,nameAr, minPrice,maxPrice,duration, this.Id, serviecCategoryId, descriptionEn,descriptionAr);

            _services.Add(service);
            return Result.Success("Workshop added successfully");
        }

        public void AddSpecialization(int specializationId)
        {
            if (_specializations.Any(x =>
             x.SpecializationId == specializationId))
            {
                throw new DomainException(
                    "Specialization already added.");
            }

            var specialization =
                WorkshopSpecialization.Create(specializationId,this.Id);

            _specializations.Add(specialization);
        }
        public void RemoveSpecialization(int specializationId)
        {
            var specialization = _specializations
       .FirstOrDefault(x => x.SpecializationId == specializationId);

            if (specialization is null)
                throw new DomainException(
                    "Specialization is not associated with this workshop.");

            _specializations.Remove(specialization);

            UpdatedAt = DateTime.UtcNow;
        }
        public void UpdateSettings(bool acceptOnlineBookings, bool showPricesToCustomers, bool autoSendUpdates, bool emailDailySummary)
        {
            AcceptOnlineBookings = acceptOnlineBookings;
            ShowPricesToCustomers = showPricesToCustomers;
            AutoSendUpdates = autoSendUpdates;
            EmailDailySummary = emailDailySummary;
            UpdatedAt = DateTime.UtcNow;
        }
    
    public void UpdateDetails(string name, string phone, string googleMapsLink, string address, double lat, double lng, TimeOnly openingTime, TimeOnly closingTime)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new DomainException("Workshop name cannot be empty.");
            if (string.IsNullOrWhiteSpace(phone))
                throw new DomainException("Workshop phone cannot be empty.");
            if (string.IsNullOrWhiteSpace(address))
                throw new DomainException("Workshop address cannot be empty.");
            Name = name;
            Phone = phone;
            GoogleMapsLink = googleMapsLink;
            Address = address;
            Lat = lat;
            Lng = lng;
            OpeningTime = openingTime;
            ClosingTime = closingTime;
            UpdatedAt = DateTime.UtcNow;
        }
    }
}
    
