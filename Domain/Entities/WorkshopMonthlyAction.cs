using Domain.Common;
using Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
    public class WorkshopMonthlyAction : BaseEntity
    {
        public int WorkshopId { get; private set; }
        public int BookingId { get; private set; }

        public int Year { get; private set; }
        public int Month { get; private set; }

        public WorkshopActionType ActionType { get; private set; }
        public decimal PenaltyAmount { get; private set; }

        protected WorkshopMonthlyAction() { }

        public static WorkshopMonthlyAction Create(
            int workshopId,
            int bookingId,
            int year,
            int month,
            WorkshopActionType actionType,
            decimal penaltyAmount)
        {
            if (workshopId <= 0)
                throw new DomainException("Invalid workshop ID.");

            if (bookingId <= 0)
                throw new DomainException("Invalid booking ID.");

            if (year < 2000 || month is < 1 or > 12)
                throw new DomainException("Invalid action period.");

            if (penaltyAmount < 0)
                throw new DomainException("Penalty cannot be negative.");

            return new WorkshopMonthlyAction
            {
                WorkshopId = workshopId,
                BookingId = bookingId,
                Year = year,
                Month = month,
                ActionType = actionType,
                PenaltyAmount = penaltyAmount
            };
        }
    }
}