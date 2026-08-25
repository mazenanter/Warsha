using Domain.Common;
using MediatR;

namespace Application.Features.Workshop.Commands.Admin.Verify
{
    public class VerifyWorkshopCommand : IRequest<Result>
    {
        public int WorkshopId { get; set; }
    }
}
