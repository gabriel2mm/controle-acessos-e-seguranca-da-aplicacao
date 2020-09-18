using Microsoft.EntityFrameworkCore;
using Middleware.Domain.Models;
using Middleware.Infrastructure.Data.Context;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Middleware.Infrastructure.Respositories
{
    public class RoleRepository : Repository<Role>
    {
        private new readonly Context _context;
        public RoleRepository(Context context) : base(context) {
            _context = context;
        }
        public override IQueryable<Role> GetAll()
        {
            return _context.Roles.Include("Organization").AsQueryable();
        }
        public override void Add(Role obj)
        {
            _context.Set<Role>().Add(obj);
            _context.ChangeTracker.DetectChanges();
        }
        public override void Update(Role obj)
        {
            _context.Entry(obj.Organization).State = EntityState.Modified;
            _context.Entry(obj).State = EntityState.Modified;
            _context.ChangeTracker.DetectChanges();
        }
        public override Role Find(params object[] key)
        {
            Role user = _context.Roles.Find(key);
            if (user != null)
            {
                _context.Entry(user).Reference(r => r.Organization).Load();
            }
            return user;
        }
    }
}
