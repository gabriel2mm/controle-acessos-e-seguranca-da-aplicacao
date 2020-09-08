using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using middleware_autorization_authentication_auditing.Models;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace middleware.Security
{
    /// <summary>
    /// Class responsible for managing jwt tokens
    /// </summary>
    public static class TokenService
    {
        /// <summary>
        /// Generate JWT token
        /// </summary>
        /// <param name="user">Logged User</param>
        /// <param name="key">Secret key application</param>
        /// <param name="pairRoles">Dictionary of roles and claims</param>
        /// <returns>String Token JWT</returns>
        public static String GenerateToken(User user, byte[] key, Dictionary<IdentityRole, IList<Claim>> pairRoles)
        {
            JwtSecurityTokenHandler tokenHandler = new JwtSecurityTokenHandler();
            SecurityTokenDescriptor tokenDescriptor = new SecurityTokenDescriptor()
            {
                Subject = new ClaimsIdentity(Claims(user, pairRoles)),
                IssuedAt = DateTime.UtcNow,
                Expires = DateTime.UtcNow.AddHours(1),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };
            SecurityToken token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }
        /// <summary>
        /// Method responsible for organizing user statements
        /// </summary>
        /// <param name="user">Logged User</param>
        /// <param name="pairRoles">Dictionary of roles and claims</param>
        /// <returns>Array of claim</returns>
        public static Claim[] Claims(User user, Dictionary<IdentityRole, IList<Claim>> pairRoles)
        {
            List<Claim> claims = new List<Claim>();
            claims.Add(new Claim(ClaimTypes.NameIdentifier, user.Id));
            claims.Add(new Claim(ClaimTypes.Name, user.UserName));
            claims.Add(new Claim(ClaimTypes.Email, user.Email));
            claims.Add(new Claim("Login", user.Login));
            claims.Add(new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()));

            foreach (KeyValuePair<IdentityRole, IList<Claim>> entry in pairRoles)
            {
                claims.Add(new Claim(ClaimTypes.Role, entry.Key.Name));
                foreach (Claim c in entry.Value)
                {
                    claims.Add(c);
                }
            }

            return claims.ToArray();
        }
    }
}
