using System;
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
    public class CommentCudLogic : BaseEntityCudLogic<Comment>
    {
        private readonly IWriteOnlyRepository<CommentTrack> _writeCommentTrackRepository;

        public CommentCudLogic(
            IWriteOnlyRepository<Comment> repository,
            IWriteOnlyRepository<CommentTrack> writeCommentTrackRepository,
            IThunderCqrs thunderCqrs,
            IUserContext userContext) : base(repository, thunderCqrs, userContext)
        {
            _writeCommentTrackRepository = writeCommentTrackRepository;
        }

        public async Task Insert(Comment entity, CancellationToken cancellationToken = default)
        {
            await RootRepository.InsertAsync(entity);

            await _writeCommentTrackRepository.InsertAsync(new CommentTrack()
            {
                Id = Guid.NewGuid(),
                CommentId = entity.Id,
                UserId = entity.UserId
            });

            await ThunderCqrs.SendEvent(
                new CommentChangeEvent(entity, CommentChangeType.Created),
                cancellationToken);
        }

        public async Task InsertMany(List<Comment> entities, CancellationToken cancellationToken = default)
        {
            await RootRepository.InsertManyAsync(entities);

            await _writeCommentTrackRepository.InsertManyAsync(entities.Select(x => new CommentTrack()
            {
                Id = Guid.NewGuid(),
                CommentId = x.Id,
                UserId = x.UserId
            }));

            await ThunderCqrs.SendEvents(
                entities.Select(x => new CommentChangeEvent(x, CommentChangeType.Created)),
                cancellationToken);
        }

        public async Task Update(Comment entity, CancellationToken cancellationToken = default)
        {
            await RootRepository.UpdateAsync(entity);

            await ThunderCqrs.SendEvent(
                new CommentChangeEvent(entity, CommentChangeType.Updated),
                cancellationToken);
        }

        public async Task UpdateMany(List<Comment> entities, CancellationToken cancellationToken = default)
        {
            await RootRepository.UpdateManyAsync(entities);

            await ThunderCqrs.SendEvents(
                entities.Select(x => new CommentChangeEvent(x, CommentChangeType.Updated)),
                cancellationToken);
        }

        public async Task Delete(Comment entity, CancellationToken cancellationToken = default)
        {
            await RootRepository.DeleteAsync(entity);

            await ThunderCqrs.SendEvent(
                new CommentChangeEvent(entity, CommentChangeType.Deleted),
                cancellationToken);
        }

        public async Task DeleteMany(List<Comment> entities, CancellationToken cancellationToken = default)
        {
            await RootRepository.DeleteManyAsync(entities);

            await ThunderCqrs.SendEvents(
                entities.Select(x => new CommentChangeEvent(x, CommentChangeType.Deleted)),
                cancellationToken);
        }

        public async Task InsertManyCommentTrack(IEnumerable<CommentTrack> entities)
        {
            await _writeCommentTrackRepository.InsertManyAsync(entities);
        }
    }
}
