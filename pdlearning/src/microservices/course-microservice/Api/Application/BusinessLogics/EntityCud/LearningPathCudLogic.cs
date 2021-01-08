using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microservice.Course.Application.Events;
using Microservice.Course.Application.Models;
using Microservice.Course.Application.RequestDtos;
using Microservice.Course.Domain.Entities;
using Thunder.Platform.Core.Context;
using Thunder.Platform.Core.Domain.Repositories;
using Thunder.Platform.Cqrs;

namespace Microservice.Course.Application.BusinessLogics.EntityCud
{
    public class LearningPathCudLogic : BaseEntityCudLogic<LearningPath>
    {
        private readonly IRepository<LearningPathCourse> _learningPathCourseRepository;

        public LearningPathCudLogic(
            IWriteOnlyRepository<LearningPath> repository,
            IRepository<LearningPathCourse> learningPathCourseRepository,
            IThunderCqrs thunderCqrs,
            IUserContext userContext) : base(repository, thunderCqrs, userContext)
        {
            _learningPathCourseRepository = learningPathCourseRepository;
        }

        public async Task Insert(
            LearningPath entity,
            List<LearningPathCourse> learningPathCourseEntities = null,
            CancellationToken cancellationToken = default)
        {
            await RootRepository.InsertAsync(entity);

            await _learningPathCourseRepository.InsertManyAsync(learningPathCourseEntities);

            await ThunderCqrs.SendEvent(
                new LearningPathChangeEvent(LearningPathModel.Create(entity, learningPathCourseEntities), LearningPathChangeType.Created),
                cancellationToken);
        }

        public async Task Update(
            LearningPath entity,
            List<SaveLearningPathCourseDto> toSaveLearningPathCourses,
            CancellationToken cancellationToken = default)
        {
            await RootRepository.UpdateAsync(entity);

            var learningPathCourses = await _learningPathCourseRepository.GetAllListAsync(p => p.LearningPathId == entity.Id);
            var learningPathCoursesDic = learningPathCourses.ToDictionary(p => p.Id);

            var updateLearningPathCourses = toSaveLearningPathCourses
                           .Where(p => p.Id.HasValue)
                           .Select(p =>
                           {
                               p.Update(learningPathCoursesDic[p.Id.Value]);
                               return learningPathCoursesDic[p.Id.GetValueOrDefault()];
                           })
                           .ToList();
            var insertLearningPathCourses = toSaveLearningPathCourses
               .Where(p => p.Id == null)
               .Select(p => p.Create(entity.Id, p.CourseId, p.Order.GetValueOrDefault()))
               .ToList();

            var deleteLearningPathCourses = learningPathCourses
                .Where(p => !toSaveLearningPathCourses.Where(_ => _.Id.HasValue).Select(_ => _.Id).Contains(p.Id));

            await Task.WhenAll(
               _learningPathCourseRepository.DeleteManyAsync(deleteLearningPathCourses),
               _learningPathCourseRepository.UpdateManyAsync(updateLearningPathCourses),
               _learningPathCourseRepository.InsertManyAsync(insertLearningPathCourses));

            var learningPathCoursesAfterChanged = insertLearningPathCourses.Concat(updateLearningPathCourses);

            await ThunderCqrs.SendEvent(
                new LearningPathChangeEvent(LearningPathModel.Create(entity, learningPathCoursesAfterChanged), LearningPathChangeType.Updated),
                cancellationToken);
        }

        public async Task DeleteLearningPathCourseByCourseId(Guid courseId)
        {
            var learningPathCourses = await _learningPathCourseRepository.GetAllListAsync(p => p.CourseId == courseId);

            await _learningPathCourseRepository.DeleteManyAsync(learningPathCourses);
        }
    }
}
