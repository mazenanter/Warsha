using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Features.Admin.DTOs
{
    public record PermissionModuleDto(string Module, IEnumerable<PermissionDto> Permissions);
}
