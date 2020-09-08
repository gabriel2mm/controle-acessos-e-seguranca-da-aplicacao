using System;

namespace middleware.Helpers
{
    /// <summary>
    /// permission for modules
    /// </summary>
    public static class Permission
    {
        public static String ADMIN = "Admin";
        public static class Users
        {
            public const String Viwer = "Permission.Users.Viwer";
            public const String Manager = "Permission.Users.Manager";
        }

        public static class Roles
        {
            public const String Viwer = "Permission.Roles.Viwer";
            public const String Manager = "Permission.Roles.Manager";
        }

        public static class Accounts
        {
            public const String Viwer = "Permission.Accounts.Viwer";
            public const String Manager = "Permission.Accounts.Manager";
        }
    }
}
