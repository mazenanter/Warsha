using Domain.Common;
using MediatR;

namespace Application.Features.CarCatalog.Commands.AddCarModel
{
    public record AddCarModelCommand(string Name, int BrandId)
      : IRequest<Result>;
}
