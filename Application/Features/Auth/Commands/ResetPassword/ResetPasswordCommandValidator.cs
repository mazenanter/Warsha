using FluentValidation;

namespace Application.Features.Auth.Commands.ResetPassword
{
    public class ResetPasswordCommandValidator : AbstractValidator<ResetPasswordCommand>
    {
        public ResetPasswordCommandValidator()
        {
            RuleFor(x=>x.Email).NotEmpty().WithMessage("Email is required")
                .EmailAddress().WithMessage("Invalid email address");

            RuleFor(x => x.NewPassword)
                .NotEmpty()
                .MinimumLength(6).WithMessage("Password must be at least 6 characters");

            RuleFor(x => x.OTP).MaximumLength(6).WithMessage("OTP cannot be larger than 6 characters");

        }
    }
}
