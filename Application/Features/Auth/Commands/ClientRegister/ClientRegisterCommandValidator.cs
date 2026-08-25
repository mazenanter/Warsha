using Application.Features.Auth.Commands.CreateClient;
using FluentValidation;

namespace Application.Features.Auth.Commands.ClientRegister
{
    public class ClientRegisterCommandValidator : AbstractValidator<ClientRegisterCommand>
    {
        public ClientRegisterCommandValidator()
        {
            RuleFor(x => x.Name)
           .NotEmpty().WithMessage("Name is required")
           .MaximumLength(100);

            RuleFor(x => x.Email)
                .NotEmpty()
                .EmailAddress().WithMessage("Invalid email address");

            RuleFor(x => x.PhoneNumber)
                .NotEmpty()
                .Matches(@"^(\+20|0)?1[0125][0-9]{8}$")
                .WithMessage("Invalid Egyptian phone number");

            RuleFor(x => x.Password)
                .NotEmpty()
                .MinimumLength(6).WithMessage("Password must be at least 6 characters");
        }
    }
}
