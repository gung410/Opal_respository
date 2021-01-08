using System;
using Thunder.Platform.Core.Domain.Entities;
using Thunder.Platform.Core.Domain.UnitOfWork.Abstractions;
using Thunder.Platform.EntityFrameworkCore;
using Thunder.Platform.EntityFrameworkCore.Repositories;

namespace Microservice.WebinarVideoConverter.Infrastructure
{
    public class WebinarRecordGenericRepository<TEntity> : BaseEfCoreRepository<WebinarRecordMangementDbContext, TEntity>
        where TEntity : class, IEntity<Guid>
    {
        public WebinarRecordGenericRepository(IUnitOfWorkManager unitOfWorkManager, IDbContextProvider<WebinarRecordMangementDbContext> dbContextProvider) : base(unitOfWorkManager, dbContextProvider)
        {
        }
    }
}
