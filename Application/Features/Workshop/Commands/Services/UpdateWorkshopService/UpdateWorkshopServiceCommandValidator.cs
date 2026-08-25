using FluentValidation;

namespace Application.Features.Workshop.Commands.Services.UpdateWorkshopService
{
    public class UpdateWorkshopServiceCommandValidator : AbstractValidator<UpdateWorkshopServiceCommand>
    {
        public UpdateWorkshopServiceCommandValidator()
        {
            RuleFor(x => x.NameEn)
               .NotEmpty()
               .WithMessage("Service name is required.")
               .MaximumLength(100)
               .WithMessage("Service name cannot exceed 100 characters.");
            RuleFor(x => x.NameAr)
            .NotEmpty()
            .WithMessage("Service name is required.")
            .MaximumLength(100)
            .WithMessage("Service name cannot exceed 100 characters.");
            RuleFor(x => x.WorkshopServiceId).NotEmpty().GreaterThan(0).WithMessage("Workshop service id must be greater than 0.");
            RuleFor(x => x.MinPrice)
                .GreaterThanOrEqualTo(0)
                .WithMessage("Minimum price cannot be negative.");
            RuleFor(x => x.MaxPrice)
                .GreaterThanOrEqualTo(0)
                .WithMessage("Maximum price cannot be negative.");

            RuleFor(x => x.MaxPrice)
                .GreaterThan(x => x.MinPrice)
                .WithMessage("Maximum price must be greater than minimum price.");

            RuleFor(x => x.ServiceCategoryId)
                .GreaterThan(0)
                .WithMessage("Please select a valid service category.");

            RuleFor(x => x.Duration)
                .GreaterThan(0)
                .WithMessage("Service duration must be greater than zero.");

            RuleFor(x => x.DescriptionEn)
                .MaximumLength(500)
                .WithMessage("Description cannot exceed 500 characters.")
                .When(x => x.DescriptionEn != null);
            RuleFor(x => x.DescriptionAr)
               .MaximumLength(500)
               .WithMessage("Description cannot exceed 500 characters.")
               .When(x => x.DescriptionAr != null);
        }
    }
}
