using Domain.Common;
using MediatR;

namespace Application.Features.Client.Workshops.Commands.SaveWorkshop
{
    public class SaveWorkshopCommand : IRequest<Result>
    {
        public int WorkshopId { get; set; }
    }
}