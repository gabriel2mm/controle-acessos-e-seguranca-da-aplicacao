using Microsoft.EntityFrameworkCore;
using Middleware.Domain.Models;
using Middleware.Infrastructure.Data.Context;
using System.Linq;

namespace Middleware.Infrastructure.Respositories
{
    public class OrganizationRepository : Repository<Organization>
    {
        private new readonly Context _context;
        public OrganizationRepository(Context context) : base(context)
        {
            _context = context;
        }
        public override void Update(Organization obj)
        {
            _context.Entry(obj.Modules).State = EntityState.Modified;
        }
        public override void Add(Organization obj)
        {
            _context.Set<Organization>().Add(obj);
            _context.ChangeTracker.DetectChanges();
        }

        public override IQueryable<Organization> GetAll()
        {
            return _context.Organizations.Include("Modules").AsQueryable();
        }

        public override Organization Find(params object[] key)
        {
            Organization Organization = _context.Organizations.Find(key);
            if (Organization != null)
            {
                _context.Entry(Organization).Collection(p => p.Modules).Load();
            }
            return Organization;
        }
    }
}
