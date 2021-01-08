using Microsoft.EntityFrameworkCore;

namespace Thunder.Platform.EntityFrameworkCore.Tests.Dummy
{
    public class SampleSeeder : IDbContextSeeder
    {
        public void Seed<TDbContext>(TDbContext dbContext) where TDbContext : BaseThunderDbContext
        {
        }
    }
}
