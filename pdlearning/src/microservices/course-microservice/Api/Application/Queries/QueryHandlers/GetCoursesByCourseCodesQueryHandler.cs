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
    public class GetCoursesByCourseCodesQueryHandler : BaseQueryHandler<GetCoursesByCourseCodesQuery, IEnumerable<CourseModel>>
    {
        private readonly IReadOnlyRepository<CourseEntity> _readCourseRepository;

        public GetCoursesByCourseCodesQueryHandler(
            IReadOnlyRepository<CourseEntity> readCourseRepository,
            IUserContext userContext,
            IAccessControlContext<CourseUser> accessControlContext,
            IUnitOfWorkManager unitOfWorkManager) : base(userContext, accessControlContext, unitOfWorkManager)
        {
            _readCourseRepository = readCourseRepository;
        }

        protected override async Task<IEnumerable<CourseModel>> HandleAsync(
            GetCoursesByCourseCodesQuery query,
            CancellationToken cancellationToken)
        {
            if (query.CourseCodes == null || !query.CourseCodes.Any())
            {
                return new List<CourseModel>();
            }

            // Get published courses since this is supposed to be used by PDPM only
            var courses = await _readCourseRepository.GetAll().Where(x => query.CourseCodes.Contains(x.CourseCode)).ToListAsync(cancellationToken);
            var hasRightChecker = _readCourseRepository.GetHasAdminRightChecker(courses, AccessControlContext);
            return courses.Select(x => new CourseModel(x, hasRightChecker(x)));
        }
    }
}
