using Microsoft.EntityFrameworkCore;
using Middleware.Domain.Models;
using Middleware.Infrastructure.Data.Context;
using System.Linq;

namespace Middleware.Infrastructure.Respositories
{
    public class UserRepository : Repository<User>
    {
        protected new readonly Context _context;

        public UserRepository(Context context) : base(context)
        {
            _context = context;
        }
        public override IQueryable<User> GetAll()
        {
            return _context.UserSet.Include("Organization").AsQueryable();
        }
        public override void Add(User obj)
        {
            _context.Set<User>().Add(obj);
            _context.ChangeTracker.DetectChanges();
        }
        public override void Update(User obj)
        {
            _context.Entry(obj.Organization).State = EntityState.Modified;
            _context.Entry(obj).State = EntityState.Modified;
            _context.ChangeTracker.DetectChanges();
        }
        public override User Find(params object[] key)
        {
            User user = _context.UserSet.Find(key);
            if (user != null)
            {
                _context.Entry(user).Reference(u => u.Organization).Load();
            }
            return user;
        }
    }
}
