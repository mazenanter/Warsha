<<<<<<< HEAD
using FluentValidation;
=======
﻿using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
>>>>>>> 8849e011a858a3a8c7b288408309aeada6ba21b8

namespace Application.Features.Client.Profile.Commands.UpdateProfile
{
    public class UpdateProfileCommandValidator : AbstractValidator<UpdateProfileCommand>
    {
        public UpdateProfileCommandValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("Name is required")
                .MaximumLength(100);

            RuleFor(x => x.PhoneNumber)
                .NotEmpty().WithMessage("Phone number is required")
                .MaximumLength(20);
            // If ClientRegisterCommandValidator has a phone-format rule, copy it here
            // so registration and profile updates accept the same numbers.
        }
    }
<<<<<<< HEAD
}
=======
}
>>>>>>> 8849e011a858a3a8c7b288408309aeada6ba21b8
