using Application.Features.Admin.DTOs;
using Domain.Common;
using MediatR;

namespace Application.Features.Admin.Commands.CreateAdminUser
{
    public record CreateAdminUserCommand(
     string Name,
     string Email,
     string Password,
     List<int> PermissionIds
 ) : IRequest<Result<CreateAdminAndEmployeeDto>>;
}
