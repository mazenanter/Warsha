using Domain.Common;
using MediatR;

namespace Application.Features.Admin.Commands.AssignUserPermissions
{
    public record AssignUserPermissionsCommand(
     int TargetUserId,
     List<int> PermissionIds
 ) : IRequest<Result>;
}
