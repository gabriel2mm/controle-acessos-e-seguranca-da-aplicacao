using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using middleware.Models;

namespace middleware.Context
{
    /// <summary>
    /// Entityframework Context
    /// </summary>
    public class Context : IdentityDbContext
    {
        public Context(DbContextOptions options) : base(options) { }
        public virtual DbSet<User> _Users { get; set; }
        public virtual DbSet<Order> Order { get; set; }
        public virtual DbSet<Request> Request { get; set; }
    }
}
