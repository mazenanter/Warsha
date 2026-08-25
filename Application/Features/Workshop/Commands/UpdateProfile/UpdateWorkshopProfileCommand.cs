using Domain.Common;
using MediatR;

namespace Application.Features.Workshop.Commands.UpdateProfile
{
    public class UpdateWorkshopProfileCommand : IRequest<Result>
    {
        public int WorkshopId { get; set; }
        public string Name { get; set; }
        public string Phone { get; set; }
        public string GoogleMapsLink { get; set; }
        public string Address { get; set; }
        public double Lat { get; set; }
        public double Lng { get; set; }
        public string OpeningTime { get; set; }
        public string  ClosingTime { get; set; }
    }
}
