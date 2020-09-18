
using Microsoft.EntityFrameworkCore;
using Middleware.Domain.Models;
using Middleware.Infrastructure.Data.Context;
using System.Linq;

namespace Middleware.Infrastructure.Respositories
{
    public class ModuleRepository : Repository<Module>
    {
        private new readonly Context _context;
        public ModuleRepository(Context context) : base(context)
        {
            _context = context;
        }
        public override void Update(Module obj)
        { 
            _context.Entry(obj.Organization).State = EntityState.Modified;
            _context.Entry(obj).State = EntityState.Modified;
        }
        public override void Add(Module obj)
        {
            _context.Set<Module>().Add(obj);
            _context.ChangeTracker.DetectChanges();
        }

        public override IQueryable<Module> GetAll()
        {
            return _context.Modules.Include("Organization").AsQueryable();
        }

        public override Module Find(params object[] key)
        {
            Module Module = _context.Modules.Find(key);
            if (Module != null)
            {
                _context.Entry(Module).Reference(p => p.Organization).Load();
            }
            return Module;
        }
    }
}
