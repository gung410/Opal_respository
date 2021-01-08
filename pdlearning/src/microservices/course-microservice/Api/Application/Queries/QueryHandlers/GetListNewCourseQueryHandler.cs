using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Conexus.Opal.AccessControl.Extensions;
using Conexus.Opal.AccessControl.Infrastructure;
using Microservice.Course.Application.Models;
using Microservice.Course.Application.Queries.Abstracts;
using Microservice.Course.Domain.Entities;
using Microservice.Course.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Thunder.Platform.Core.Application.Dtos;
using Thunder.Platform.Core.Context;
using Thunder.Platform.Core.Domain.Repositories;
using Thunder.Platform.Core.Domain.UnitOfWork.Abstractions;

namespace Microservice.Course.Application.Queries.QueryHandlers
{
    public class GetListNewCourseQueryHandler : BaseQueryHandler<GetListNewCourseQuery, PagedResultDto<CourseModel>>
    {
        private readonly IReadOnlyRepository<CourseEntity> _readCourseRepository;

        public GetListNewCourseQueryHandler(
            IReadOnlyRepository<CourseEntity> readCourseRepository,
            IUserContext userContext,
            IAccessControlContext<CourseUser> accessControlContext,
            IUnitOfWorkManager unitOfWorkManager) : base(userContext, accessControlContext, unitOfWorkManager)
        {
            _readCourseRepository = readCourseRepository;
        }

        protected override async Task<PagedResultDto<CourseModel>> HandleAsync(GetListNewCourseQuery query, CancellationToken cancellationToken)
        {
            var dbQuery = _readCourseRepository
                .GetAllWithAccessControl(AccessControlContext)
                .Where(_ => _.Status == CourseStatus.Published);

            var totalCount = await dbQuery.CountAsync(cancellationToken);

            dbQuery = ApplySorting(dbQuery, query.PageInfo, $"{nameof(CourseEntity.CreatedDate)} DESC");
            dbQuery = ApplyPaging(dbQuery, query.PageInfo);

            var courses = await dbQuery.ToListAsync(cancellationToken);
            var hasRightChecker = _readCourseRepository.GetHasAdminRightChecker(courses, AccessControlContext);
            var entities = courses.Select(p => new CourseModel(p, hasRightChecker(p), null)).ToList();

            return new PagedResultDto<CourseModel>(totalCount, entities);
        }
    }
}
