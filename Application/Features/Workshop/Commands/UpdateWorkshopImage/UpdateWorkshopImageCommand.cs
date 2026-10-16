using Domain.Common;
using MediatR;
using Microsoft.AspNetCore.Http;

namespace Application.Features.Workshop.Commands.UpdateWorkshopImage
{
    public record UpdateWorkshopImageCommand(
    IFormFile Image
) : IRequest<Result<string>>;
}
