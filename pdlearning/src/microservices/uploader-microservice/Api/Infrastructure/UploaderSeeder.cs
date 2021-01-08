using Thunder.Platform.EntityFrameworkCore;

namespace Microservice.Uploader.Infrastructure
{
    public class UploaderSeeder : IDbContextSeeder
    {
        public void Seed<TDbContext>(TDbContext dbContext) where TDbContext : BaseThunderDbContext
        {
        }
    }
}
