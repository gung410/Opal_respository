using System;
using Thunder.Platform.Core.Domain.Entities;
using Thunder.Platform.Core.Domain.UnitOfWork.Abstractions;
using Thunder.Platform.EntityFrameworkCore;
using Thunder.Platform.EntityFrameworkCore.Repositories;

namespace Microservice.Uploader.Infrastructure
{
    public class UploaderGenericRepository<TEntity> : BaseEfCoreRepository<UploaderDbContext, TEntity>
        where TEntity : class, IEntity<Guid>
    {
        public UploaderGenericRepository(IUnitOfWorkManager unitOfWorkManager, IDbContextProvider<UploaderDbContext> dbContextProvider) : base(unitOfWorkManager, dbContextProvider)
        {
        }
    }
}
