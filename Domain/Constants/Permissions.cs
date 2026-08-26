namespace Domain.Constants
{
    public static class Permissions
    {
        public static class Workshops
        {
            public const string View = "Permissions.Workshops.View";
            public const string Manage = "Permissions.Workshops.Manage";
            public const string Verify = "Permissions.Workshops.Verify";
        }

        public static class Bookings
        {
            public const string View = "Permissions.Bookings.View";
            public const string Manage = "Permissions.Bookings.Manage";
        }

        public static class Clients
        {
            public const string View = "Permissions.Clients.View";
            public const string Manage = "Permissions.Clients.Manage";
        }

        public static class Reviews
        {
            public const string View = "Permissions.Reviews.View";
            public const string Manage = "Permissions.Reviews.Manage";
        }

        public static class Analytics
        {
            public const string View = "Permissions.Analytics.View";
        }

        public static class Admins
        {
            public const string Manage = "Permissions.Admins.Manage";
        }

        public static IEnumerable<(string Name, string Code, string Module)> GetAll() =>
        [
            ("View Workshops",Workshops.View,  "Workshops"),
        ("Manage Workshops",Workshops.Manage,  "Workshops"),
        ("Verify Workshops",Workshops.Verify,  "Workshops"),
        (  "View Bookings",   Bookings.View,   "Bookings"),
        ( "Manage Bookings",Bookings.Manage,   "Bookings"),
        ( "View Clients", Clients.View,        "Clients"),
        ( "Manage Clients",Clients.Manage,     "Clients"),
        ( "View Reviews",  Reviews.View,       "Reviews"),
        ( "Manage Reviews",Reviews.Manage,     "Reviews"),
        ( "View Analytics",Analytics.View,     "Analytics"),
        ( "Manage Admins", Admins.Manage,      "Admins"),
    ];
    }
}
