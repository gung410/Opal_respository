using System;
using System.Collections.Generic;
using cxPlatform.Core;
using System.Linq.Expressions;
using cxPlatform.Client.ConexusBase;
using cxOrganization.Domain.Entities;
using System.Threading.Tasks;

namespace cxOrganization.Domain.Repositories
{
    /// <summary>
    /// Interface IDepartmentRepository
    /// </summary>
    public interface IDepartmentRepository : IRepository<DepartmentEntity>
    {
        PaginatedList<DepartmentEntity> GetDepartments(int ownerId, List<int> userIds, 
            List<int> customerIds,
            List<int> departmentIds,
            List<EntityStatusEnum> statusIds,
            List<int> archetypeIds,
            int parentDepartmentId,
            int childrenDepartmentId,
            List<string> departmetTypeExtIds,
            List<string> extIds,
            DateTime? lastUpdatedBefore = null,
            DateTime? lastUpdatedAfter = null,
            int pageIndex = 0,
            int pageSize = 0,
            string orderBy = "");
        Task<PaginatedList<DepartmentEntity>> GetDepartmentsAsync(int ownerId, List<int> userIds,
            List<int> customerIds,
            List<int> departmentIds,
            List<EntityStatusEnum> statusIds,
            List<int> archetypeIds,
            int parentDepartmentId,
            int childrenDepartmentId,
            List<string> departmetTypeExtIds,
            List<string> extIds,
            List<int> parentDepartmentIds = null,
            DateTime? lastUpdatedBefore = null,
            DateTime? lastUpdatedAfter = null,
            bool? externallyMastered = null,
            bool? includeDepartmentType = true,
            string searchText = "",
            int pageIndex = 0,
            int pageSize = 0,
            string orderBy = "");
        List<DepartmentEntity> GetParentDepartment(int departmentId);
        DepartmentEntity GetDepartmentByExtId(string departmentExtId, int customerId, bool includedInActiveStatus = false);
        Task<DepartmentEntity> GetDepartmentByExtIdAsync(string departmentExtId, List<int> customerIds, bool includedInActiveStatus = false);
        DepartmentEntity GetDepartmentByExtIdIncludeHd(string extId, int ownerId, bool includedInActiveStatus = false);
        List<DepartmentEntity> GetDepartmentByIdsAndArchetypeId(List<long?> departmentIds, List<int> allowArchetypeIds);
        List<DepartmentEntity> GetDepartmentsByDepartmentIds(List<int> departmentIds, bool includeDepartmentTypes = false);
        DepartmentEntity GetDepartmentIncludeDepartmentTypes(int departmentId, int ownerId);
        DepartmentEntity GetDepartmentIncludeDepartmentTypes(int departmentId);
        List<DepartmentEntity> GetDepartmentsByIdOrExtId(int? Id, string extId);
        DepartmentEntity GetDepartment(int departmentId, int ownerId, bool includeInActiveStatus = false);
        List<DepartmentEntity> GetDepartmentByNames(List<string> departmentNames);

        DepartmentEntity GetDepartmentByIdIncludeHd(int departmentId, int ownerId, int customerId);

        List<string> GetModifiedProperties(DepartmentEntity entity);

    }
}
