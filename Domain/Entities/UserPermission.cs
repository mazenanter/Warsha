namespace Domain.Entities
{
    public class UserPermission
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public int PermissionId { get; set; }

        protected UserPermission() { }

        public static UserPermission Create(int usrId, int permissionId) 
        {
            return new UserPermission
            {
                 PermissionId = permissionId,
                 UserId = usrId,
            };
        }
    }
}
