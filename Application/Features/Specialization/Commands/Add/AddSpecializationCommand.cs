using Domain.Common;
using MediatR;

namespace Application.Features.Specialization.Commands.Add
{
    public class AddSpecializationCommand : IRequest<Result>
    {
        public string Name { get; set; }
        public string? Icon { get; set; }
    }
}
