using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Conexus.Opal.AccessControl.Entities;
using Conexus.Opal.AccessControl.Infrastructure;
using Microservice.Learner.Application.Models;
using Microservice.Learner.Domain.Entities;
using Microservice.Learner.Domain.ValueObject;
using Microsoft.EntityFrameworkCore;
using Thunder.Platform.Core.Application.Dtos;
using Thunder.Platform.Core.Context;
using Thunder.Platform.Core.Domain.Entities;
using Thunder.Platform.Core.Domain.Repositories;
using Thunder.Platform.Core.Extensions;

namespace Microservice.Learner.Application.Queries.QueryHandlers
{
    public class GetUsersQueryHandler : BaseQueryHandler<GetUsersQuery, PagedResultDto<UserModel>>
    {
        private readonly IRepository<LearnerUser> _userRepository;
        private readonly IRepository<HierarchyDepartment> _hierarchyDepartmentRepository;
        private readonly IRepository<DepartmentType> _departmentTypeRepository;
        private readonly IRepository<DepartmentTypeDepartment> _departmentTypeDepartmentRepository;
        private readonly IRepository<UserFollowing> _userFollowingRepository;

        public GetUsersQueryHandler(
            IRepository<LearnerUser> userRepository,
            IRepository<HierarchyDepartment> hierarchyDepartmentRepository,
            IRepository<DepartmentType> departmentTypeRepository,
            IRepository<DepartmentTypeDepartment> departmentTypeDepartmentRepository,
            IRepository<UserFollowing> userFollowingRepository,
            IUserContext userContext) : base(userContext)
        {
            _userRepository = userRepository;
            _hierarchyDepartmentRepository = hierarchyDepartmentRepository;
            _departmentTypeRepository = departmentTypeRepository;
            _departmentTypeDepartmentRepository = departmentTypeDepartmentRepository;
            _userFollowingRepository = userFollowingRepository;
        }

        protected override async Task<PagedResultDto<UserModel>> HandleAsync(GetUsersQuery query, CancellationToken cancellationToken)
        {
            var userDepartmentId = await GetUserDepartmentId(query);

            var hierarchyDepartmentQuery = _hierarchyDepartmentRepository.GetAll();

            if (query.IncludeSubDepartments)
            {
                var userDepartmentPath = await GetUserDepartmentPath(query, userDepartmentId);
                hierarchyDepartmentQuery = hierarchyDepartmentQuery
                    .Where(HierarchyDepartment.FromSubsequentDepartment(userDepartmentPath));
            }
            else
            {
                hierarchyDepartmentQuery = hierarchyDepartmentQuery
                    .Where(HierarchyDepartment.FromDepartmentId(userDepartmentId));
            }

            var departmentIdsQuery = hierarchyDepartmentQuery.Select(p => p.DepartmentId);

            if (query.IncludeSubDepartments)
            {
                var userBelongToSchoolLevel = await IsUserBelongToSchoolLevel(userDepartmentId);

                if (!userBelongToSchoolLevel)
                {
                    var filteredDepartmentTypeDepartments = _departmentTypeDepartmentRepository
                        .GetAll()
                        .Join(
                            departmentIdsQuery,
                            dtd => dtd.DepartmentId,
                            id => id,
                            (dtd, id) => dtd);

                    var notSchoolDepartmentIdsQuery = _departmentTypeRepository
                        .GetAll()
                        .Where(p => p.ExtId != CommonDepartmentTypeExtIDs.SCHOOL)
                        .Join(
                            filteredDepartmentTypeDepartments,
                            dt => dt.DepartmentTypeId,
                            dtd => dtd.DepartmentTypeId,
                            (p, p1) => p1.DepartmentId)
                        .Distinct();

                    departmentIdsQuery = notSchoolDepartmentIdsQuery;
                }
            }

            var usersQuery = _userRepository
                .GetAll()
                .Where(p => p.Id != CurrentUserId)
                .Where(p => !string.IsNullOrEmpty(p.FirstName) || !string.IsNullOrEmpty(p.LastName))
                .Where(p => p.Status == LearnerUserStatus.Active)
                .Join(
                    departmentIdsQuery,
                    user => user.DepartmentId,
                    deptId => deptId,
                    (user, deptId) => user);

            query.SearchText = query.SearchText?.ToLower();

            // Combine FirstName & LastName but currently the system only store FirstName for name
            // We must be checked the data and concat after that It compare with query search text
            usersQuery = usersQuery
                    .WhereIf(
                        !string.IsNullOrEmpty(query.SearchText),
                        r =>
                        ((r.FirstName == null ? string.Empty : r.FirstName.ToLower())
                            + " " + (r.LastName == null ? string.Empty : r.LastName.ToLower())).Contains(query.SearchText) ||
                        (!string.IsNullOrEmpty(r.Email) && r.Email.Contains(query.SearchText)));

            var userFollowing = GetUserFollowing(query);

            var combinedUser = usersQuery.Union(userFollowing).Distinct();

            var usersCount = await combinedUser.CountAsync(cancellationToken);
            combinedUser = ApplySorting(combinedUser, query.PageInfo, $"{nameof(UserEntity.FirstName)} ASC");
            combinedUser = ApplyPaging(combinedUser, query.PageInfo);

            var users = await combinedUser
                .Select(user => new UserModel(user, false))
                .ToListAsync(cancellationToken);

            if (users.Count > 0)
            {
                users = UpdateFollowingUser(users, userFollowing);
            }

            return new PagedResultDto<UserModel>(usersCount, users);
        }

