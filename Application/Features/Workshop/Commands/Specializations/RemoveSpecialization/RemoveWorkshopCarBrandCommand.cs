using Domain.Common;
using MediatR;

namespace Application.Features.Workshop.Commands.Specializations.RemoveSpecialization
{
    public class RemoveWorkshopCarBrandCommand : IRequest<Result>
    {
        public int CarBrandId { get; set; }
    }
}
