using Application.Features.Admin.DTOs;
using Domain.Common;
using MediatR;

namespace Application.Features.Admin.Queries.GetUserPermissions
{
    public class GetUserPermissionsQuery : IRequest<Result<IEnumerable<PermissionDto>>>
    {
        public int UserId { get; set; }
    }
}
