using Domain.Common;
using MediatR;

namespace Application.Features.Workshop.Commands.UpdateProfile
{
    public class UpdateWorkshopProfileCommand : IRequest<Result>
    {
        public int WorkshopId { get; set; }
        public string Name { get; set; } = default!;
        public string Phone { get; set; }= default!;
        public string GoogleMapsLink { get; set; }= default!;
        public string Address { get; set; }= default!;
        public double Lat { get; set; }
        public double Lng { get; set; }
        public string OpeningTime { get; set; }= default!;
        public string  ClosingTime { get; set; }= default!;
    }
}
