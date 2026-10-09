using Domain.Common;
using MediatR;

namespace Application.Features.Client.Reviews.Commands.AddReview
{
    public class AddReviewCommand : IRequest<Result>
    {
        public int WorkshopId { get; set; }
        public int Rating { get; set; }
        public string? Comment { get; set; }
    }
}