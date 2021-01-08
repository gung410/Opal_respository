using cxOrganization.Client.Departments;
using cxOrganization.Domain.Entities;
using cxPlatform.Client.ConexusBase;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace cxOrganization.Domain.Services
{
    public interface IHierarchyDepartmentService
    {
        HierarchyDepartmentEntity GetH_DByDepartmentID(int departmentId, bool includeDepartment = false, bool allowGetDepartmentDeleted = false);
        HierarchyDepartmentEntity GetHierachyDepartment(int hierarchyId, int departmentId);
        IList<HierarchyDepartmentEntity> GetChildHds(string path, bool includeDepartment = true, bool includeInActiveStatus = false, List<int> departmentTypeIds = null, List<int> departmentIds = null);
        HierarchyDepartmentEntity GetHierachyDepartmentByHierachyIdAndDepartmentId(int hierarchyId, int departmentId);
        List<int> GetAllDepartmentIdsFromAHierachyDepartmentToBelow(int departmentId);
        List<int> GetAllDepartmentIdsFromAHierachyDepartmentToTheTop(int departmentId);
        Task<List<int>> GetAllDepartmentIdsFromAHierachyDepartmentToTheTopAsync(int departmentId);

        /// <summary>
        /// Get list HD by a filter argument
        /// </summary>
        /// <returns>H_D.</returns>
        List<HierarchyDepartmentEntity> GetHierarchyDepartmentEntities(int ownerId,
               int hierarchyId,
               List<int?> customerIds = null,
               List<int> hdIds = null,
               List<int> departmentIds = null,
               List<string> departmentExtIds = null,
               List<ArchetypeEnum> departmentArchetypes = null,
               List<EntityStatusEnum> departmentStatuses = null,
               bool includeDepartment = false,
               string orderBy = null);

        /// <summary>Update the hierarchy department</summary>
        /// <param name="hierarchyDepartment">The hierarchy department.</param>
        /// <returns>H_D.</returns>
        HierarchyDepartmentEntity UpdateHierarchyDepartment(HierarchyDepartmentEntity hierarchyDepartment);

        HierarchyDepartmentEntity GetById(int hdId);
        HierarchyDepartmentEntity GetCurrentHierarchyDepartment();

        List<int> GetAllDepartmentIdsFromAHierachyDepartmentToBelowByHdId(int hierarchyDepartmentId, bool getAllStatus = false);
        List<int> GetAllHDIdsFromAHierachyDepartmentToBelowByHdId(int hDId, bool getAllStatus = false);
        List<HierachyDepartmentIdentityDto> GetHierarchyDepartmentIdentities(int hierarchyId, int departmentId,
           bool includeParentHDs = true, bool includeChildrenHDs = false,
           int ownerId = 0, List<int> customerIds = null,
           List<EntityStatusEnum> departmentEntityStatuses = null, 
           int? maxChildrenLevel = null, 
           bool countChildren = false, List<int> departmentTypeIds = null,
           string departmentName = null, bool includeDepartmentType = false, bool getParentNode = false,
           bool countUser = false, List<EntityStatusEnum> countUserEntityStatuses = null,
           List<string> jsonDynamicData = null,
           bool checkPermission = false);

        PaginatedList<HierachyDepartmentIdentityDto> GetAllHdsByPath(string path, string departmentName, 
            int pageIndex, int pageSize, string orderBy, 
            List<string> jsonDynamicData = null, bool getDetailDepartment = true,      
            List<EntityStatusEnum> departmentEntityStatuses = null,
            List<int> departmentTypeIds = null,
            bool includeDepartmentType = false,
            bool getParentDepartmentId = false);
        Task<List<HierachyDepartmentIdentityDto>> GetHierarchyDepartmentIdentitiesAsync(int hierarchyId, int departmentId,
            bool includeParentHDs = true, bool includeChildrenHDs = false,
           int ownerId = 0, List<int> customerIds = null, List<EntityStatusEnum> departmentEntityStatuses = null,
           int? maxChildrenLevel = null, bool countChildren = false, List<int> departmentTypeIds = null,
           string departmentName = null,
           bool includeDepartmentType = false, bool getParentNode = false,
           bool countUser = false, List<EntityStatusEnum> countUserEntityStatuses = null,
           List<string> jsonDynamicData = null,
           bool checkPermission = false);
        Task<PaginatedList<HierachyDepartmentIdentityDto>> GetAllHdsByPathAsync(string path, string departmentName,
           int pageIndex, int pageSize, string orderBy,
           List<string> jsonDynamicData = null,
           bool getDetailDepartment = true,
           List<EntityStatusEnum> departmentEntityStatuses = null,
           List<int> departmentTypeIds = null,
           bool includeDepartmentType = false,
           bool getParentDepartmentId = false,
           List<int> departmentIds = null);
        Task<HierarchyDepartmentEntity> GetHierachyDepartmentAsync(int hierarchyId, int departmentId);
    }
}
