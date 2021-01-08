using System;
using Thunder.Platform.Core.Domain.Entities;
using Thunder.Platform.Core.Domain.UnitOfWork.Abstractions;
using Thunder.Platform.EntityFrameworkCore;
using Thunder.Platform.EntityFrameworkCore.Repositories;

namespace Microservice.BrokenLink.Infrastructure
{
    public class BrokenLinkGenericRepository<TEntity> : BaseEfCoreRepository<BrokenLinkDbContext, TEntity>
        where TEntity : class, IEntity<Guid>
    {
        public BrokenLinkGenericRepository(IUnitOfWorkManager unitOfWorkManager, IDbContextProvider<BrokenLinkDbContext> dbContextProvider) : base(unitOfWorkManager, dbContextProvider)
        {
        }
    }
}
