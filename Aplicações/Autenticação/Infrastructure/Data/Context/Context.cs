using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Middleware.Domain.Models;
using System;

namespace Middleware.Infrastructure.Data.Context
{
    /// <summary>
    /// Entityframework Context
    /// </summary>
    public class Context : IdentityDbContext<User, Role, Guid>
    {
        public Context(DbContextOptions options) : base(options) { }
        public virtual DbSet<User> UserSet { get; set; }
        public virtual DbSet<Organization> Organizations { get; set; }
        public virtual DbSet<Module> Modules { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {

            builder.Entity<Organization>()
                .HasMany(o => o.Modules)
                .WithOne(m => m.Organization)
                .OnDelete(DeleteBehavior.Cascade);
            
            base.OnModelCreating(builder);
        }
    }
}
