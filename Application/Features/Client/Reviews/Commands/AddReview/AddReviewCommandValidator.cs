<<<<<<< HEAD
using FluentValidation;
=======
﻿using FluentValidation;
>>>>>>> 8849e011a858a3a8c7b288408309aeada6ba21b8

namespace Application.Features.Client.Reviews.Commands.AddReview
{
    public class AddReviewCommandValidator : AbstractValidator<AddReviewCommand>
    {
        public AddReviewCommandValidator()
        {
            RuleFor(x => x.WorkshopId).GreaterThan(0);
            RuleFor(x => x.Rating).InclusiveBetween(1, 5);
            RuleFor(x => x.Comment).MaximumLength(1000);
        }
    }
<<<<<<< HEAD
}
=======
}
>>>>>>> 8849e011a858a3a8c7b288408309aeada6ba21b8
