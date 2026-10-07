using Application.Features.Client.Workshops.DTOs;
using Domain.Common;
using MediatR;

namespace Application.Features.Client.Workshops.Queries.GetAll
{
    public class GetWorkshopsQuery: IRequest<Result<PagedResult<WorkshopResponseDto>>>
    {
        public int PageSize { get; set; } = 10;
        public int PageNumber { get; set; } = 1;
        public double Lat { get;   set; } 
        public double Lng { get;   set; }
        public string? SearchTerm  { get;   set; }
        public bool TopRated {get;   set; }
        public bool NearMe {get;   set; }
        public bool Offers {get;  set; }
        public double? RadiusKm { get; set; } = 10;
    }
}