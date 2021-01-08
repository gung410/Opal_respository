using Microsoft.EntityFrameworkCore;

namespace Thunder.Platform.EntityFrameworkCore
{
    public interface IDbContextSeeder
    {
        void Seed<TDbContext>(TDbContext dbContext) where TDbContext : BaseThunderDbContext;
    }
}
