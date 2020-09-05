using Microsoft.EntityFrameworkCore;
using middleware.Contracts;
using middleware_autorization_authentication_auditing.Context;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace middleware.Respositories
{
    public class Repository<TEntity> : IRepository<TEntity>, IDisposable where TEntity : class
    {
        private bool isDisposed;
        protected readonly Context _context;

        public Repository(Context context)
        {
            _context = context;
        }

        public virtual IQueryable<TEntity> GetAll()
        {
            return _context.Set<TEntity>();
        }

        public virtual IQueryable<TEntity> Get(Func<TEntity, bool> predicate)
        {
            return GetAll().Where(predicate).AsQueryable();
        }

        public virtual TEntity Find(params object[] key)
        {
            return _context.Set<TEntity>().Find(key);
        }

        public virtual void Update(TEntity obj)
        {
            _context.Entry(obj).State = EntityState.Modified;
        }

        public virtual void SaveAll()
        {
            _context.SaveChanges();
        }

        public virtual void Add(TEntity obj)
        {
            _context.Set<TEntity>().Add(obj);
        }

        public virtual void Delete(Func<TEntity, bool> predicate)
        {
            _context.Set<TEntity>()
                .Where(predicate).ToList()
                .ForEach(del => _context.Set<TEntity>().Remove(del));
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (isDisposed) return;

            if (disposing)
            {
                _context.Dispose();
                GC.Collect();
            }

            isDisposed = true;
        }

        ~Repository()
        {
            Dispose(false);
        }
    }
}
