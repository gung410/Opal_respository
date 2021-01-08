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
    public class AnnouncementCudLogic : BaseEntityCudLogic<Announcement>
    {
        public AnnouncementCudLogic(
            IWriteOnlyRepository<Announcement> repository,
            IThunderCqrs thunderCqrs,
            IUserContext userContext) : base(repository, thunderCqrs, userContext)
        {
        }

        public async Task Insert(Announcement entity, CancellationToken cancellationToken = default)
        {
            await RootRepository.InsertAsync(entity);

            await ThunderCqrs.SendEvent(
                new AnnouncementChangeEvent(entity, AnnouncementChangeType.Created),
                cancellationToken);
        }

        public async Task InsertMany(List<Announcement> entities, CancellationToken cancellationToken = default)
        {
            await RootRepository.InsertManyAsync(entities);

            await ThunderCqrs.SendEvents(
                entities.Select(x => new AnnouncementChangeEvent(x, AnnouncementChangeType.Created)),
                cancellationToken);
        }

        public async Task Update(Announcement entity, CancellationToken cancellationToken = default)
        {
            await RootRepository.UpdateAsync(entity);

            await ThunderCqrs.SendEvent(
                new AnnouncementChangeEvent(entity, AnnouncementChangeType.Updated),
                cancellationToken);
        }

        public async Task UpdateMany(List<Announcement> entities, CancellationToken cancellationToken = default)
        {
            await RootRepository.UpdateManyAsync(entities);

            await ThunderCqrs.SendEvents(
                entities.Select(x => new AnnouncementChangeEvent(x, AnnouncementChangeType.Updated)),
                cancellationToken);
        }

        public async Task Delete(Announcement entity, CancellationToken cancellationToken = default)
        {
            await RootRepository.DeleteAsync(entity);

            await ThunderCqrs.SendEvent(
                new AnnouncementChangeEvent(entity, AnnouncementChangeType.Deleted),
                cancellationToken);
        }

        public async Task DeleteMany(List<Announcement> entities, CancellationToken cancellationToken = default)
        {
            await RootRepository.DeleteManyAsync(entities);

            await ThunderCqrs.SendEvents(
                entities.Select(x => new AnnouncementChangeEvent(x, AnnouncementChangeType.Deleted)),
                cancellationToken);
        }
    }
}
