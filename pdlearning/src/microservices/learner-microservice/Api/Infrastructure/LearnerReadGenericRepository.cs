using System;
using Thunder.Platform.Core.Domain.Entities;
using Thunder.Platform.Core.Domain.UnitOfWork.Abstractions;
using Thunder.Platform.EntityFrameworkCore;
using Thunder.Platform.EntityFrameworkCore.Repositories;

namespace Microservice.Learner.Infrastructure
{
    public class LearnerReadGenericRepository<TEntity> : BaseReadOnlyEfCoreRepository<LearnerDbContext, TEntity> where TEntity : class, IEntity<Guid>
    {
        public LearnerReadGenericRepository(IUnitOfWorkManager unitOfWorkManager, IDbContextProvider<LearnerDbContext> dbContextProvider)
            : base(unitOfWorkManager, dbContextProvider)
        {
        }
    }
}
