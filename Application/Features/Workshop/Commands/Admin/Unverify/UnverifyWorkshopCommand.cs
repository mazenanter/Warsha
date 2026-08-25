using Domain.Common;
using MediatR;

namespace Application.Features.Workshop.Commands.Admin.Unverify
{
    public class UnverifyWorkshopCommand : IRequest<Result>
    {
        public int WorkshopId { get; set; }
    }
   
}
