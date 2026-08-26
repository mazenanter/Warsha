using Domain.Common;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Features.Admin.Commands.CreateAdminUser
{
    public record CreateAdminUserCommand(
     string Name,
     string Email,
     string Password,
     List<int> PermissionIds
 ) : IRequest<Result>;
}
