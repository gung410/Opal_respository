using System;
using System.Threading.Tasks;
using Conexus.Opal.Microservice.Metadata.DataSync;
using Conexus.Opal.Microservice.Tagging.Cache;
using Microsoft.Extensions.DependencyInjection;
using Thunder.Platform.Caching;
using Thunder.Platform.EntityFrameworkCore;

namespace Conexus.Opal.Microservice.Tagging.Infrastructure
{
    public class TaggingSeeder : IDbContextSeeder
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ICacheDataProvider _cacheDataProvider;

        public TaggingSeeder(IServiceProvider serviceProvider, ICacheDataProvider cacheDataProvider)
        {
            _serviceProvider = serviceProvider;
            _cacheDataProvider = cacheDataProvider;
        }

        public void Seed<TDbContext>(TDbContext dbContext) where TDbContext : BaseThunderDbContext
        {
            _ = SyncMetaData();
        }

        private async Task SyncMetaData()
        {
            using var scope = _serviceProvider.CreateScope();
            var metadataSynchronizer = scope.ServiceProvider.GetRequiredService<IMetadataSynchronizer>();
            await metadataSynchronizer.Sync(true);

            _cacheDataProvider.RemoveCacheData(MetadataCacheKey.ForAll());
        }
    }
}
