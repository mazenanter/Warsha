using Application.Features.Admin.DTOs;
using Application.Interfaces;
using Domain.Common;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.Admin.Queries.GetUserPermissions
{
    public class GetUserPermissionsQueryHandler : IRequestHandler<GetUserPermissionsQuery, Result<IEnumerable<PermissionDto>>>
    {
        private readonly IUnitOfWork _unitOfWork;

        public GetUserPermissionsQueryHandler(IUnitOfWork unitOfWork)
            => _unitOfWork = unitOfWork;
        public async Task<Result<IEnumerable<PermissionDto>>> Handle(GetUserPermissionsQuery request, CancellationToken cancellationToken)
        {
            var permissions =  _unitOfWork.Permissions
           .GetAll()
           .Where(x=>x.UserPermissions.Any(x=>x.UserId == request.UserId)).ToList();

            var result = permissions
                .Select(p => new PermissionDto(p.Id, p.Name, p.Code, p.Module));

            return Result<IEnumerable<PermissionDto>>.Success(result,"Permissions retrieved successfully");
        }
    }
}
