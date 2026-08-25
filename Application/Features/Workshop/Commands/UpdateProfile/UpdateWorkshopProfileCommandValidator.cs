using FluentValidation;
using System.Globalization;

namespace Application.Features.Workshop.Commands.UpdateProfile
{
    public class UpdateWorkshopProfileCommandValidator : AbstractValidator<UpdateWorkshopProfileCommand>
    {
        public UpdateWorkshopProfileCommandValidator()
        {
            RuleFor(x => x.Name)
             .NotEmpty().WithMessage("Workshop name is required.")
             .MaximumLength(100);

            RuleFor(x => x.Phone)
                .NotEmpty().WithMessage("Phone is required.")
                .Matches(@"^01[0125][0-9]{8}$")
                .WithMessage("Invalid Egyptian phone number.");

            RuleFor(x => x.Address)
                .NotEmpty().WithMessage("Address is required.")
                .MaximumLength(250);

            RuleFor(x => x.Lat)
                .InclusiveBetween(-90, 90)
                .WithMessage("Latitude must be between -90 and 90.");

            RuleFor(x => x.Lng)
                .InclusiveBetween(-180, 180)
                .WithMessage("Longitude must be between -180 and 180.");

            RuleFor(x => x.OpeningTime)

           .Must(IsValidTime)
           .WithMessage("Opening time must be in HH:mm tt format.");
            RuleFor(x => x.ClosingTime)
    
           .Must(IsValidTime)
           .WithMessage("Closing time must be in HH:mm tt format.");
        }
        private bool IsValidTime(string time)
        {
            return TimeOnly.TryParseExact(
                time,
               "hh:mm tt",
        CultureInfo.InvariantCulture,
        DateTimeStyles.None,
        out var closingTime
            );
        }
    }
}
