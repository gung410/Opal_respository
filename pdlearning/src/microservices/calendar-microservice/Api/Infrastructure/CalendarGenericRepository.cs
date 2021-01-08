using System;
using Thunder.Platform.Core.Domain.Entities;
using Thunder.Platform.Core.Domain.UnitOfWork.Abstractions;
using Thunder.Platform.EntityFrameworkCore;
using Thunder.Platform.EntityFrameworkCore.Repositories;

namespace Microservice.Calendar.Infrastructure
{
    public class CalendarGenericRepository<TEntity> : BaseEfCoreRepository<CalendarDbContext, TEntity>
        where TEntity : class, IEntity<Guid>
    {
        public CalendarGenericRepository(IUnitOfWorkManager unitOfWorkManager, IDbContextProvider<CalendarDbContext> dbContextProvider) : base(unitOfWorkManager, dbContextProvider)
        {
        }
    }
}
