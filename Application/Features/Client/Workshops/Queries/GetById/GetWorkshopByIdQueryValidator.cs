using FluentValidation;

namespace Application.Features.Client.Workshops.Queries.GetById
{
    public class GetWorkshopByIdQueryValidator : AbstractValidator<GetWorkshopByIdQuery>
    {
        public GetWorkshopByIdQueryValidator(){
            RuleFor(x => x.WorkshopId).GreaterThan(0);
            RuleFor(x => x.ReviewsPageNumber).GreaterThanOrEqualTo(1);
            RuleFor(x => x.ReviewsPageSize).ExclusiveBetween(1, 50);
        }
    }
}