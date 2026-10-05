using Application.Features.Booking.DTOs;
using Application.Interfaces;
using Domain.Common;
using MediatR;

namespace Application.Features.Booking.Queries.GetWorkshopQuotes
{
    public class GetWorkshopQuotesQueryHandler
    : IRequestHandler<GetWorkshopQuotesQuery, Result<IEnumerable<WorkshopQuoteDto>>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICurrentUserService _currentUser;

        public GetWorkshopQuotesQueryHandler(IUnitOfWork unitOfWork, ICurrentUserService currentUser)
        {
            _unitOfWork = unitOfWork;
            _currentUser = currentUser;
        }

        public async Task<Result<IEnumerable<WorkshopQuoteDto>>> Handle(
            GetWorkshopQuotesQuery request, CancellationToken ct)
        {
            var workshopId = _currentUser.WorkshopId!.Value;
            var jobs = await _unitOfWork.Bookings.GetWorkshopActiveJobsAsync(workshopId, ct);

            var quotes = jobs
                .SelectMany(b => b.Quotes.Select(q => new WorkshopQuoteDto(
                    q.Id, b.Id, b.BookingNumber, b.Client.Name,
                    q.TotalAmount, q.Status.ToString(), q.CreatedAt)))
                .OrderByDescending(q => q.CreatedAt);

            return Result<IEnumerable<WorkshopQuoteDto>>.Success(quotes, "Workshop quotes retrieved successfully");
        }
    }
}
