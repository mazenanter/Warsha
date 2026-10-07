using Domain.Common;
using MediatR;

namespace Application.Features.Specialization.Commands.Add
{
    public class AddSpecializationCommand : IRequest<Result>
    {
        public string Name { get; set; } = default!;
        public string? Icon { get; set; }
        public int? CarBrandId { get; set; }
        public int? CarModelId { get; set; }
    }
}