using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Conexus.Opal.AccessControl.Entities;
using Conexus.Opal.AccessControl.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Thunder.Platform.Core.Domain.Repositories;

namespace Conexus.Opal.AccessControl.Helpers
{
    public static class UserHierarchyHelper
    {
        public static IQueryable<int> GetInferiorDepartmentIds<TUser>(
            Guid? userId,
            IRepository<TUser> userRepository,
            IRepository<HierarchyDepartment> hierarchyDepartmentRepository,
            IRepository<DepartmentType> departmentTypeRepository,
            IRepository<DepartmentTypeDepartment> departmentTypeDepartmentRepository) where TUser : class, IUserEntity
        {
            var userDepartmentId = userRepository.FirstOrDefault(p => p.Id == userId)?.DepartmentId;
            var userDepartmentPath = hierarchyDepartmentRepository.FirstOrDefault(p => p.DepartmentId == userDepartmentId)?.Path;

            var allDepartmentIdsQuery = hierarchyDepartmentRepository.GetAll()
                .Where(p => userDepartmentPath != null && EF.Functions.Like(p.Path, $"{userDepartmentPath}%"))
                .Select(p => p.DepartmentId);

            var userBelongToSchoolLevel = CheckDepartmentBelongToSchoolLevel(userDepartmentId, departmentTypeRepository, departmentTypeDepartmentRepository);
            if (!userBelongToSchoolLevel)
            {
                var notSchoolDeparmentIdsQuery = departmentTypeRepository.GetAll().Where(p => p.ExtId != CommonDepartmentTypeExtIDs.SCHOOL)
                    .Join(
                        departmentTypeDepartmentRepository.GetAll(),
                        p => p.DepartmentTypeId,
                        p => p.DepartmentTypeId,
                        (p, p1) => p1.DepartmentId)
                    .Distinct();
                return allDepartmentIdsQuery.Join(notSchoolDeparmentIdsQuery, p => p, p => p, (p, p1) => p);
            }

            return allDepartmentIdsQuery;
        }

        // Check a department Id belong to school level or not.
        public static bool CheckDepartmentBelongToSchoolLevel(
            int? departmentId,
            IRepository<DepartmentType> departmentTypeRepository,
            IRepository<DepartmentTypeDepartment> departmentTypeDepartmentRepository)
        {
            var schoolDepartmentTypeId = departmentTypeRepository.FirstOrDefault(dt => dt.ExtId == CommonDepartmentTypeExtIDs.SCHOOL)?.DepartmentTypeId;

            return departmentTypeDepartmentRepository
                .Count(dtd => dtd.DepartmentTypeId == schoolDepartmentTypeId && dtd.DepartmentId == departmentId) > 0;
        }
    }
}
