using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Conexus.Opal.AccessControl.Extensions;
using Conexus.Opal.AccessControl.Infrastructure;
using Microservice.Course.Application.DomainExtensions;
using Microservice.Course.Application.Enums;
using Microservice.Course.Application.Models;
using Microservice.Course.Application.Queries.Abstracts;
using Microservice.Course.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Thunder.Platform.Core.Application.Dtos;
using Thunder.Platform.Core.Context;
using Thunder.Platform.Core.Domain.Entities;
using Thunder.Platform.Core.Domain.Repositories;
using Thunder.Platform.Core.Domain.UnitOfWork.Abstractions;

namespace Microservice.Course.Application.Queries.QueryHandlers
{
    public class GetSessionsByClassRunIdQueryHandler : BaseQueryHandler<GetSessionsByClassRunIdQuery, PagedResultDto<SessionModel>>
    {
        private readonly IReadOnlyRepository<ClassRun> _readClassRunRepository;
        private readonly IReadOnlyRepository<Session> _readSessionRepository;
        private readonly IReadOnlyRepository<CourseEntity> _readCourseRepository;

        public GetSessionsByClassRunIdQueryHandler(
            IUserContext userContext,
            IAccessControlContext<CourseUser> accessControlContext,
            IUnitOfWorkManager unitOfWorkManager,
            IReadOnlyRepository<ClassRun> readClassRunRepository,
            IReadOnlyRepository<Session> readSessionRepository,
            IReadOnlyRepository<CourseEntity> readCourseRepository) : base(userContext, accessControlContext, unitOfWorkManager)
        {
            _readClassRunRepository = readClassRunRepository;
            _readSessionRepository = readSessionRepository;
            _readCourseRepository = readCourseRepository;
        }

        protected override async Task<PagedResultDto<SessionModel>> HandleAsync(
            GetSessionsByClassRunIdQuery query,
            CancellationToken cancellationToken)
        {
            var dbQuery = _readSessionRepository.GetAll().Where(x => x.ClassRunId == query.ClassRunId);
            dbQuery = await FilterBySearchType(dbQuery, query.SearchType, query.ClassRunId, cancellationToken);
            var totalCount = await dbQuery.CountAsync(cancellationToken);

            dbQuery = dbQuery.OrderByDescending(p => p.CreatedDate);
            dbQuery = ApplyPaging(dbQuery, query.PageInfo);

            var entities = await dbQuery.Select(x => new SessionModel(x)).ToListAsync(cancellationToken);

            return new PagedResultDto<SessionModel>(totalCount, entities);
        }

        private async Task<IQueryable<Session>> FilterBySearchType(
            IQueryable<Session> dbQuery,
            SearchSessionType searchType,
            Guid classRunId,
            CancellationToken cancellationToken)
        {
            switch (searchType)
            {
                case SearchSessionType.Learner:
                    {
                        return dbQuery.Where(x => x.ClassRunId == classRunId);
                    }

                default:
                    {
                        var classRun = await _readClassRunRepository.GetAsync(classRunId);

                        var hasCoursePermission = await _readCourseRepository
                            .GetAllWithAccessControl(AccessControlContext, CourseEntityExtensions.HasViewSessionPermissionQueryExpr(CurrentUserId, CurrentUserRoles, p => HasPermissionPrefix(p)))
                            .Where(x => x.Id == classRun.CourseId)
                            .AnyAsync(cancellationToken);

                        EnsureValidPermission(hasCoursePermission);

                        return dbQuery;
                    }
            }
        }
    }
}