        private async Task<int> GetUserDepartmentId(GetUsersQuery query)
        {
            var currentUser = await _userRepository.FirstOrDefaultAsync(CurrentUserIdOrDefault);
            if (currentUser == null)
            {
                throw new EntityNotFoundException(typeof(LearnerUser), query.QueryId);
            }

            return currentUser.DepartmentId;
        }

        private async Task<bool> IsUserBelongToSchoolLevel(int userDepartmentId)
        {
            var currentDepartmentType = await _departmentTypeDepartmentRepository
                .GetAll()
                .Where(dept => dept.DepartmentId == userDepartmentId)
                .Join(
                    _departmentTypeRepository.GetAll(),
                    p => p.DepartmentTypeId,
                    p => p.DepartmentTypeId,
                    (dtd, dt) => dt)
                .FirstOrDefaultAsync();

            return currentDepartmentType != null && currentDepartmentType.ExtId == CommonDepartmentTypeExtIDs.SCHOOL;
        }

        private async Task<string> GetUserDepartmentPath(GetUsersQuery query, int userDepartmentId)
        {
            var userDepartment = await _hierarchyDepartmentRepository.FirstOrDefaultAsync(p => p.DepartmentId == userDepartmentId);

            if (userDepartment == null)
            {
                throw new EntityNotFoundException(typeof(HierarchyDepartment), query.QueryId);
            }

            return userDepartment.Path;
        }

        private IQueryable<UserEntity> GetUserFollowing(GetUsersQuery query)
        {
            // Combine FirstName & LastName but currently the system only store FirstName for name
            // We must be checked the data and concat after that It compare with query search text
            return _userFollowingRepository
                .GetAll()
                .Where(p => p.UserId == CurrentUserId)
                .Join(
                    _userRepository.GetAll()
                    .Where(p => p.Status == LearnerUserStatus.Active)
                    .WhereIf(
                        !string.IsNullOrEmpty(query.SearchText),
                        r =>
                        ((r.FirstName == null ? string.Empty : r.FirstName.ToLower())
                            + " " + (r.LastName == null ? string.Empty : r.LastName.ToLower())).Contains(query.SearchText) ||
                        (!string.IsNullOrEmpty(r.Email) && r.Email.Contains(query.SearchText))),
                    p => p.FollowingUserId,
                    p => p.Id,
                    (uf, u) => u);
        }

        private List<UserModel> UpdateFollowingUser(List<UserModel> combinedUser, IQueryable<UserEntity> userFollowing)
        {
            // Update following user in list user return
            var userIds = combinedUser.Select(p => p.Id);
            var userFollowed = userFollowing.Where(p => userIds.Contains(p.Id)).ToList();
            combinedUser.ForEach(p =>
            {
                p.IsFollowing = userFollowed.Any(q => q.Id == p.Id);
            });

            return combinedUser;
        }
    }
}
