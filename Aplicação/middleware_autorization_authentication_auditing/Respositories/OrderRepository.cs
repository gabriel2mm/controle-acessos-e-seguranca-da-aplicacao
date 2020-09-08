using Microsoft.EntityFrameworkCore;
using middleware.Models;
using middleware.Context;
using System.Linq;

namespace middleware.Respositories
{
    public class OrderRepository : Repository<Order>
    {
        private readonly Context.Context context;
        public OrderRepository(Context.Context context) : base(context)
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
            return context.Order.Include("User").AsQueryable();
        }

        public override Order Find(params object[] key)
        {
            Order order = context.Order.Find(key);
            if (order != null)
            {
                context.Entry(order).Reference(p => p.User).Load();
                context.Entry(order).Reference(p => p.Request).Load();
            }

            return order;
        }
    }
}
