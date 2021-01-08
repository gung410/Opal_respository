using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace cxOrganization.Adapter.Shared.Entity
{
    public class AsyncDbSet<T> : DbSet<T>
        where T : class
    {
        private readonly DbSet<T> _innerDbSet;
        public AsyncDbSet(DbSet<T> dbSet)
        {
            _innerDbSet = dbSet;
        }
        public override ValueTask<T> FindAsync(params object[] keyValues)
        {
            return _innerDbSet.FindAsync(keyValues);
        }
        public override T Find(params object[] keyValues)
        {
            return _innerDbSet.Find(keyValues);
        }
    }
}