using Domain.Common;
using MediatR;

namespace Application.Features.Client.Cars.Commands.CorrectOdometer
{
    public class CorrectOdometerCommand: IRequest<Result>
    {
        public int CarId { get; set; }
        public int OdometerKm { get; set; }
        public string? Note { get; set; }
      
    }

}
