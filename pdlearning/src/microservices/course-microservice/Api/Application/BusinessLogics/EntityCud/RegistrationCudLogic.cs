using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microservice.Course.Application.AggregatedEntityModels;
using Microservice.Course.Application.AssociatedEntities;
using Microservice.Course.Application.Events;
using Microservice.Course.Application.SharedQueries;
using Microservice.Course.Domain.Entities;
using Thunder.Platform.Core.Context;
using Thunder.Platform.Core.Domain.Repositories;
using Thunder.Platform.Core.Extensions;
using Thunder.Platform.Cqrs;

namespace Microservice.Course.Application.BusinessLogics.EntityCud
{
    public class RegistrationCudLogic : BaseEntityCudLogic<Registration>
    {
        private readonly GetAggregatedRegistrationSharedQuery _aggregatedRegistrationSharedQuery;

        public RegistrationCudLogic(
            IWriteOnlyRepository<Registration> repository,
            IThunderCqrs thunderCqrs,
            IUserContext userContext,
            GetAggregatedRegistrationSharedQuery aggregatedRegistrationSharedQuery) : base(repository, thunderCqrs, userContext)
        {
            _aggregatedRegistrationSharedQuery = aggregatedRegistrationSharedQuery;
        }

        public async Task InsertMany(List<Registration> entities, CancellationToken cancellationToken = default)
        {
            await RootRepository.InsertManyAsync(entities);

            var aggregatedRegistrations =
                await _aggregatedRegistrationSharedQuery.FullByRegistrations(entities, cancellationToken);
            await ThunderCqrs.SendEvents(
                aggregatedRegistrations.Select(x => new RegistrationChangeEvent(
                    new RegistrationAssociatedEntity(x.Registration, x.Course, x.ClassRun),
                    RegistrationChangeType.Created)),
                cancellationToken);
        }

        public async Task Update(RegistrationAggregatedEntityModel aggregatedEntity, CancellationToken cancellationToken = default)
        {
            await RootRepository.UpdateAsync(aggregatedEntity.Registration);

            RegistrationChangeEvent updatedEvent = new RegistrationChangeEvent(
                new RegistrationAssociatedEntity(aggregatedEntity.Registration, aggregatedEntity.Course, aggregatedEntity.ClassRun),
                RegistrationChangeType.Updated);
            await ThunderCqrs.SendEvent(updatedEvent, cancellationToken);
        }

        public async Task UpdateMany(List<Registration> entities, CancellationToken cancellationToken = default)
        {
            var aggregatedRegistrations =
                await _aggregatedRegistrationSharedQuery.FullByRegistrations(entities, cancellationToken);
            await UpdateMany(aggregatedRegistrations, cancellationToken);
        }

        public async Task UpdateMany(List<RegistrationAggregatedEntityModel> aggregatedEntities, CancellationToken cancellationToken = default)
        {
            await RootRepository.UpdateManyAsync(aggregatedEntities.SelectList(p => p.Registration));

            await ThunderCqrs.SendEvents(
                aggregatedEntities.Select(x => new RegistrationChangeEvent(
                    new RegistrationAssociatedEntity(x.Registration, x.Course, x.ClassRun),
                    RegistrationChangeType.Updated)),
                cancellationToken);
        }
    }
}
