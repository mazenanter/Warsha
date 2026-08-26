using Application.Features.Admin.DTOs;
using Application.Interfaces;
using Domain.Common;
using MediatR;

namespace Application.Features.Admin.Queries.GetAllPermissions
{
    public class GetAllPermissionsQueryHandler : IRequestHandler<GetAllPermissionsQuery, Result<List<PermissionModuleDto>>>
    {
        private readonly IUnitOfWork _unitOfWork;

        public GetAllPermissionsQueryHandler(IUnitOfWork unitOfWork)
            => _unitOfWork = unitOfWork;
        public async Task<Result<List<PermissionModuleDto>>> Handle(GetAllPermissionsQuery request, CancellationToken cancellationToken)
        {
            var permissions =  _unitOfWork.Permissions.GetAll();

            var grouped = permissions
                .GroupBy(p => p.Module)
                .Select(g => new PermissionModuleDto(
                    g.Key,
                    g.Select(p => new PermissionDto(p.Id, p.Name, p.Code, p.Module))))
                .ToList();

            return Result<List<PermissionModuleDto>>.Success(grouped,"Permissions retrieved successfully");
        }
    }
}
