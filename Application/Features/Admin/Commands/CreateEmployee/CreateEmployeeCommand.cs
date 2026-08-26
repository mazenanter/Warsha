using Domain.Common;
using MediatR;

namespace Application.Features.Admin.Commands.CreateEmployee
{
    public record CreateEmployeeCommand(
     string Name,
     string Email,
     string Password,
     List<int> PermissionIds
 ) : IRequest<Result>;
}
