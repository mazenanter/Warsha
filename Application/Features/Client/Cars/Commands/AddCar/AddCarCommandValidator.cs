using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentValidation;

namespace Application.Features.Client.Cars.Commands.AddCar
{
    public class AddCarCommandValidator : AbstractValidator<AddCarCommand>
    {
        public AddCarCommandValidator()
        {
            RuleFor(x => x.CarModel).NotEqual(0).WithMessage("Car model is required.");


            RuleFor(x => x.Year)
                .NotEmpty().WithMessage("Year is required.")
                .InclusiveBetween(1900, DateTime.Now.Year).WithMessage("Invalid year.");
        }
    }
}
