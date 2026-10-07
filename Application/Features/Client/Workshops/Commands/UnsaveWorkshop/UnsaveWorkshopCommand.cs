using Domain.Common;
using MediatR;

namespace Application.Features.Client.Workshops.Commands.UnsaveWorkshop
{
    public class UnsaveWorkshopCommand : IRequest<Result>
    {
        public int WorkshopId { get; set; }
    }
}