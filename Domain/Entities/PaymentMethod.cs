using Domain.Common;
using Domain.Enums;

namespace Domain.Entities
{
    public class PaymentMethod : BaseEntity
    {
        public int ClientId { get; private set; }
        public PaymentMethodType Type { get; private set; }
        public bool IsDefault { get; private set; }

        protected PaymentMethod() { }

        public static PaymentMethod Create(int clientId, PaymentMethodType type)
        {
            if (clientId <= 0)
                throw new DomainException("Invalid client ID");

            return new PaymentMethod
            {
                ClientId = clientId,
                Type = type,
                IsDefault = false
            };
        }

        public void SetAsDefault() => IsDefault = true;
        public void RemoveDefault() => IsDefault = false;
    }
    }
