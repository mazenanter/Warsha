using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Features.CarCatalog.Commands.AddCarModel
{
    public class AddCarModelCommandValidator : AbstractValidator<AddCarModelCommand>
    {
        public AddCarModelCommandValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("Model name is required")
                .MaximumLength(50).WithMessage("Model name too long");

            RuleFor(x => x.BrandId)
                .GreaterThan(0).WithMessage("Invalid brand ID");
        }
    }
}
