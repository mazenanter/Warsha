using Domain.Common;
using MediatR;

namespace Application.Features.Specialization.Commands.Active
{
    public class ActiveSpecializationCommand : IRequest<Result>
    {
        public int Id { get; set; }
    }
}
