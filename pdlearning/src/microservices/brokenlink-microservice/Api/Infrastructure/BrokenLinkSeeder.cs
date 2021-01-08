using Thunder.Platform.EntityFrameworkCore;

namespace Microservice.BrokenLink.Infrastructure
{
    public class BrokenLinkSeeder : IDbContextSeeder
    {
        public void Seed<TDbContext>(TDbContext dbContext) where TDbContext : BaseThunderDbContext
        {
        }
    }
}
