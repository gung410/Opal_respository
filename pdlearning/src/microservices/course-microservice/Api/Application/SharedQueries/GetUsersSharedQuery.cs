using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Conexus.Opal.AccessControl.Entities;
using Microservice.Course.Application.Enums;
using Microservice.Course.Application.SharedQueries.Abstractions;
using Microservice.Course.Domain.Entities;
using Microservice.Course.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Thunder.Platform.Core.Domain.Repositories;
using Thunder.Platform.Core.Extensions;

namespace Microservice.Course.Application.SharedQueries
{
    public class GetUsersSharedQuery : BaseSharedQuery
    {
        private readonly IReadOnlyRepository<CourseUser> _readUserRepository;
        private readonly IReadOnlyRepository<DepartmentType> _readDepartmentTypeRepository;
        private readonly IReadOnlyRepository<DepartmentTypeDepartment> _readDepartmentTypeDepartmentRepository;
        private readonly IReadOnlyRepository<Department> _readDepartmentRepository;
        private readonly GetFullTextFilteredEntitiesSharedQuery _getFullTextFilteredEntitiesSharedQuery;

        public GetUsersSharedQuery(
            IReadOnlyRepository<CourseUser> readUserRepository,
            IReadOnlyRepository<DepartmentType> readDepartmentTypeRepository,
            IReadOnlyRepository<DepartmentTypeDepartment> readDepartmentTypeDepartmentRepository,
            IReadOnlyRepository<Department> readDepartmentRepository,
            GetFullTextFilteredEntitiesSharedQuery getFullTextFilteredEntitiesSharedQuery)
        {
            _readUserRepository = readUserRepository;
            _readDepartmentTypeRepository = readDepartmentTypeRepository;
            _readDepartmentTypeDepartmentRepository = readDepartmentTypeDepartmentRepository;
            _readDepartmentRepository = readDepartmentRepository;
            _getFullTextFilteredEntitiesSharedQuery = getFullTextFilteredEntitiesSharedQuery;
        }

        public IQueryable<CourseUser> BySearchText(IQueryable<CourseUser> userQuery, string searchText)
        {
            return _getFullTextFilteredEntitiesSharedQuery.BySearchText(userQuery, searchText, user => user.FullTextSearch);
        }

        public IQueryable<CourseUser> ByIds(List<Guid> userIds, List<int> departmentIds = null)
        {
            var byUserIdsQuery = _readUserRepository.GetAll().Where(x => userIds != null && userIds.Contains(x.Id));
            if (departmentIds != null && departmentIds.Any())
            {
                var byDepartmentIdsQuery = _readUserRepository.GetAll().Where(x => departmentIds.Contains(x.DepartmentId));
                return byUserIdsQuery.Union(byDepartmentIdsQuery);
            }

            return byUserIdsQuery;
        }

        public async Task<List<CourseUser>> ByEmails(List<string> userEmails)
        {
            var userInfos = new List<CourseUser>();

            for (int i = 0; i < userEmails.Count; i += 100)
            {
                var emails = userEmails.Skip(i).Take(100);
                var userInfosTemp = await _readUserRepository
                    .GetAll()
                    .Where(x => emails.Contains(x.Email))
                    .ToListAsync();

                userInfos.AddRange(userInfosTemp);
            }

            return userInfos;
        }

        public Task<List<CourseUser>> ByDepartmentTypeAndRoles(
             DepartmentTypeEnum departmentType,
             List<string> userRoles,
             CancellationToken cancellationToken = default)
        {
            var departmentTypeQuery = _readDepartmentTypeRepository.GetAll();
            var deparmentTypeDepartmentQuery = _readDepartmentTypeDepartmentRepository.GetAll();
            var departmentQuery = _readDepartmentRepository.GetAll();
            var userQuery = _readUserRepository.GetAll();

            var usersbyDepartmentType = departmentTypeQuery
                .Join(deparmentTypeDepartmentQuery, dt => dt.DepartmentTypeId, dtd => dtd.DepartmentTypeId, (dt, dtd) => new { dt, dtd })
                .Join(departmentQuery, departmentTypeGroup => departmentTypeGroup.dtd.DepartmentId, d => d.DepartmentId, (departmentTypeGroup, d) => new { departmentTypeGroup, d })
                .Join(userQuery, departmentGrp => departmentGrp.d.DepartmentId, user => user.DepartmentId, (departmentGrp, user) => new { departmentGrp, user })
                .Where(x => x.departmentGrp.departmentTypeGroup.dt.ExtId == departmentType.ToString() && x.user.Status == CourseUserStatus.Active)
                .Select(grouping => grouping.user).Distinct();

            usersbyDepartmentType = usersbyDepartmentType.WhereIf(!userRoles.IsNullOrEmpty(), CourseUser.HasRoleExpr(userRoles));
            return usersbyDepartmentType.ToListAsync(cancellationToken);
        }
    }
}
