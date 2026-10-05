using Application.Features.Booking.DTOs;
using Domain.Common;
using MediatR;

namespace Application.Features.Booking.Queries.GetJobsBoard
{
    public record GetJobsBoardQuery : IRequest<Result<JobsBoardDto>>;

}
