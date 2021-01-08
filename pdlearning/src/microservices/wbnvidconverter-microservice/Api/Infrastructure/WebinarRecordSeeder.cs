using Thunder.Platform.EntityFrameworkCore;

namespace Microservice.WebinarVideoConverter.Infrastructure
{
    public class WebinarRecordSeeder : IDbContextSeeder
    {
        public void Seed<TDbContext>(TDbContext dbContext) where TDbContext : BaseThunderDbContext
        {
        }
    }
}
