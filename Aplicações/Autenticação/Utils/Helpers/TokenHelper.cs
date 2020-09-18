using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Middleware.Domain.Models;
using Middleware.Infrastructure.Security;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Middleware.Utils.Helpers
{
    /// <summary>
    /// Support class for creating tokens
    /// </summary>
    public static class TokenHelper
    {
        /// <summary>
        /// Class responsible for fixing user claims
        /// </summary>
        /// <param name="user">Logged User</param>
        /// <param name="userManager">Usermanager</param>
        /// <param name="roleManager">RoleManager</param>
        /// <param name="configuration">configuration - responsible for capturing information from app.json</param>
        /// <returns> String JWT token</returns>
        public static async Task<String> TokenWithRolesClaims(User user, UserManager<User> userManager, RoleManager<Role> roleManager, IConfiguration configuration)
        {
            byte[] key = Encoding.UTF8.GetBytes(configuration.GetSection("secret").Value);
            Dictionary<Role, IList<Claim>> pairsRoles = new Dictionary<Role, IList<Claim>>();
            foreach (Role role in roleManager.Roles.ToList())
            {
                bool isInRole = await userManager.IsInRoleAsync(user, role.Name);
                if (isInRole)
                {
                    IList<Claim> claims = await roleManager.GetClaimsAsync(role);
                    pairsRoles.Add(role, claims);
                }
            }

            return TokenService.GenerateToken(user, key, pairsRoles);
        }
    }
}
