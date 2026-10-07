using Domain.Common;
using MediatR;

namespace Application.Features.Specialization.Commands.Update
{
    public class UpdateSpecializationCommand : IRequest<Result>
    {
        public int Id { get; set; }
        public string Name { get; set; } = default!;
        public string? Icon { get; set; }
        public int? CarBrandId { get; set; }
        public int? CarModelId { get; set; }
    }
}