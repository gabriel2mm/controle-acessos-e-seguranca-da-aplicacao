using System;
using System.Linq;

namespace Middleware.Domain.Interfaces
{
    /// <summary>
    /// Contract for all repositories
    /// </summary>
    /// <typeparam name="TEntity">Database model class</typeparam>
    public interface IRepository<TEntity> where TEntity : class
    {
        IQueryable<TEntity> GetAll();
        IQueryable<TEntity> Get(Func<TEntity, bool> predicate);
        TEntity Find(params object[] key);
        void Update(TEntity obj);
        void SaveAll();
        void Add(TEntity obj);
        void Delete(Func<TEntity, bool> predicate);
    }
}
