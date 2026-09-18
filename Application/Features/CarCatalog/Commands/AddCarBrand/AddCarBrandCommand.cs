using Domain.Common;
using MediatR;

namespace Application.Features.CarCatalog.Commands.AddCarBrand
{
    public record AddCarBrandCommand(string Name, string? Icon)
     : IRequest<Result>;
}
