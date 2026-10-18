using Application.Features.Admin.DTOs;
using Domain.Common;
using MediatR;

namespace Application.Features.Admin.Queries.GetClients
{
    public class GetClientsQuery : IRequest<Result<AdminClientListResponseDto>>
    {
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 10;
        public string? Search { get; set; }                 // matches name or phone
        public ClientStatusFilter Status { get; set; } = ClientStatusFilter.All;
    }
}