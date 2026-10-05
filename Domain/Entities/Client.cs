using Domain.Common;
using Domain.Events;
using static Domain.Constants.Permissions;

namespace Domain.Entities
{
    public class Client : BaseAggregateRoot
    {
        public int UserId { get; private set; } = default!;
        public string Name { get; private set; } = default!;
        public string PhoneNumber { get; private set; } = default!;
        public string Email { get; private set; } = default!;
        public string ReferralCode { get; private set; } = default!; 
        public string? ReferredByCode { get; private set; }
        private readonly List<Car> _cars = [];
        public IReadOnlyCollection<Car> Cars => _cars;

        protected Client() { }

        public static Client Create(int userId, string name,string email,string phoneNumber, string? referredByCode = null)
        {
            if (string.IsNullOrEmpty(name))
              throw new DomainException("Name is required");
            if(string.IsNullOrEmpty(email))
                throw new DomainException("Email is required");
            if (string.IsNullOrEmpty(phoneNumber))
                throw new DomainException("Phone number is required");
            var client = new Client
            {
                UserId = userId,
                Name = name,
                Email = email,
                PhoneNumber = phoneNumber,
                ReferralCode = GenerateReferralCode(),
                ReferredByCode = referredByCode
            };
            client.AddDomainEvent(new ClientRegisteredEvent(client.Id));

            return client;


        }
        private static string GenerateReferralCode()
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            return new string(Enumerable.Range(0, 8)
                .Select(_ => chars[Random.Shared.Next(chars.Length)])
                .ToArray());
        }

        public void UpdateProfile(string name, string email,string phoneNumber)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new DomainException("Name is required");

            if(string.IsNullOrWhiteSpace(email))
                throw new DomainException("Email is required");

            if (string.IsNullOrWhiteSpace(phoneNumber))
                throw new DomainException("Phone number is required");

            Name = name;
            Email = email;
            PhoneNumber = phoneNumber;
            UpdatedAt = DateTime.UtcNow;
        }

        public void AddCar( int carModelId, int year, string? licensePlate, int? currentKm)
        {
            var car = Car.Create(this.Id, carModelId, year, licensePlate, currentKm);
            _cars.Add(car);
        }

    }
}
