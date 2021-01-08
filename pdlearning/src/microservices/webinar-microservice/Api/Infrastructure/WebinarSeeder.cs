using Thunder.Platform.EntityFrameworkCore;

namespace Microservice.Webinar.Infrastructure
{
    public class WebinarSeeder : IDbContextSeeder
    {
        public void Seed<TDbContext>(TDbContext dbContext) where TDbContext : BaseThunderDbContext
        {
        }
    }
}
