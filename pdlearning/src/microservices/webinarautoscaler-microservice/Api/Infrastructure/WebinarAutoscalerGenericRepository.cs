using System;
using Thunder.Platform.Core.Domain.Entities;
using Thunder.Platform.Core.Domain.UnitOfWork.Abstractions;
using Thunder.Platform.EntityFrameworkCore;
using Thunder.Platform.EntityFrameworkCore.Repositories;

namespace Microservice.WebinarAutoscaler.Infrastructure
{
    public class WebinarAutoscalerGenericRepository<TEntity> : BaseEfCoreRepository<WebinarAutoscalerDbContext, TEntity>
        where TEntity : class, IEntity<Guid>
    {
        public WebinarAutoscalerGenericRepository(IUnitOfWorkManager unitOfWorkManager, IDbContextProvider<WebinarAutoscalerDbContext> dbContextProvider) : base(unitOfWorkManager, dbContextProvider)
        {
        }
    }
}
