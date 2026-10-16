using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Enums
{
    public enum PaymentTransactionStatus
    {
        Pending = 1,
        Authorized = 2,
        Captured = 3,
        Voided = 4,
        Failed = 5,
        Refunded = 6
    }
}
