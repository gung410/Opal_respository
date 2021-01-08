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
    public class AttendanceTrackingCudLogic : BaseEntityCudLogic<AttendanceTracking>
    {
        public AttendanceTrackingCudLogic(
            IWriteOnlyRepository<AttendanceTracking> repository,
            IThunderCqrs thunderCqrs,
            IUserContext userContext) : base(repository, thunderCqrs, userContext)
        {
        }

        public async Task Insert(AttendanceTracking entity, CancellationToken cancellationToken = default)
        {
            await RootRepository.InsertAsync(entity);

            await ThunderCqrs.SendEvent(
                new AttendanceTrackingChangeEvent(entity, AttendanceTrackingChangeType.Created),
                cancellationToken);
        }

        public async Task InsertMany(List<AttendanceTracking> entities, CancellationToken cancellationToken = default)
        {
            await RootRepository.InsertManyAsync(entities);

            await ThunderCqrs.SendEvents(
                entities.Select(x => new AttendanceTrackingChangeEvent(x, AttendanceTrackingChangeType.Created)),
                cancellationToken);
        }

        public async Task Update(AttendanceTracking entity, CancellationToken cancellationToken = default)
        {
            await RootRepository.UpdateAsync(entity);

            await ThunderCqrs.SendEvent(
                new AttendanceTrackingChangeEvent(entity, AttendanceTrackingChangeType.Updated),
                cancellationToken);
        }

        public async Task UpdateMany(List<AttendanceTracking> entities, CancellationToken cancellationToken = default)
        {
            await RootRepository.UpdateManyAsync(entities);

            await ThunderCqrs.SendEvents(
                entities.Select(x => new AttendanceTrackingChangeEvent(x, AttendanceTrackingChangeType.Updated)),
                cancellationToken);
        }

        public async Task Delete(AttendanceTracking entity, CancellationToken cancellationToken = default)
        {
            await RootRepository.DeleteAsync(entity);

            await ThunderCqrs.SendEvent(
                new AttendanceTrackingChangeEvent(entity, AttendanceTrackingChangeType.Deleted),
                cancellationToken);
        }

        public async Task DeleteMany(List<AttendanceTracking> entities, CancellationToken cancellationToken = default)
        {
            await RootRepository.DeleteManyAsync(entities);

            await ThunderCqrs.SendEvents(
                entities.Select(x => new AttendanceTrackingChangeEvent(x, AttendanceTrackingChangeType.Deleted)),
                cancellationToken);
        }
    }
}
