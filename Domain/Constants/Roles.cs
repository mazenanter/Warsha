using Domain.Enums;

namespace Domain.Constants
{
    public static class Roles
    {
        public const string SuperAdmin = nameof(UserType.SUPERADMIN);
        public const string Admin = nameof(UserType.ADMIN);
        public const string Client = nameof(UserType.CLIENT);
        public const string Workshop = nameof(UserType.WORKSHOP);
        public const string Employee = nameof(UserType.EMPLOYEE);

    }
}
