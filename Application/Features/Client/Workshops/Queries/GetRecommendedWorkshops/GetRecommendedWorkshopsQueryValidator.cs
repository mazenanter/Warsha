using FluentValidation;

namespace Application.Features.Client.Workshops.Queries.GetRecommendedWorkshops
{
    public class GetRecommendedWorkshopQueryValidator: AbstractValidator<GetRecommendedWorkshopsQuery>
    {
        public GetRecommendedWorkshopQueryValidator()
        {
            RuleFor(w => w.PageNumber).GreaterThanOrEqualTo(1);

            RuleFor(w => w.PageSize).InclusiveBetween(1, 50);
        }
    }
}