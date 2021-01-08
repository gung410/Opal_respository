using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microservice.Course.Application.AggregatedEntityModels;
using Microservice.Course.Application.Events;
using Microservice.Course.Domain.Entities;
using Thunder.Platform.Core.Context;
using Thunder.Platform.Core.Domain.Repositories;
using Thunder.Platform.Cqrs;

namespace Microservice.Course.Application.BusinessLogics.EntityCud
{
    public class ClassRunCudLogic : BaseEntityCudLogic<ClassRun>
    {
        public ClassRunCudLogic(
            IWriteOnlyRepository<ClassRun> repository,
            IThunderCqrs thunderCqrs,
            IUserContext userContext) : base(repository, thunderCqrs, userContext)
        {
        }

        public async Task Insert(ClassRunAggregatedEntityModel aggregatedEntity, CancellationToken cancellationToken = default)
        {
            await RootRepository.InsertAsync(aggregatedEntity.ClassRun);

            await ThunderCqrs.SendEvent(
                new ClassRunChangeEvent(aggregatedEntity.ToAssociatedEntity(), ClassRunChangeType.Created),
                cancellationToken);
        }

        public async Task Update(ClassRunAggregatedEntityModel aggregatedEntity, CancellationToken cancellationToken = default)
        {
            await RootRepository.UpdateAsync(aggregatedEntity.ClassRun);

            await ThunderCqrs.SendEvent(
                new ClassRunChangeEvent(aggregatedEntity.ToAssociatedEntity(), ClassRunChangeType.Updated),
                cancellationToken);
        }

        public async Task UpdateMany(List<ClassRunAggregatedEntityModel> aggregatedEntities, CancellationToken cancellationToken = default)
        {
            await RootRepository.UpdateManyAsync(aggregatedEntities.Select(p => p.ClassRun));

            await ThunderCqrs.SendEvents(
                aggregatedEntities.Select(x => new ClassRunChangeEvent(x.ToAssociatedEntity(), ClassRunChangeType.Updated)),
                cancellationToken);
        }
    }
}
