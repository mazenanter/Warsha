using Domain.Entities;

namespace Application.Interfaces.Repositories
{
    public interface IPermissionRepository : IRepository<Permission>
    {
        Task<Permission?> GetByNameAsync(string name, CancellationToken ct = default);
        Task<IEnumerable<Permission>> GetByIdsAsync(
            IEnumerable<int> ids, CancellationToken ct = default);

        Task<IEnumerable<string>> GetUserPermissionNamesAsync(
            int userId, CancellationToken ct = default);
        Task AssignPermissionsToUserAsync(
            int userId, IEnumerable<int> permissionIds, CancellationToken ct = default);
        Task RemoveUserPermissionsAsync(int userId, CancellationToken ct = default);

        Task<IEnumerable<string>> GetRolePermissionNamesAsync(
            int roleId, CancellationToken ct = default);
        Task AssignPermissionsToRoleAsync(
            int roleId, IEnumerable<int> permissionIds, CancellationToken ct = default);
        Task RemoveRolePermissionsAsync(int roleId, CancellationToken ct = default);
    }
}
