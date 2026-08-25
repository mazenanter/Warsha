using FluentValidation;

namespace Application.Features.Auth.Commands.VerifyAccount
{
    public class VerifyAccountCommandValidator : AbstractValidator<VerifyAccountCommand>
    {
        public VerifyAccountCommandValidator()
        {
            RuleFor(x => x.Email).NotEmpty().WithMessage("Email is required").EmailAddress().WithMessage("Invalid email format")
               .MaximumLength(100);
            RuleFor(x => x.OtpCode).MaximumLength(6).WithMessage("otp cannot be larger than 6 charchters");
        }
    }
}
