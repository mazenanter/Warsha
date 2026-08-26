using Application.Features.Admin.DTOs;
using Domain.Common;
using MediatR;

namespace Application.Features.Admin.Queries.GetAllPermissions
{
    public class GetAllPermissionsQuery : IRequest<Result<List<PermissionModuleDto>>>
    {
    }
}
