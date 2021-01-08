using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Conexus.Opal.AccessControl.Infrastructure;
using Microservice.Course.Application.Models;
using Microservice.Course.Application.Queries.Abstracts;
using Microservice.Course.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Thunder.Platform.Core.Context;
using Thunder.Platform.Core.Domain.Entities;
using Thunder.Platform.Core.Domain.Repositories;
using Thunder.Platform.Core.Domain.UnitOfWork.Abstractions;
using Thunder.Platform.Core.Extensions;

namespace Microservice.Course.Application.Queries.QueryHandlers
{
    public class GetNoOfFinishedRegistrationQueryHandler : BaseQueryHandler<GetNoOfFinishedRegistrationQuery, NoOfFinishedRegistrationModel>
    {
        private readonly IReadOnlyRepository<Registration> _readRegistrationRepository;
        private readonly IReadOnlyRepository<CourseEntity> _readCourseRepository;
        private readonly IReadOnlyRepository<ClassRun> _readClassRunRepository;
        private readonly IReadOnlyRepository<CourseUser> _readUserRepository;

        public GetNoOfFinishedRegistrationQueryHandler(
            IReadOnlyRepository<CourseEntity> readCourseRepository,
            IReadOnlyRepository<Registration> readRegistrationRepository,
            IReadOnlyRepository<ClassRun> readClassRunRepository,
            IReadOnlyRepository<CourseUser> readUserRepository,
            IUserContext userContext,
            IAccessControlContext<CourseUser> accessControlContext,
            IUnitOfWorkManager unitOfWorkManager) : base(userContext, accessControlContext, unitOfWorkManager)
        {
            _readCourseRepository = readCourseRepository;
            _readRegistrationRepository = readRegistrationRepository;
            _readClassRunRepository = readClassRunRepository;
            _readUserRepository = readUserRepository;
        }

        protected override async Task<NoOfFinishedRegistrationModel> HandleAsync(GetNoOfFinishedRegistrationQuery query, CancellationToken cancellationToken)
        {
            var course = await _readCourseRepository.GetAsync(query.CourseId);

            var classrunQuery = _readClassRunRepository
                    .GetAll()
                    .WhereIf(query.ForClassRunEndAfter.HasValue, p => p.EndDateTime >= query.ForClassRunEndAfter)
                    .WhereIf(query.ForClassRunEndBefore.HasValue, p => p.EndDateTime <= query.ForClassRunEndBefore)
                    .Where(p => p.CourseId == course.Id);

            var registrationQuery = _readRegistrationRepository
                    .GetAll()
                    .Where(Registration.IsLearningFinishedExpr());

            var userQuery = _readUserRepository.GetAll().Where(p => p.DepartmentId == query.DepartmentId);

            var finishedRegistrationOfClassruns = classrunQuery
                    .Join(registrationQuery, p => p.Id, p => p.ClassRunId, (classrun, registration) => registration.UserId)
                    .Join(userQuery, p => p, p => p.Id, (userId, user) => userId);

            var noOfCompletedRegistrations = await finishedRegistrationOfClassruns.CountAsync(cancellationToken);

            return new NoOfFinishedRegistrationModel
            {
                CourseId = query.CourseId,
                TotalFinishedLearner = noOfCompletedRegistrations
            };
        }
    }
}
