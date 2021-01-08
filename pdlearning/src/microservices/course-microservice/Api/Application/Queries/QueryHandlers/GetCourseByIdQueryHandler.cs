using System.Threading;
using System.Threading.Tasks;
using Conexus.Opal.AccessControl.Extensions;
using Conexus.Opal.AccessControl.Infrastructure;
using Microservice.Course.Application.Models;
using Microservice.Course.Application.Queries.Abstracts;
using Microservice.Course.Domain.Entities;
using Thunder.Platform.Core.Context;
using Thunder.Platform.Core.Domain.Repositories;
using Thunder.Platform.Core.Domain.UnitOfWork.Abstractions;

namespace Microservice.Course.Application.Queries.QueryHandlers
{
    public class GetCourseByIdQueryHandler : BaseQueryHandler<GetCourseByIdQuery, CourseModel>
    {
        private readonly IReadOnlyRepository<CourseEntity> _readCourseRepository;

        public GetCourseByIdQueryHandler(
            IReadOnlyRepository<CourseEntity> readCourseRepository,
            IUserContext userContext,
            IAccessControlContext<CourseUser> accessControlContext,
            IUnitOfWorkManager unitOfWorkManager) : base(userContext, accessControlContext, unitOfWorkManager)
        {
            _readCourseRepository = readCourseRepository;
        }

        protected override async Task<CourseModel> HandleAsync(
            GetCourseByIdQuery query,
            CancellationToken cancellationToken)
        {
            var course = await _readCourseRepository.GetAsync(query.Id);
            var hasRightChecker = _readCourseRepository.GetHasAdminRightChecker(course, AccessControlContext);
            var courseModel = new CourseModel(course, hasRightChecker(course), null);
            return courseModel;
        }
    }
}
