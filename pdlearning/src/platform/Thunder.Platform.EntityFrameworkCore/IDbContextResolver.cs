using System.Data.Common;
using JetBrains.Annotations;

namespace Thunder.Platform.EntityFrameworkCore
{
    public interface IDbContextResolver
    {
        /// <summary>
        /// This method will call method "Migrate" of EF Core. The option ensureDatabaseCreated is used for sqllite.
        /// </summary>
        /// <typeparam name="TDbContext">Type of DbContext.</typeparam>
        /// <param name="connectionString">Dbcontext connection string.</param>
        /// <param name="ensureDatabaseCreated">Execute to ensure database created. Default is false.</param>
        void InitDatabase<TDbContext>([NotNull] string connectionString, bool ensureDatabaseCreated = false) where TDbContext : BaseThunderDbContext;

        TDbContext Resolve<TDbContext>([NotNull] string connectionString) where TDbContext : BaseThunderDbContext;

        TDbContext Resolve<TDbContext>([NotNull] string connectionString, DbConnection connection) where TDbContext : BaseThunderDbContext;
    }
}
