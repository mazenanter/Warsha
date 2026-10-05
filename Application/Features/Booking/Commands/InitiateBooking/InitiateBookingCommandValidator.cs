using FluentValidation;

namespace Application.Features.Booking.Commands.InitiateBooking
{
    public class InitiateBookingCommandValidator : AbstractValidator<InitiateBookingCommand>
    {
        public InitiateBookingCommandValidator()
        {
            RuleFor(x => x.WorkshopId).GreaterThan(0);
            RuleFor(x => x.CarId).GreaterThan(0);
            RuleFor(x => x.ServiceIds)
                .NotEmpty().WithMessage("At least one service is required");
            RuleFor(x => x.ScheduledAt)
                .GreaterThan(DateTime.UtcNow.AddHours(1))
                .WithMessage("Scheduled time must be at least 1 hour from now");
        }
    }
}
