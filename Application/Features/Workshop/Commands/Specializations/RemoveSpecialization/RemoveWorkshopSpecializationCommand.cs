using Domain.Common;
using MediatR;

namespace Application.Features.Workshop.Commands.Specializations.RemoveSpecialization
{
    public class RemoveWorkshopSpecializationCommand : IRequest<Result>
    {
        public int SpecializationId { get; set; }
    }
}
