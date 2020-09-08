using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Middleware.Domain.Models;

namespace Middleware.Infrastructure.Data.Context
{
    /// <summary>
    /// Entityframework Context
    /// </summary>
    public class Context : IdentityDbContext
    {
        public Context(DbContextOptions options) : base(options) { }
        public virtual DbSet<User> UsersSet { get; set; }
        public virtual DbSet<Order> OrderSet { get; set; }
        public virtual DbSet<Request> RequestSet { get; set; }
    }
}
