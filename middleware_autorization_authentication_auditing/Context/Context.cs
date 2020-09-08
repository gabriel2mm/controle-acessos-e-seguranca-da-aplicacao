using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using middleware_autorization_authentication_auditing.Models;

namespace middleware_autorization_authentication_auditing.Context
{
    /// <summary>
    /// Entityframework Context
    /// </summary>
    public class Context : IdentityDbContext
    {
        public Context(DbContextOptions options) : base(options) { }

        public DbSet<User> _Users { get; set; }
    }
}
