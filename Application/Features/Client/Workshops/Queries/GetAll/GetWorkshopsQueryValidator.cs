using System.Data;
using FluentValidation;

namespace Application.Features.Client.Workshops.Queries.GetAll
{
    public class GetWorkshopsQueryValidator: AbstractValidator<GetWorkshopsQuery>
    {
        public GetWorkshopsQueryValidator()
        {
            RuleFor(x => x.PageNumber).GreaterThanOrEqualTo(1);

            RuleFor(x => x.PageSize).ExclusiveBetween(1,50);

            When(x => x.NearMe, () =>
            {
                RuleFor(x => x.Lat).NotNull()
                .WithMessage("lat must has value when the Nearme is true").ExclusiveBetween(-90, 90);

                RuleFor(x => x.Lng).NotNull()
                .WithMessage("Lng must has value when the Nearme is true").ExclusiveBetween(-180, 180);

                RuleFor(x => x.RadiusKm).GreaterThan(0)
                .When(x => x.RadiusKm.HasValue).WithMessage("RadiusKm should be greater than zero");
            });


        }
    }
}