using Domain.Common;

namespace Domain.Entities
{
    public class Permission
    {
        public int Id { get; private set; }

        public string Name { get; private set; } = default!;

        public string Code { get; private set; } = default!;

        public string Module { get; private set; } = default!;

        public bool IsActive { get; private set; } = true;
        private readonly List<UserPermission> _userPermissions = [];
        private readonly List<RolePermission> _rolePermissions = [];
        public IReadOnlyCollection<UserPermission> UserPermissions => _userPermissions;
        public IReadOnlyCollection<RolePermission> RolePermissions => _rolePermissions;

        protected Permission() { }
        public static Permission Create(
       string name,
       string code,
       string module)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new DomainException("Permission name is required.");

            if (string.IsNullOrWhiteSpace(code))
                throw new DomainException("Permission code is required.");

            if (string.IsNullOrWhiteSpace(module))
                throw new DomainException("Permission module is required.");

            return new Permission { Name = name,Module = module,Code =code,};
        }
    }
}
