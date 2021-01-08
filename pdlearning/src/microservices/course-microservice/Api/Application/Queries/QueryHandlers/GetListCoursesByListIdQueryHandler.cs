using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Conexus.Opal.AccessControl.Extensions;
using Conexus.Opal.AccessControl.Infrastructure;
using Microservice.Course.Application.Models;
using Microservice.Course.Application.Queries.Abstracts;
using Microservice.Course.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Thunder.Platform.Core.Context;
using Thunder.Platform.Core.Domain.Repositories;
using Thunder.Platform.Core.Domain.UnitOfWork.Abstractions;

namespace Microservice.Course.Application.Queries.QueryHandlers
{
    public class GetListCoursesByListIdQueryHandler : BaseQueryHandler<GetListCoursesByListIdQuery, List<CourseModel>>
    {
        private readonly IReadOnlyRepository<CourseEntity> _readCourseRepository;

        public GetListCoursesByListIdQueryHandler(
            IReadOnlyRepository<CourseEntity> readCourseRepository,
            IUserContext userContext,
            IAccessControlContext<CourseUser> accessControlContext,
            IUnitOfWorkManager unitOfWorkManager) : base(userContext, accessControlContext, unitOfWorkManager)
        {
            _readCourseRepository = readCourseRepository;
        }

        protected override async Task<List<CourseModel>> HandleAsync(GetListCoursesByListIdQuery query, CancellationToken cancellationToken)
        {
            var courses = await _readCourseRepository.GetAll().Where(_ => query.ListIds.Contains(_.Id)).ToListAsync(cancellationToken);
            var hasRightChecker = _readCourseRepository.GetHasAdminRightChecker(courses, AccessControlContext);
            return courses.Select(p => new CourseModel(p, hasRightChecker(p), null)).ToList();
        }
    }
}
