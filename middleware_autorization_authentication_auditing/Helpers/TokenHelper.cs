using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using middleware.Security;
using middleware_autorization_authentication_auditing.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace middleware.Helpers
{
    public static class TokenHelper
    {
        public static async Task<String> TokenWithRolesClaims(User user, UserManager<User> userManager, RoleManager<IdentityRole> roleManager, IConfiguration configuration)
        {
            List<IdentityRole> roles = new List<IdentityRole>();
            byte[] key = Encoding.UTF8.GetBytes(configuration.GetSection("secret").Value);
            foreach (IdentityRole role in roleManager.Roles.ToList())
            {
                bool isInRole = await userManager.IsInRoleAsync(user, role.Name);
                if (isInRole)
                {
                    roles.Add(role);
                }
            }

            return TokenService.GenerateToken(user, key, roles);
        }
    }
}
