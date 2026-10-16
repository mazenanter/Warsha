using Domain.Common;
using MediatR;

namespace Application.Features.Workshop.Commands.UpdateProfile
{
    public class UpdateWorkshopProfileCommand : IRequest<Result>
    {
        public string Name { get; set; } = default!;
        public string Phone { get; set; }= default!;
        public string GoogleMapsLink { get; set; }= default!;
        public string AddressEn { get; set; }= default!;
        public string AddressAr { get; set; }= default!;
        public string DescriptionEn { get; set; } = default!;
        public string DescriptionAr { get; set; } = default!;
        public double Lat { get; set; }
        public double Lng { get; set; }
        public string OpeningTime { get; set; }= default!;
        public string  ClosingTime { get; set; }= default!;
    }
}
