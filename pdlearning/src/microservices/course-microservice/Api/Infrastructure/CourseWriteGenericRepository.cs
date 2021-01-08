using System;
using Thunder.Platform.Core.Domain.Entities;
using Thunder.Platform.Core.Domain.UnitOfWork.Abstractions;
using Thunder.Platform.EntityFrameworkCore;
using Thunder.Platform.EntityFrameworkCore.Repositories;

namespace Microservice.Course.Infrastructure
{
    public class CourseWriteGenericRepository<TEntity> : BaseWriteOnlyEfCoreRepository<CourseDbContext, TEntity>
        where TEntity : class, IEntity<Guid>
    {
        public CourseWriteGenericRepository(IUnitOfWorkManager unitOfWorkManager, IDbContextProvider<CourseDbContext> dbContextProvider) : base(unitOfWorkManager, dbContextProvider)
        {
        }
    }
}
