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
    public class CoursePlanningCycleCudLogic : BaseEntityCudLogic<CoursePlanningCycle>
    {
        public CoursePlanningCycleCudLogic(
            IWriteOnlyRepository<CoursePlanningCycle> repository,
            IThunderCqrs thunderCqrs,
            IUserContext userContext) : base(repository, thunderCqrs, userContext)
        {
        }

        public async Task Insert(CoursePlanningCycle entity, CancellationToken cancellationToken = default)
        {
            await RootRepository.InsertAsync(entity);

            await ThunderCqrs.SendEvent(
                new CoursePlanningCycleChangeEvent(entity, CoursePlanningCycleChangeType.Created),
                cancellationToken);
        }

        public async Task InsertMany(List<CoursePlanningCycle> entities, CancellationToken cancellationToken = default)
        {
            await RootRepository.InsertManyAsync(entities);

            await ThunderCqrs.SendEvents(
                entities.Select(x => new CoursePlanningCycleChangeEvent(x, CoursePlanningCycleChangeType.Created)),
                cancellationToken);
        }

        public async Task Update(CoursePlanningCycle entity, CancellationToken cancellationToken = default)
        {
            await RootRepository.UpdateAsync(entity);

            await ThunderCqrs.SendEvent(
                new CoursePlanningCycleChangeEvent(entity, CoursePlanningCycleChangeType.Updated),
                cancellationToken);
        }

        public async Task UpdateMany(List<CoursePlanningCycle> entities, CancellationToken cancellationToken = default)
        {
            await RootRepository.UpdateManyAsync(entities);

            await ThunderCqrs.SendEvents(
                entities.Select(x => new CoursePlanningCycleChangeEvent(x, CoursePlanningCycleChangeType.Updated)),
                cancellationToken);
        }

        public async Task Delete(CoursePlanningCycle entity, CancellationToken cancellationToken = default)
        {
            await RootRepository.DeleteAsync(entity);

            await ThunderCqrs.SendEvent(
                new CoursePlanningCycleChangeEvent(entity, CoursePlanningCycleChangeType.Deleted),
                cancellationToken);
        }

        public async Task DeleteMany(List<CoursePlanningCycle> entities, CancellationToken cancellationToken = default)
        {
            await RootRepository.DeleteManyAsync(entities);

            await ThunderCqrs.SendEvents(
                entities.Select(x => new CoursePlanningCycleChangeEvent(x, CoursePlanningCycleChangeType.Deleted)),
                cancellationToken);
        }
    }
}
