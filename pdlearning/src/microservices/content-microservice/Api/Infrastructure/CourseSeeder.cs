using Thunder.Platform.EntityFrameworkCore;

namespace Microservice.Content.Infrastructure
{
    public class CourseSeeder : IDbContextSeeder
    {
        public void Seed<TDbContext>(TDbContext dbContext) where TDbContext : BaseThunderDbContext
        {
        }
    }
}
