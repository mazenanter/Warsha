using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Enums
{
    public enum WorkshopActionType
    {
        ManualRejection = 1,
        TimeoutRejection = 2,
        CancellationAfterAcceptance = 3
    }
}
