using System;
using Thunder.Platform.Core.Domain.Entities;
using Thunder.Platform.Core.Domain.UnitOfWork.Abstractions;
using Thunder.Platform.EntityFrameworkCore;
using Thunder.Platform.EntityFrameworkCore.Repositories;

namespace Microservice.LnaForm.Infrastructure
{
    public class LnaFormGenericRepository<TEntity> : BaseEfCoreRepository<LnaFormDbContext, TEntity>
        where TEntity : class, IEntity<Guid>
    {
        public LnaFormGenericRepository(IUnitOfWorkManager unitOfWorkManager, IDbContextProvider<LnaFormDbContext> dbContextProvider) : base(unitOfWorkManager, dbContextProvider)
        {
        }
    }
}
