using System;
using Thunder.Platform.Core.Domain.Entities;
using Thunder.Platform.Core.Domain.UnitOfWork.Abstractions;
using Thunder.Platform.EntityFrameworkCore;
using Thunder.Platform.EntityFrameworkCore.Repositories;

namespace Microservice.Content.Infrastructure
{
    public class ContentGenericRepository<TEntity> : BaseEfCoreRepository<ContentDbContext, TEntity>
        where TEntity : class, IEntity<Guid>
    {
        public ContentGenericRepository(IUnitOfWorkManager unitOfWorkManager, IDbContextProvider<ContentDbContext> dbContextProvider) : base(unitOfWorkManager, dbContextProvider)
        {
        }
    }
}
