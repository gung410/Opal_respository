using System;
using Microservice.Webinar.Infrastructure;
using Thunder.Platform.Core.Domain.Entities;
using Thunder.Platform.Core.Domain.UnitOfWork.Abstractions;
using Thunder.Platform.EntityFrameworkCore;
using Thunder.Platform.EntityFrameworkCore.Repositories;

namespace Microservice.Webinar.Infrastructure
{
    public class WebinarGenericRepository<TEntity> : BaseEfCoreRepository<WebinarDbContext, TEntity>
        where TEntity : class, IEntity<Guid>
    {
        public WebinarGenericRepository(IUnitOfWorkManager unitOfWorkManager, IDbContextProvider<WebinarDbContext> dbContextProvider) : base(unitOfWorkManager, dbContextProvider)
        {
        }
    }
}
