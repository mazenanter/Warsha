using FluentValidation;

namespace Application.Features.Workshop.Queries.GetDashboardRevenue
{
    public class GetDashboardRevenueQueryValidator : AbstractValidator<GetDashboardRevenueQuery>
    {
        private static readonly DateOnly Min = new(2020, 1, 1);
        private static readonly DateOnly Max = new(2100, 1, 1);

        public GetDashboardRevenueQueryValidator()
        {
            RuleFor(x => x.WeekStart)
                .Must(d => d!.Value >= Min && d.Value <= Max)
                .When(x => x.WeekStart.HasValue)
                .WithMessage("weekStart is out of the supported range");
        }
    }
}