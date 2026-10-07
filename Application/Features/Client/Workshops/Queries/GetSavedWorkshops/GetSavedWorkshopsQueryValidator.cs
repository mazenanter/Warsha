using FluentValidation;

namespace Application.Features.Client.SavedWorkshops.Queries.GetSavedWorkshops
{
    public class GetSavedWorkshopsQueryValidator : AbstractValidator<GetSavedWorkshopsQuery>
    {
        public GetSavedWorkshopsQueryValidator()
        {
            RuleFor(x => x.PageNumber).GreaterThanOrEqualTo(1);
            RuleFor(x => x.PageSize).InclusiveBetween(1, 50);
        }
    }
}