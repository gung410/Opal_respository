using Thunder.Platform.EntityFrameworkCore;

namespace Microservice.WebinarAutoscaler.Infrastructure
{
    public class WebinarAutoscalerSeeder : IDbContextSeeder
    {
        public void Seed<TDbContext>(TDbContext dbContext) where TDbContext : BaseThunderDbContext
        {
        }
    }
}
