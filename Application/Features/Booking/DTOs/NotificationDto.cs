using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Features.Booking.DTOs
{
    public record NotificationDto(
    int Id,
    string Title,
    string Body,
    string Type,
    bool IsRead,
    int? BookingId,
    DateTime CreatedAt);
}
