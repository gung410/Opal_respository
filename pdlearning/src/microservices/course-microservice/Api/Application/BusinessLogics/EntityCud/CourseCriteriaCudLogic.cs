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
    public class CourseCriteriaCudLogic : BaseEntityCudLogic<CourseCriteria>
    {
        public CourseCriteriaCudLogic(
            IWriteOnlyRepository<CourseCriteria> repository,
            IThunderCqrs thunderCqrs,
            IUserContext userContext) : base(repository, thunderCqrs, userContext)
        {
        }

        public async Task Insert(CourseCriteria entity, CancellationToken cancellationToken = default)
        {
            await RootRepository.InsertAsync(entity);

            await ThunderCqrs.SendEvent(
                new CourseCriteriaChangeEvent(entity, CourseCriteriaChangeType.Created),
                cancellationToken);
        }

        public async Task InsertMany(List<CourseCriteria> entities, CancellationToken cancellationToken = default)
        {
            await RootRepository.InsertManyAsync(entities);

            await ThunderCqrs.SendEvents(
                entities.Select(x => new CourseCriteriaChangeEvent(x, CourseCriteriaChangeType.Created)),
                cancellationToken);
        }

        public async Task Update(CourseCriteria entity, CancellationToken cancellationToken = default)
        {
            await RootRepository.UpdateAsync(entity);

            await ThunderCqrs.SendEvent(
                new CourseCriteriaChangeEvent(entity, CourseCriteriaChangeType.Updated),
                cancellationToken);
        }

        public async Task UpdateMany(List<CourseCriteria> entities, CancellationToken cancellationToken = default)
        {
            await RootRepository.UpdateManyAsync(entities);

            await ThunderCqrs.SendEvents(
                entities.Select(x => new CourseCriteriaChangeEvent(x, CourseCriteriaChangeType.Updated)),
                cancellationToken);
        }

        public async Task Delete(CourseCriteria entity, CancellationToken cancellationToken = default)
        {
            await RootRepository.DeleteAsync(entity);

            await ThunderCqrs.SendEvent(
                new CourseCriteriaChangeEvent(entity, CourseCriteriaChangeType.Deleted),
                cancellationToken);
        }

        public async Task DeleteMany(List<CourseCriteria> entities, CancellationToken cancellationToken = default)
        {
            await RootRepository.DeleteManyAsync(entities);

            await ThunderCqrs.SendEvents(
                entities.Select(x => new CourseCriteriaChangeEvent(x, CourseCriteriaChangeType.Deleted)),
                cancellationToken);
        }
    }
}
