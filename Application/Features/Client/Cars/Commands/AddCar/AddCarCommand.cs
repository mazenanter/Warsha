using Domain.Common;
using MediatR;

namespace Application.Features.Client.Cars.Commands.AddCar
{
    public class AddCarCommand : IRequest<Result>
    {
        public int CarModel { get; set; }
        public int Year { get; set; }
        public int ActualOdometerKm { get; set; } = 0;
    }
}
