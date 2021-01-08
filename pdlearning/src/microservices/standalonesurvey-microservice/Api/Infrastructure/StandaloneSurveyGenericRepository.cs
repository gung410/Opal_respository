using System;
using Thunder.Platform.Core.Domain.Entities;
using Thunder.Platform.Core.Domain.UnitOfWork.Abstractions;
using Thunder.Platform.EntityFrameworkCore;
using Thunder.Platform.EntityFrameworkCore.Repositories;

namespace Microservice.StandaloneSurvey.Infrastructure
{
    public class StandaloneSurveyGenericRepository<TEntity> : BaseEfCoreRepository<StandaloneSurveyDbContext, TEntity>
        where TEntity : class, IEntity<Guid>
    {
        public StandaloneSurveyGenericRepository(IUnitOfWorkManager unitOfWorkManager, IDbContextProvider<StandaloneSurveyDbContext> dbContextProvider) : base(unitOfWorkManager, dbContextProvider)
        {
        }
    }
}
