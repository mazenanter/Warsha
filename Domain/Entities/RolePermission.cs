namespace Domain.Entities
{
    public class RolePermission
    {
        public int Id { get;private set; }
        public int RoleId { get; private set; }

        public int PermissionId { get; private set; }

        protected RolePermission() { }

        public static RolePermission Create(int roleId, int permissionId) 
        {
            return new RolePermission
            {
                PermissionId = permissionId,
                RoleId = roleId,
            };
        }
    }
}
