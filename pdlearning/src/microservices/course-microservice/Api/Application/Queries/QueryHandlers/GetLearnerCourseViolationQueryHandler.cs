using System.Threading;
using System.Threading.Tasks;
using Conexus.Opal.AccessControl.Infrastructure;
using Microservice.Course.Application.BusinessLogics;
using Microservice.Course.Application.Queries.Abstracts;
using Microservice.Course.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Thunder.Platform.Core.Context;
using Thunder.Platform.Core.Domain.Entities;
using Thunder.Platform.Core.Domain.Repositories;
using Thunder.Platform.Core.Domain.UnitOfWork.Abstractions;

namespace Microservice.Course.Application.Queries.QueryHandlers
{
    public class GetLearnerCourseViolationQueryHandler : BaseQueryHandler<GetLearnerCourseViolationQuery, GetLearnerCourseViolationQueryResult>
    {
        private readonly IReadOnlyRepository<CourseCriteria> _readCourseCriteriaRepository;
        private readonly IReadOnlyRepository<CourseEntity> _readCourseRepository;
        private readonly IReadOnlyRepository<ClassRun> _readClassRunRepository;

        private readonly CheckLearnerCourseCriteriaViolationLogic _checkLearnerCourseCriteriaViolationLogic;

        public GetLearnerCourseViolationQueryHandler(
            IReadOnlyRepository<CourseCriteria> readCourseCriteriaRepository,
            IReadOnlyRepository<CourseEntity> readCourseRepository,
            IReadOnlyRepository<ClassRun> readClassRunRepository,
            CheckLearnerCourseCriteriaViolationLogic checkLearnerCourseCriteriaViolationLogic,
            IUserContext userContext,
            IAccessControlContext<CourseUser> accessControlContext,
            IUnitOfWorkManager unitOfWorkManager) : base(userContext, accessControlContext, unitOfWorkManager)
        {
            _readCourseCriteriaRepository = readCourseCriteriaRepository;
            _readCourseRepository = readCourseRepository;
            _readClassRunRepository = readClassRunRepository;
            _checkLearnerCourseCriteriaViolationLogic = checkLearnerCourseCriteriaViolationLogic;
        }

        protected override async Task<GetLearnerCourseViolationQueryResult> HandleAsync(GetLearnerCourseViolationQuery query, CancellationToken cancellationToken)
        {
            var course = await _readCourseRepository.GetAsync(query.CourseId);

            var courseCriteria = await _readCourseCriteriaRepository.FirstOrDefaultAsync(p => p.Id == course.Id);
            if (courseCriteria == null)
            {
                return null;
            }

            var classRun = await _readClassRunRepository.GetAsync(query.ClassRunId);
            var courseCriteriaLearnerViolation = await _checkLearnerCourseCriteriaViolationLogic.Execute(query.CourseId, query.ClassRunId, CurrentUserIdOrDefault, courseCriteria, null, cancellationToken);

            return new GetLearnerCourseViolationQueryResult { IsCourseCriteriaForClassRunActivated = classRun.CourseCriteriaActivated, ViolationDetail = courseCriteriaLearnerViolation };
        }
    }
}
