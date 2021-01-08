using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microservice.Course.Application.Events;
using Microservice.Course.Application.Models;
using Microservice.Course.Domain.Entities;
using Thunder.Platform.Core.Context;
using Thunder.Platform.Core.Domain.Repositories;
using Thunder.Platform.Cqrs;

namespace Microservice.Course.Application.BusinessLogics.EntityCud
{
    public class LectureCudLogic : BaseEntityCudLogic<Lecture>
    {
        private readonly IWriteOnlyRepository<LectureContent> _writeContentRepository;
        private readonly IReadOnlyRepository<LectureContent> _readContentRepository;

        public LectureCudLogic(
            IWriteOnlyRepository<Lecture> repository,
            IWriteOnlyRepository<LectureContent> writeContentRepository,
            IReadOnlyRepository<LectureContent> readContentRepository,
            IThunderCqrs thunderCqrs,
            IUserContext userContext) : base(repository, thunderCqrs, userContext)
        {
            _writeContentRepository = writeContentRepository;
            _readContentRepository = readContentRepository;
        }

        public async Task Insert(Lecture entity, LectureContent contentEntity = null, CancellationToken cancellationToken = default)
        {
            await RootRepository.InsertAsync(entity);

            await _writeContentRepository.InsertAsync(contentEntity);

            await ThunderCqrs.SendEvent(
                new LectureChangeEvent(LectureModel.Create(entity, contentEntity), LectureChangeType.Created),
                cancellationToken);
        }

        public async Task InsertMany(List<Lecture> entities, List<LectureContent> contentEntities, CancellationToken cancellationToken = default)
        {
            await RootRepository.InsertManyAsync(entities);

            await _writeContentRepository.InsertManyAsync(contentEntities);

            var contentDic = contentEntities.ToDictionary(x => x.LectureId);

            await ThunderCqrs.SendEvents(
                entities
                .Select(x =>
                    new LectureChangeEvent(
                        LectureModel.Create(
                            x,
                            contentDic.GetValueOrDefault(x.Id, new LectureContent())),
                        LectureChangeType.Created)),
                cancellationToken);
        }

        public async Task Update(Lecture entity, LectureContent contentEntity = null, CancellationToken cancellationToken = default)
        {
            await RootRepository.UpdateAsync(entity);

            LectureContent content;
            if (contentEntity != null)
            {
                await _writeContentRepository.UpdateAsync(contentEntity);
                content = contentEntity;
            }
            else
            {
                content = await _readContentRepository.FirstOrDefaultAsync(x => x.LectureId == entity.Id);
            }

            await ThunderCqrs.SendEvent(
                new LectureChangeEvent(LectureModel.Create(entity, content), LectureChangeType.Updated),
                cancellationToken);
        }

        public async Task UpdateMany(List<Lecture> entities, CancellationToken cancellationToken = default)
        {
            await RootRepository.UpdateManyAsync(entities);

            var lectureIds = entities.Select(x => x.Id);

            var contents = await _readContentRepository.GetAllListAsync(x => lectureIds.Contains(x.LectureId));

            var contentDic = contents.ToDictionary(x => x.LectureId);

            await ThunderCqrs.SendEvents(
                entities
                .Select(x =>
                    new LectureChangeEvent(
                        LectureModel.Create(
                            x,
                            contentDic.GetValueOrDefault(x.Id, new LectureContent())),
                        LectureChangeType.Created)),
                cancellationToken);
        }

        public async Task Delete(Lecture entity, CancellationToken cancellationToken = default)
        {
            await RootRepository.DeleteAsync(entity);

            var content = await _readContentRepository.FirstOrDefaultAsync(_ => _.LectureId == entity.Id);

            await _writeContentRepository.DeleteAsync(content);

            await ThunderCqrs.SendEvent(
                new LectureChangeEvent(LectureModel.Create(entity, content), LectureChangeType.Deleted),
                cancellationToken);
        }

        public async Task DeleteMany(List<Lecture> entities, CancellationToken cancellationToken = default)
        {
            await RootRepository.DeleteManyAsync(entities);

            var ids = entities.Select(x => x.Id);

            var contents = await _readContentRepository.GetAllListAsync(_ => ids.Contains(_.LectureId));
            var contentDic = contents.ToDictionary(x => x.LectureId);

            await _writeContentRepository.DeleteManyAsync(contents);

            await ThunderCqrs.SendEvents(
                entities.Select(x =>
                    new LectureChangeEvent(
                        LectureModel.Create(
                            x,
                            contentDic.GetValueOrDefault(x.Id, new LectureContent())),
                        LectureChangeType.Deleted)),
                cancellationToken);
        }
    }
}
