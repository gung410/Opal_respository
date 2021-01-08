using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Conexus.Opal.AccessControl.Infrastructure;
using Microservice.Course.Application.Models;
using Microservice.Course.Application.Queries.Abstracts;
using Microservice.Course.Application.SharedQueries;
using Microservice.Course.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Thunder.Platform.Core.Application.Dtos;
using Thunder.Platform.Core.Context;
using Thunder.Platform.Core.Domain.Repositories;
using Thunder.Platform.Core.Domain.UnitOfWork.Abstractions;

namespace Microservice.Course.Application.Queries.QueryHandlers
{
    public class SearchUsersQueryHandler : BaseQueryHandler<SearchUsersQuery, PagedResultDto<UserModel>>
    {
        private readonly IReadOnlyRepository<CourseUser> _readUserRepository;
        private readonly IReadOnlyRepository<CourseEntity> _readCourseRepository;
        private readonly GetCanApplyCourseUsersSharedQuery _getCanApplyCourseUsersSharedQuery;
        private readonly GetUsersSharedQuery _getUsersSharedQuery;

        public SearchUsersQueryHandler(
            IReadOnlyRepository<CourseUser> readUserRepository,
            IReadOnlyRepository<CourseEntity> readCourseRepository,
            IUserContext userContext,
            IAccessControlContext<CourseUser> accessControlContext,
            IUnitOfWorkManager unitOfWorkManager,
            GetCanApplyCourseUsersSharedQuery getCanApplyCourseUsersSharedQuery,
            GetUsersSharedQuery getUsersSharedQuery) : base(userContext, accessControlContext, unitOfWorkManager)
        {
            _readUserRepository = readUserRepository;
            _readCourseRepository = readCourseRepository;
            _getCanApplyCourseUsersSharedQuery = getCanApplyCourseUsersSharedQuery;
            _getUsersSharedQuery = getUsersSharedQuery;
        }

        protected override async Task<PagedResultDto<UserModel>> HandleAsync(SearchUsersQuery query, CancellationToken cancellationToken)
        {
            var dbQuery = _getUsersSharedQuery.BySearchText(_readUserRepository.GetAll(), query.SearchText);

            if (query.CanApplyForCourse != null)
            {
                var course = await _readCourseRepository.GetAsync(query.CanApplyForCourse.CourseId);
                dbQuery = _getCanApplyCourseUsersSharedQuery.FromQuery(dbQuery, course, query.CanApplyForCourse.FollowCourseTargetParticipant);
            }

            var totalCount = await dbQuery.CountAsync(cancellationToken);

            if (query.PageInfo != null && query.PageInfo.MaxResultCount == 0)
            {
                return new PagedResultDto<UserModel>(totalCount);
            }

            dbQuery = ApplyPaging(dbQuery, query.PageInfo);

            return new PagedResultDto<UserModel>(totalCount, dbQuery.Select(p => new UserModel(p)).ToList());
        }
    }
}
