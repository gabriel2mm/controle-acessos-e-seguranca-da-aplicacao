using Microsoft.EntityFrameworkCore;
using middleware.Models;
using System.Linq;

namespace middleware.Respositories
{
    public class RequestRepository : Repository<Request>
    {
        protected readonly Context.Context context;
        public RequestRepository(Context.Context context) : base(context)
        {
            this.context = context;
        }

        public override IQueryable<Request> GetAll()
        {
            return context.Request.Include("User").AsQueryable();
        }

        public override Request Find(params object[] key)
        {

            Request request = context.Request.Find(key);

            context.Entry(request).Reference(p => p.User).Load();

            return request;
        }
    }
}
