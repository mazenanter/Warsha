using Domain.Common;
using MediatR;

namespace Application.Features.Workshop.Commands.Specializations.AddSpecialization
{
    public class AddWorkshopCarBrandCommand : IRequest<Result>
    {
        public int CarBrandId { get; set; }
    }
}
