using Domain.Common;
using MediatR;

namespace Application.Features.Client.Cars.Commands.UpdateCar
{
    public class UpdateCarCommand : IRequest<Result>
    {
        public int CarId { get; set; }
        public int CarModel { get; set; }
        public int Year { get; set; }
    }
}
