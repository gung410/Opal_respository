using System;
using Thunder.Platform.Core.Domain.Entities;
using Thunder.Platform.Core.Domain.UnitOfWork.Abstractions;
using Thunder.Platform.EntityFrameworkCore;
using Thunder.Platform.EntityFrameworkCore.Repositories;

namespace Microservice.Learner.Infrastructure
{
    public class LearnerWriteGenericRepository<TEntity> : BaseWriteOnlyEfCoreRepository<LearnerDbContext, TEntity> where TEntity : class, IEntity<Guid>
    {
        public LearnerWriteGenericRepository(
            IUnitOfWorkManager unitOfWorkManager,
            IDbContextProvider<LearnerDbContext> dbContextProvider)
            : base(unitOfWorkManager, dbContextProvider)
        {
        }
    }
}
