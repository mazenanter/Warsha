using Domain.Common;
using Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
    public class Notification : BaseEntity
    {
        public int RecipientUserId { get; private set; }
        public string RecipientType { get; private set; } = default!;
        public NotificationType Type { get; private set; }
        public string Title { get; private set; } = default!;
        public string Body { get; private set; } = default!;
        public bool IsRead { get; private set; }
        public int? BookingId { get; private set; }
        public string? Payload { get; private set; }

        protected Notification() { }

        public static Notification Create(
            int recipientUserId,
            string recipientType,
            NotificationType type,
            string title,
            string body,
            int? bookingId = null,
            string? payload = null) =>
            new()
            {
                RecipientUserId = recipientUserId,
                RecipientType = recipientType,
                Type = type,
                Title = title,
                Body = body,
                IsRead = false,
                BookingId = bookingId,
                Payload = payload
            };

        public void MarkRead()
        {
            IsRead = true;
            UpdatedAt = DateTime.UtcNow;
        }
    }
}
