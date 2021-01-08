using System;
using System.Data.Common;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Thunder.Platform.EntityFrameworkCore.Logging;

namespace Thunder.Platform.EntityFrameworkCore
{
    public abstract class BaseDbContextResolver : IDbContextResolver
    {
        private readonly IServiceProvider _serviceProvider;

        protected BaseDbContextResolver(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        /// <inheritdoc/>
        public virtual void InitDatabase<TDbContext>(string connectionString, bool ensureDatabaseCreated = false) where TDbContext : BaseThunderDbContext
        {
            using (var context = ResolveDbContext<TDbContext>(connectionString, null))
            {
                // TODO: this is for testing purpose only. Will be remove in the next release.
                // context.Database.EnsureDeleted();
                if (ensureDatabaseCreated)
                {
                    context.Database.EnsureCreated();
                }

                context.Database.Migrate();

                // https://docs.microsoft.com/en-us/ef/core/modeling/data-seeding.
                var seeder = _serviceProvider.GetService<IDbContextSeeder>();
                seeder.Seed(context);
                context.SaveChanges();
            }
        }

        public virtual TDbContext Resolve<TDbContext>(string connectionString) where TDbContext : BaseThunderDbContext
        {
            var dbContext = ResolveDbContext<TDbContext>(connectionString, null);

            return dbContext;
        }

        public virtual TDbContext Resolve<TDbContext>(string connectionString, DbConnection connection) where TDbContext : BaseThunderDbContext
        {
            var dbContext = ResolveDbContext<TDbContext>(connectionString, connection);

            return dbContext;
        }

        private TDbContext ResolveDbContext<TDbContext>([NotNull] string connectionString, [CanBeNull] DbConnection connection)
            where TDbContext : BaseThunderDbContext
        {
            var dbContext = _serviceProvider.GetService<TDbContext>();
            var trackingSource = _serviceProvider.GetService<IQueryTrackingSource>();

            dbContext.InitializeConnectionString(connectionString);
            dbContext.InitializeDiInstances(trackingSource);

            if (connection != null)
            {
                dbContext.InitializeDbConnection(connection);
            }

            return dbContext;
        }
    }
}
