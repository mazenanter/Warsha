using Application.Interfaces.Repositories;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Persistence.Repositories
{
    public class PermissionRepository : Repository<Permission>, IPermissionRepository
    {
        public PermissionRepository(AppDbContext context) : base(context) { }
        public async Task<Permission?> GetByNameAsync(string name, CancellationToken ct = default)
      => await _context.Permissions
          .FirstOrDefaultAsync(p => p.Name == name, ct);

        public async Task<IEnumerable<Permission>> GetByIdsAsync(
            IEnumerable<int> ids, CancellationToken ct = default)
            => await _context.Permissions
                .Where(p => ids.Contains(p.Id))
                .ToListAsync(ct);


        public async Task<IEnumerable<string>> GetUserPermissionNamesAsync(
            int userId, CancellationToken ct = default)
            => await _context.UserPermissions
                .Where(up => up.UserId == userId)
                .Select(up => up.Permission.Name)
                .ToListAsync(ct);

        public async Task AssignPermissionsToUserAsync(
            int userId, IEnumerable<int> permissionIds, CancellationToken ct = default)
        {
            var permissions = permissionIds
                .Select(pid => UserPermission.Create(userId, pid))
                .ToList();

            await _context.UserPermissions.AddRangeAsync(permissions, ct);
        }

        public async Task RemoveUserPermissionsAsync(int userId, CancellationToken ct = default)
        {
            var existing = await _context.UserPermissions
                .Where(up => up.UserId == userId)
                .ToListAsync(ct);

            _context.UserPermissions.RemoveRange(existing);
        }


        public async Task<IEnumerable<string>> GetRolePermissionNamesAsync(
            int roleId, CancellationToken ct = default)
            => await _context.RolePermissions
                .Where(rp => rp.RoleId == roleId)
                .Select(rp => rp.Permission.Name)
                .ToListAsync(ct);

        public async Task AssignPermissionsToRoleAsync(
            int roleId, IEnumerable<int> permissionIds, CancellationToken ct = default)
        {
            var permissions = permissionIds
                .Select(pid => RolePermission.Create(roleId, pid))
                .ToList();

            await _context.RolePermissions.AddRangeAsync(permissions, ct);
        }

        public async Task RemoveRolePermissionsAsync(int roleId, CancellationToken ct = default)
        {
            var existing = await _context.RolePermissions
                .Where(rp => rp.RoleId == roleId)
                .ToListAsync(ct);

            _context.RolePermissions.RemoveRange(existing);
        }
    }
}
