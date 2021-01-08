using System;
using System.Data.Common;

namespace Thunder.Platform.EntityFrameworkCore
{
    public class NullDbContextResolver : IDbContextResolver
    {
        public void InitDatabase<TDbContext>(string connectionString, bool ensureDatabaseCreated = true) where TDbContext : BaseThunderDbContext
        {
            throw new NotSupportedException("Please replace the null service with your service. Example: services.Replace(ServiceDescriptor.Scoped<IDbContextResolver, YourContextResolver>());");
        }

        public TDbContext Resolve<TDbContext>(string connectionString) where TDbContext : BaseThunderDbContext
        {
            throw new NotSupportedException("Please replace the null service with your service. Example: services.Replace(ServiceDescriptor.Scoped<IDbContextResolver, YourContextResolver>());");
        }

        public TDbContext Resolve<TDbContext>(string connectionString, DbConnection connection) where TDbContext : BaseThunderDbContext
        {
            throw new NotSupportedException("Please replace the null service with your service. Example: services.Replace(ServiceDescriptor.Scoped<IDbContextResolver, YourContextResolver>());");
        }
    }
}
