using System;
using Thunder.Platform.EntityFrameworkCore;

namespace Microservice.Analytics.Infrastructure
{
    public class AnalyticsSeeder : IDbContextSeeder
    {
        public void Seed<TDbContext>(TDbContext dbContext) where TDbContext : BaseThunderDbContext
        {
        }
    }
}
