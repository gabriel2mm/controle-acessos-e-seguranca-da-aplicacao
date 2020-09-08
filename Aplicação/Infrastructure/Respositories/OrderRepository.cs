using Microsoft.EntityFrameworkCore;
using Middleware.Domain.Models;
using Middleware.Infrastructure.Data.Context;
using System.Linq;

namespace Middleware.Infrastructure.Respositories
{
    public class OrderRepository : Repository<Order>
    {
        private readonly Context context;
        public OrderRepository(Context context) : base(context)
        {
            this.context = context;
        }

        public override void Update(Order obj)
        {
            context.Entry(obj.User).State = EntityState.Modified;
            context.Entry(obj.Request).State = EntityState.Modified;
            context.Entry(obj).State = EntityState.Modified;
        }
        public override void Add(Order obj)
        {
            context.Set<User>().Attach(obj.User).State = EntityState.Unchanged;
            context.Set<Order>().Add(obj);
            context.ChangeTracker.DetectChanges();
        }

        public override IQueryable<Order> GetAll()
        {
            return context.OrderSet.Include("User").AsQueryable();
        }

        public override Order Find(params object[] key)
        {
            Order order = context.OrderSet.Find(key);
            if (order != null)
            {
                context.Entry(order).Reference(p => p.User).Load();
                context.Entry(order).Reference(p => p.Request).Load();
            }

            return order;
        }
    }
}
