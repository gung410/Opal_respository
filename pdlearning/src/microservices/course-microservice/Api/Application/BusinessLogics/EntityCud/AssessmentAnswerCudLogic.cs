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
    public class AssessmentAnswerCudLogic : BaseEntityCudLogic<AssessmentAnswer>
    {
        public AssessmentAnswerCudLogic(
           IWriteOnlyRepository<AssessmentAnswer> repository,
           IThunderCqrs thunderCqrs,
           IUserContext userContext) : base(repository, thunderCqrs, userContext)
        {
        }

        public async Task Insert(AssessmentAnswer entity, CancellationToken cancellationToken = default)
        {
            await RootRepository.InsertAsync(entity);

            await ThunderCqrs.SendEvent(
                new AssessmentAnswerChangeEvent(entity, AssessmentAnswerChangeType.Created),
                cancellationToken);
        }

        public async Task InsertMany(List<AssessmentAnswer> entities, CancellationToken cancellationToken = default)
        {
            await RootRepository.InsertManyAsync(entities);

            await ThunderCqrs.SendEvents(
                entities.Select(x => new AssessmentAnswerChangeEvent(x, AssessmentAnswerChangeType.Created)),
                cancellationToken);
        }

        public async Task Update(AssessmentAnswer entity, CancellationToken cancellationToken = default)
        {
            await RootRepository.UpdateAsync(entity);

            await ThunderCqrs.SendEvent(
                new AssessmentAnswerChangeEvent(entity, AssessmentAnswerChangeType.Updated),
                cancellationToken);
        }

        public async Task UpdateMany(List<AssessmentAnswer> entities, CancellationToken cancellationToken = default)
        {
            await RootRepository.UpdateManyAsync(entities);

            await ThunderCqrs.SendEvents(
                entities.Select(x => new AssessmentAnswerChangeEvent(x, AssessmentAnswerChangeType.Updated)),
                cancellationToken);
        }

        public async Task Delete(AssessmentAnswer entity, CancellationToken cancellationToken = default)
        {
            await RootRepository.DeleteAsync(entity);

            await ThunderCqrs.SendEvent(
                new AssessmentAnswerChangeEvent(entity, AssessmentAnswerChangeType.Deleted),
                cancellationToken);
        }

        public async Task DeleteMany(List<AssessmentAnswer> entities, CancellationToken cancellationToken = default)
        {
            await RootRepository.DeleteManyAsync(entities);

            await ThunderCqrs.SendEvents(
                entities.Select(x => new AssessmentAnswerChangeEvent(x, AssessmentAnswerChangeType.Deleted)),
                cancellationToken);
        }
    }
}
