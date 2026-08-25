using Domain.Common;
using MediatR;

namespace Application.Features.Workshop.Commands.Specializations.AddSpecialization
{
    public class AddWorkshopSpecializationCommand : IRequest<Result>
    {
        public int SpecializationId { get; set; }
    }
}
