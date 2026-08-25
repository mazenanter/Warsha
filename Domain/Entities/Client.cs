using Domain.Common;

namespace Domain.Entities
{
    public class Client : BaseAggregateRoot
    {
        public int UserId { get; private set; } = default!;
        public string Name { get; private set; } = default!;
        public string PhoneNumber { get; private set; } = default!;
        public string Email { get; private set; } = default!;

        protected Client() { }

        public static Client Create(int userId, string name,string email,string phoneNumber)
        {
            if (string.IsNullOrEmpty(name))
              throw new DomainException("Name is required");
            if(string.IsNullOrEmpty(email))
                throw new DomainException("Email is required");
            if (string.IsNullOrEmpty(phoneNumber))
                throw new DomainException("Phone number is required");

            return new Client
            {
                UserId = userId,
                Name = name,
                Email = email,
                PhoneNumber = phoneNumber
            };


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

    }
}
