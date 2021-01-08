using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microservice.Course.Application.Events;
using Microservice.Course.Domain.Entities;
using Thunder.Platform.Core.Context;
using Thunder.Platform.Core.Domain.Repositories;
using Thunder.Platform.Cqrs;

namespace Microservice.Course.Application.BusinessLogics.EntityCud
{
    public class CourseCudLogic : BaseEntityCudLogic<CourseEntity>
    {
        public CourseCudLogic(
            IWriteOnlyRepository<CourseEntity> repository,
            IThunderCqrs thunderCqrs,
            IUserContext userContext) : base(repository, thunderCqrs, userContext)
        {
        }

        public async Task Insert(CourseEntity entity, CancellationToken cancellationToken = default)
        {
            await RootRepository.InsertAsync(entity);

            await ThunderCqrs.SendEvent(
                new CourseChangeEvent(entity, CourseChangeType.Created),
                cancellationToken);
        }

        public async Task Update(CourseEntity entity, CancellationToken cancellationToken = default)
        {
            await RootRepository.UpdateAsync(entity);

            await ThunderCqrs.SendEvent(
                new CourseChangeEvent(entity, CourseChangeType.Updated),
                cancellationToken);
        }

        public async Task UpdateMany(List<CourseEntity> entities, CancellationToken cancellationToken = default)
        {
            await RootRepository.UpdateManyAsync(entities);

            await ThunderCqrs.SendEvents(
                entities.Select(x => new CourseChangeEvent(x, CourseChangeType.Updated)),
                cancellationToken);
        }

        public async Task Delete(CourseEntity entity, CancellationToken cancellationToken = default)
        {
            await RootRepository.DeleteAsync(entity);

            await ThunderCqrs.SendEvent(
                new CourseChangeEvent(entity, CourseChangeType.Deleted),
                cancellationToken);
        }
    }
}
