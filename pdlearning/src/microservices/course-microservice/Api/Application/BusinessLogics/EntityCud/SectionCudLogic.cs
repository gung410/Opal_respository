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
    public class SectionCudLogic : BaseEntityCudLogic<Section>
    {
        private readonly LectureCudLogic _lectureCudLogic;
        private readonly IReadOnlyRepository<Lecture> _readLectureRepository;

        public SectionCudLogic(
            IWriteOnlyRepository<Section> repository,
            IReadOnlyRepository<Lecture> readLectureRepository,
            LectureCudLogic lectureCudLogic,
            IThunderCqrs thunderCqrs,
            IUserContext userContext) : base(repository, thunderCqrs, userContext)
        {
            _lectureCudLogic = lectureCudLogic;
            _readLectureRepository = readLectureRepository;
        }

        public async Task Insert(Section entity, CancellationToken cancellationToken = default)
        {
            await RootRepository.InsertAsync(entity);

            await ThunderCqrs.SendEvent(
                new SectionChangeEvent(entity, SectionChangeType.Created),
                cancellationToken);
        }

        public async Task InsertMany(List<Section> entities, CancellationToken cancellationToken = default)
        {
            await RootRepository.InsertManyAsync(entities);

            await ThunderCqrs.SendEvents(
                entities.Select(x => new SectionChangeEvent(x, SectionChangeType.Created)),
                cancellationToken);
        }

        public async Task Update(Section entity, CancellationToken cancellationToken = default)
        {
            await RootRepository.UpdateAsync(entity);

            await ThunderCqrs.SendEvent(
                new SectionChangeEvent(entity, SectionChangeType.Updated),
                cancellationToken);
        }

        public async Task UpdateMany(List<Section> entities, CancellationToken cancellationToken = default)
        {
            await RootRepository.UpdateManyAsync(entities);

            await ThunderCqrs.SendEvents(
                entities.Select(x => new SectionChangeEvent(x, SectionChangeType.Updated)),
                cancellationToken);
        }

        public async Task Delete(Section entity, CancellationToken cancellationToken = default)
        {
            await RootRepository.DeleteAsync(entity);

            var lectures = await _readLectureRepository.GetAllListAsync(_ => _.SectionId == entity.Id);

            await _lectureCudLogic.DeleteMany(lectures);

            await ThunderCqrs.SendEvent(
                new SectionChangeEvent(entity, SectionChangeType.Deleted),
                cancellationToken);
        }

        public async Task DeleteMany(List<Section> entities, CancellationToken cancellationToken = default)
        {
            await RootRepository.DeleteManyAsync(entities);

            var ids = entities.Select(x => x.Id);

            var lectures = await _readLectureRepository.GetAllListAsync(_ => ids.Contains(_.SectionId.Value));

            await _lectureCudLogic.DeleteMany(lectures);

            await ThunderCqrs.SendEvents(
                entities.Select(x => new SectionChangeEvent(x, SectionChangeType.Deleted)),
                cancellationToken);
        }
    }
}
