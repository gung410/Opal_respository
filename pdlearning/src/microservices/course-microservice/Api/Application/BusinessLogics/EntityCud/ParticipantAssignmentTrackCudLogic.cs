using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microservice.Course.Application.AssociatedEntities;
using Microservice.Course.Application.Events;
using Microservice.Course.Application.Models;
using Microservice.Course.Common.Extensions;
using Microservice.Course.Domain.Entities;
using Thunder.Platform.Core.Context;
using Thunder.Platform.Core.Domain.Repositories;
using Thunder.Platform.Cqrs;

namespace Microservice.Course.Application.BusinessLogics.EntityCud
{
    public class ParticipantAssignmentTrackCudLogic : BaseEntityCudLogic<ParticipantAssignmentTrack>
    {
        private readonly IReadOnlyRepository<Assignment> _readAssignmentOnlyRepository;
        private readonly IReadOnlyRepository<Registration> _readRegistrationOnlyRepository;

        public ParticipantAssignmentTrackCudLogic(
            IWriteOnlyRepository<ParticipantAssignmentTrack> repository,
            IThunderCqrs thunderCqrs,
            IUserContext userContext,
            IReadOnlyRepository<Assignment> readAssignmentOnlyRepository,
            IReadOnlyRepository<Registration> readRegistrationOnlyRepository) : base(repository, thunderCqrs, userContext)
        {
            _readAssignmentOnlyRepository = readAssignmentOnlyRepository;
            _readRegistrationOnlyRepository = readRegistrationOnlyRepository;
        }

        public async Task InsertMany(List<ParticipantAssignmentTrack> entities, CancellationToken cancellationToken = default)
        {
            await RootRepository.InsertManyAsync(entities);

            await SendChangeEvent(entities, ParticipantAssignmentTrackChangeType.Created, cancellationToken);
        }

        public async Task Update(ParticipantAssignmentTrack entity, CancellationToken cancellationToken = default)
        {
            await RootRepository.UpdateAsync(entity);

            await SendChangeEvent(new List<ParticipantAssignmentTrack>() { entity }, ParticipantAssignmentTrackChangeType.Updated, cancellationToken);
        }

        public async Task UpdateMany(List<ParticipantAssignmentTrack> entities, CancellationToken cancellationToken = default)
        {
            await RootRepository.UpdateManyAsync(entities);

            await SendChangeEvent(entities, ParticipantAssignmentTrackChangeType.Updated, cancellationToken);
        }

        private async Task SendChangeEvent(List<ParticipantAssignmentTrack> entities, ParticipantAssignmentTrackChangeType changeType, CancellationToken cancellationToken)
        {
            var assignmentsDic =
                await _readAssignmentOnlyRepository.GetDictionaryByIdsAsync(entities.Select(p => p.AssignmentId).Distinct());
            var registrationsDic =
                await _readRegistrationOnlyRepository.GetDictionaryByIdsAsync(entities.Select(p => p.RegistrationId).Distinct());
            await ThunderCqrs.SendEvents(
                entities.Select(x =>
                    new ParticipantAssignmentTrackChangeEvent(
                        new ParticipantAssignmentTrackAssociatedEntity(x, assignmentsDic[x.AssignmentId], registrationsDic[x.RegistrationId]),
                        changeType)),
                cancellationToken);
        }
    }
}
