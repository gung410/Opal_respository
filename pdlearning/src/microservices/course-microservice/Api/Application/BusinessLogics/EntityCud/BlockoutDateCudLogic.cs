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
    public class BlockoutDateCudLogic : BaseEntityCudLogic<BlockoutDate>
    {
        public BlockoutDateCudLogic(
            IWriteOnlyRepository<BlockoutDate> repository,
            IThunderCqrs thunderCqrs,
            IUserContext userContext) : base(repository, thunderCqrs, userContext)
        {
        }

        public async Task Insert(BlockoutDate entity, CancellationToken cancellationToken = default)
        {
            await RootRepository.InsertAsync(entity);

            await ThunderCqrs.SendEvent(
                new BlockoutDateChangeEvent(entity, BlockoutDateChangeType.Created),
                cancellationToken);
        }

        public async Task InsertMany(List<BlockoutDate> entities, CancellationToken cancellationToken = default)
        {
            await RootRepository.InsertManyAsync(entities);

            await ThunderCqrs.SendEvents(
                entities.Select(x => new BlockoutDateChangeEvent(x, BlockoutDateChangeType.Created)),
                cancellationToken);
        }

        public async Task Update(BlockoutDate entity, CancellationToken cancellationToken = default)
        {
            await RootRepository.UpdateAsync(entity);

            await ThunderCqrs.SendEvent(
                new BlockoutDateChangeEvent(entity, BlockoutDateChangeType.Updated),
                cancellationToken);
        }

        public async Task UpdateMany(List<BlockoutDate> entities, CancellationToken cancellationToken = default)
        {
            await RootRepository.UpdateManyAsync(entities);

            await ThunderCqrs.SendEvents(
                entities.Select(x => new BlockoutDateChangeEvent(x, BlockoutDateChangeType.Updated)),
                cancellationToken);
        }

        public async Task Delete(BlockoutDate entity, CancellationToken cancellationToken = default)
        {
            await RootRepository.DeleteAsync(entity);

            await ThunderCqrs.SendEvent(
                new BlockoutDateChangeEvent(entity, BlockoutDateChangeType.Deleted),
                cancellationToken);
        }

        public async Task DeleteMany(List<BlockoutDate> entities, CancellationToken cancellationToken = default)
        {
            await RootRepository.DeleteManyAsync(entities);

            await ThunderCqrs.SendEvents(
                entities.Select(x => new BlockoutDateChangeEvent(x, BlockoutDateChangeType.Deleted)),
                cancellationToken);
        }
    }
}
