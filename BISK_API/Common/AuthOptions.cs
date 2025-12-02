using System.Collections.Generic;

namespace gardnerAPIs.Common
{
    public sealed class AuthOptions
    {
        public List<AuthUser> Users { get; set; } = new();
    }

    public sealed class AuthUser
    {
        public string Username { get; set; }
        public string PasswordHash { get; set; }  // BCrypt
        public List<string> Roles { get; set; } = new();
    }
}
