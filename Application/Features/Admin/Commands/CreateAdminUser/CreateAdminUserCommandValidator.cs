using FluentValidation;

namespace Application.Features.Admin.Commands.CreateAdminUser
{
    public class CreateAdminUserCommandValidator : AbstractValidator<CreateAdminUserCommand>
    {
        public CreateAdminUserCommandValidator()
        {
            RuleFor(x => x.Name).NotEmpty().MaximumLength(100);

            RuleFor(x => x.Email)
                .NotEmpty()
                .EmailAddress().WithMessage("Invalid email format");

            RuleFor(x => x.Password)
                .NotEmpty()
                .MinimumLength(6);

            RuleFor(x => x.PermissionIds)
                .NotNull()
                .Must(ids => ids.All(id => id > 0))
                .WithMessage("All permission IDs must be valid");
        }
    }
}
