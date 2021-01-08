using Thunder.Platform.EntityFrameworkCore;

namespace Microservice.StandaloneSurvey.Infrastructure
{
    public class StandaloneSurveySeeder : IDbContextSeeder
    {
        public void Seed<TDbContext>(TDbContext dbContext) where TDbContext : BaseThunderDbContext
        {
        }
    }
}
