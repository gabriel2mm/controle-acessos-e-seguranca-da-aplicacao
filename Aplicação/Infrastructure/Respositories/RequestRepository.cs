using Microsoft.EntityFrameworkCore;
using Middleware.Domain.Models;
using Middleware.Infrastructure.Data.Context;
using System.Linq;

namespace Middleware.Infrastructure.Respositories
{
    public class RequestRepository : Repository<Request>
    {
        protected readonly Context context;
        public RequestRepository(Context context) : base(context)
        {
            this.context = context;
        }

        public override IQueryable<Request> GetAll()
        {
            return context.RequestSet.Include("User").AsQueryable();
        }

        public override Request Find(params object[] key)
        {

            Request request = context.RequestSet.Find(key);

            context.Entry(request).Reference(p => p.User).Load();

            return request;
        }
    }
}
