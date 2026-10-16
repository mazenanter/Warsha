using Domain.Common;

namespace Domain.Entities
{
    public class Workshop : BaseAggregateRoot
    {
        public int UserId { get; private set; }
        public string Name { get; private set; } = default!;
        public string Email { get; private set; } = default!;
        public string Phone { get; private set; } = default!;
        public string AddressEn { get; private set; } = default!;
        public string AddressAr { get; private set; } = default!;
        public string? DescriptionAr { get; private set; } = default!;
        public string? DescriptionEn { get; private set; } = default!;
        public double Lat { get; private set; } 
        public double Lng { get; private set; }
        public double RatingAvg { get; private set; }
        public bool IsVerified { get; private set; } = default!;
        public bool AcceptOnlineBookings { get; private set; } = true;
        public bool ShowPricesToCustomers { get; private set; } = true;
        public bool AutoSendUpdates { get; private set; } = true;
        public string? ImageUrl { get; private set; }
        private readonly List<WorkshopService> _services = [];
        public IReadOnlyCollection<WorkshopService> Services => _services;
        private readonly List<WorkshopCarBrand> _supportedCarBrands = [];

        public IReadOnlyCollection<WorkshopCarBrand> SupportedCarBrands
            => _supportedCarBrands;

        private readonly List<Offer> _offers = [];

        public IReadOnlyCollection<Offer> Offers => _offers;
        public bool EmailDailySummary { get; private set; } = true;
        public TimeOnly OpeningTime { get; private set; } = new TimeOnly(8,0);
        public TimeOnly ClosingTime { get; private set; } = new TimeOnly(8, 0);

        public bool IsBusy { get; private set; }
        public DateTime? BusyFrom { get; private set; }
        public DateTime? BusyUntil { get; private set; }


        protected Workshop() { }

        public static Workshop Create(int userId,string email, string name, string phone, string addressEn,string addressAr)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new DomainException("Workshop name cannot be empty.");
            if (string.IsNullOrWhiteSpace(email))
                throw new DomainException("Workshop email cannot be empty.");
            if (string.IsNullOrWhiteSpace(phone))
                throw new DomainException("Workshop phone cannot be empty.");
            if (string.IsNullOrWhiteSpace(addressEn))
                throw new DomainException("Workshop address (English) cannot be empty.");
            if (string.IsNullOrWhiteSpace(addressAr))
                throw new DomainException("Workshop address (Arabic) cannot be empty.");
           

            return new Workshop
            {
                UserId = userId,
                Name = name,
                Phone = phone,
                AddressEn = addressEn,
                AddressAr = addressAr,
               
                Email = email,
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

        public void AddSupportedCarBrand(int carBrandId)
        {
            if (_supportedCarBrands.Any(x => x.CarBrandId == carBrandId))
                throw new DomainException(
                    "Car brand is already supported by this workshop.");

            _supportedCarBrands.Add(
                WorkshopCarBrand.Create(this.Id, carBrandId));
        }
        public void RemoveSupportedCarBrand(int carBrandId)
        {
            var item = _supportedCarBrands
                .FirstOrDefault(x => x.CarBrandId == carBrandId);

            if (item is null)
                throw new DomainException(
                    "Car brand is not supported by this workshop.");

            _supportedCarBrands.Remove(item);
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
    
    public void UpdateDetails(string name, string phone, string addressEn, string addressAr, string descriptionEn, string descriptionAr, double lat, double lng, TimeOnly openingTime, TimeOnly closingTime)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new DomainException("Workshop name cannot be empty.");
            if (string.IsNullOrWhiteSpace(phone))
                throw new DomainException("Workshop phone cannot be empty.");
            if (string.IsNullOrWhiteSpace(addressEn))
                throw new DomainException("Workshop address (English) cannot be empty.");
            if (string.IsNullOrWhiteSpace(addressAr))
                throw new DomainException("Workshop address (Arabic) cannot be empty.");
            if (string.IsNullOrWhiteSpace(descriptionEn))
                throw new DomainException("Workshop description (English) cannot be empty.");
            if (string.IsNullOrWhiteSpace(descriptionAr))
                throw new DomainException("Workshop description (Arabic) cannot be empty.");
            Name = name;
            Phone = phone;
            AddressEn = addressEn;
            AddressAr = addressAr;
            DescriptionEn = descriptionEn;
            DescriptionAr = descriptionAr;
            Lat = lat;
            Lng = lng;
            OpeningTime = openingTime;
            ClosingTime = closingTime;
    
            UpdatedAt = DateTime.UtcNow;
        }
        public void UpdateImage(string imageUrl)
        {
            if (string.IsNullOrWhiteSpace(imageUrl))
                throw new DomainException("Image URL cannot be empty.");

            ImageUrl = imageUrl;
            UpdatedAt = DateTime.UtcNow;
        }
        public Offer AddWorkshopOffer(decimal discountPercentage, DateTime startAt, DateTime endAt)
        {
            var offer = Offer.CreateWorkshopOffer(this.Id, discountPercentage, startAt, endAt);

            _offers.Add(offer);
            return offer;
        }

        public Offer AddServiceOffer(int serviceId, decimal discountPercentage, DateTime startAt, DateTime endsAt)
        {
            var service = _services.FirstOrDefault(x => x.Id == serviceId);

            if (service == null)
                throw new DomainException("Service does not belong to this workshop.");
            var offer = Offer.CreateServiceOffer(
        this.Id,
        service.Id,
        discountPercentage,
        startAt,
        endsAt);

            _offers.Add(offer);
            return offer;
        }
        public bool CanAcceptBooking()
        {
            if (!IsVerified)
                return false;

            if (IsBusy && BusyUntil.HasValue)
            {
                if (DateTime.UtcNow >= BusyUntil.Value)
                {
                    ClearBusy();
                }
                else
                {
                    return false;
                }
            }

            return true;
        }

        public bool CanAcceptOnlineBooking()
        {
            return CanAcceptBooking() && AcceptOnlineBookings;
        }

        public void SetBusy(DateTime from, DateTime until)
        {
            if (until <= DateTime.UtcNow)
                throw new DomainException("Busy until time must be in the future");
            if (until <= from)
                throw new DomainException("Busy until must be after busy from");

            IsBusy = true;
            BusyFrom = from;
            BusyUntil = until;
            UpdatedAt = DateTime.UtcNow;
        }

        public void ClearBusy()
        {
            IsBusy = false;
            BusyFrom = null;
            BusyUntil = null;
            UpdatedAt = DateTime.UtcNow;
        }
        public void UpdateRating(double ratingAvg)
        {
            RatingAvg = Math.Round(ratingAvg, 2);
            UpdatedAt = DateTime.UtcNow;
        }
    }
}
    
