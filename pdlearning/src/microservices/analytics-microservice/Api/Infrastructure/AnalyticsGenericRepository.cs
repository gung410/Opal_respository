using System;
using Thunder.Platform.Core.Domain.Entities;
using Thunder.Platform.Core.Domain.UnitOfWork.Abstractions;
using Thunder.Platform.EntityFrameworkCore;
using Thunder.Platform.EntityFrameworkCore.Repositories;

namespace Microservice.Analytics.Infrastructure
{
#pragma warning disable SA1402 // File may only contain a single type
    public class AnalyticsGenericRepository<TEntity> : BaseEfCoreRepository<AnalyticsDbContext, TEntity>
        where TEntity : class, IEntity<Guid>
    {
        public AnalyticsGenericRepository(IUnitOfWorkManager unitOfWorkManager, IDbContextProvider<AnalyticsDbContext> dbContextProvider) : base(unitOfWorkManager, dbContextProvider)
        {
        }
    }

    public class AnalyticsGenericRepositoryWithTypePrimaryKey<TEntity, TPrimaryKey> : BaseEfCoreRepository<AnalyticsDbContext, TEntity, TPrimaryKey>
        where TEntity : class, IEntity<TPrimaryKey>
    {
        public AnalyticsGenericRepositoryWithTypePrimaryKey(IUnitOfWorkManager unitOfWorkManager, IDbContextProvider<AnalyticsDbContext> dbContextProvider) : base(unitOfWorkManager, dbContextProvider)
        {
        }
    }
#pragma warning restore SA1402 // File may only contain a single type
}
