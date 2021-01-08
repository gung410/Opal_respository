using Thunder.Platform.EntityFrameworkCore;

namespace Microservice.Calendar.Infrastructure
{
    public class CalendarSeeder : IDbContextSeeder
    {
        public void Seed<TDbContext>(TDbContext dbContext) where TDbContext : BaseThunderDbContext
        {
        }
    }
}
