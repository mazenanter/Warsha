using FluentValidation;

namespace Application.Features.Auth.Commands.ClientLogin
{
    public class ClientLoginCommandValidator : AbstractValidator<ClientLoginCommand>
    {
        public ClientLoginCommandValidator()
        {

            RuleFor(x => x.Email)
                .NotEmpty()
                .EmailAddress().WithMessage("Invalid email address");


            RuleFor(x => x.Password)
                .NotEmpty()
                .MinimumLength(6).WithMessage("Password must be at least 6 characters");
        }
    }
}
