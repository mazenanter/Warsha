using Domain.Common;
using MediatR;

namespace Application.Features.Specialization.Commands.Delete
{
    public class InActiveSpecializationCommand : IRequest<Result>
    {
        public int Id { get; set; }
    }
}
