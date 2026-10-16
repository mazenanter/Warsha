using Domain.Common;
using MediatR;

namespace Application.Features.Client.Cars.Commands.DeleteCar
{
    public class DeleteCarCommand : IRequest<Result>
    {
        public int CarId { get; set; }

    }
}
