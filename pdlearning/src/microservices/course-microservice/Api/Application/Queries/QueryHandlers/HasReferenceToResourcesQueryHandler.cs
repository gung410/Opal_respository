using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Conexus.Opal.AccessControl.Infrastructure;
using Microservice.Course.Application.Queries.Abstracts;
using Microservice.Course.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Thunder.Platform.Core.Context;
using Thunder.Platform.Core.Domain.Repositories;
using Thunder.Platform.Core.Domain.UnitOfWork.Abstractions;

namespace Microservice.Course.Application.Queries.QueryHandlers
{
    public class HasReferenceToResourceQueryHandler : BaseQueryHandler<HasReferenceToResourceQuery, bool>
    {
        private readonly IReadOnlyRepository<Lecture> _readLectureRepository;
        private readonly IReadOnlyRepository<LectureContent> _readLectureContentRepository;
        private readonly IReadOnlyRepository<CourseEntity> _readCourseRepository;

        public HasReferenceToResourceQueryHandler(
            IReadOnlyRepository<Lecture> readLectureRepository,
            IReadOnlyRepository<LectureContent> readLectureContentRepository,
            IReadOnlyRepository<CourseEntity> readCourseRepository,
            IUserContext userContext,
            IAccessControlContext<CourseUser> accessControlContext,
            IUnitOfWorkManager unitOfWorkManager) : base(userContext, accessControlContext, unitOfWorkManager)
        {
            _readLectureRepository = readLectureRepository;
            _readLectureContentRepository = readLectureContentRepository;
            _readCourseRepository = readCourseRepository;
        }

        protected override async Task<bool> HandleAsync(HasReferenceToResourceQuery query, CancellationToken cancellationToken)
        {
            var resourceIdUserInCourseLecturesMap = await _readLectureContentRepository.GetAll()
                .Where(p => p.ResourceId != null && query.ResourceId.Equals(p.ResourceId.Value))
                .Join(_readLectureRepository.GetAll(), p => p.LectureId, p => p.Id, (lectureContent, lecture) => new { lectureContent.ResourceId, lecture.CourseId })
                .Join(_readCourseRepository.GetAll(), p => p.CourseId, p => p.Id, (courseResourceMap, course) => courseResourceMap)
                .Distinct()
                .ToListAsync(cancellationToken);

            return resourceIdUserInCourseLecturesMap.Any();
        }
    }
}
