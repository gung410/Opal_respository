using System;
using Thunder.Platform.Core.Domain.Entities;
using Thunder.Platform.Core.Domain.UnitOfWork.Abstractions;
using Thunder.Platform.EntityFrameworkCore;
using Thunder.Platform.EntityFrameworkCore.Repositories;

namespace Microservice.Form.Infrastructure
{
    public class FormGenericRepository<TEntity> : BaseEfCoreRepository<FormDbContext, TEntity>
        where TEntity : class, IEntity<Guid>
    {
        public FormGenericRepository(IUnitOfWorkManager unitOfWorkManager, IDbContextProvider<FormDbContext> dbContextProvider) : base(unitOfWorkManager, dbContextProvider)
        {
        }
    }
}
