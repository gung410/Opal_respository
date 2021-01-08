using System;
using Thunder.Platform.Core.Domain.Entities;
using Thunder.Platform.Core.Domain.UnitOfWork.Abstractions;
using Thunder.Platform.EntityFrameworkCore;
using Thunder.Platform.EntityFrameworkCore.Repositories;

namespace Microservice.Course.Infrastructure
{
    public class CourseReadGenericRepository<TEntity> : BaseReadOnlyEfCoreRepository<CourseDbContext, TEntity>
        where TEntity : class, IEntity<Guid>
    {
        public CourseReadGenericRepository(IUnitOfWorkManager unitOfWorkManager, IDbContextProvider<CourseDbContext> dbContextProvider) : base(unitOfWorkManager, dbContextProvider)
        {
        }
    }
}
