using Domain.Common;
using MediatR;

namespace Application.Features.Specialization.Commands.Update
{
    public class UpdateSpecializationCommand : IRequest<Result>
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string? Icon { get; set; }
    }
}
