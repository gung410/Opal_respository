using System.Collections.Generic;
using cxPlatform.Client.ConexusBase;
using cxPlatform.Core;
using cxOrganization.Domain.Entities;
using System.Threading.Tasks;
using cxOrganization.Domain.Dtos.Departments;

namespace cxOrganization.Domain.Repositories
{
    /// <summary>
    /// Interface IHierarchyDepartmentRepository
    /// </summary>
    public interface IHierarchyDepartmentRepository : IRepository<HierarchyDepartmentEntity>
    {
        Task<HierarchyDepartmentEntity> GetHierachyDepartmentAsync(int hierarchyId, int departmentId);

        /// <summary>
        /// Gets the hierachy department by hierachy identifier and department identifier.
        /// </summary>
        /// <param name="hierarchyId">The hierarchy identifier.</param>
        /// <param name="departmentId">The department identifier.</param>
        /// <returns>H_D.</returns>
        HierarchyDepartmentEntity GetHierachyDepartmentByHierachyIdAndDepartmentId(int hierarchyId, int departmentId, int ownerId = 0, List<int> customerIds = null, List<EntityStatusEnum> departmentEntityStatuses = null, List<int> departmentTypeIds = null);
        HierarchyDepartmentEntity GetHierachyDepartmentByHierachyIdAndHdId(int hierarchyId, int hdId, int ownerId = 0, List<int> customerIds = null, List<EntityStatusEnum> departmentEntityStatuses = null, List<int> departmentTypeIds = null, bool includeDepartmentType = false);
        /// <summary>
        /// Gets the hierachy department.
        /// </summary>
        /// <param name="hierarchyId">The hierarchy identifier.</param>
        /// <param name="departmentId">The department identifier.</param>
        /// <returns>H_D.</returns>
        HierarchyDepartmentEntity GetHierachyDepartment(int hierarchyId, int departmentId);

        /// <summary>
        /// Gets the hd by hierarchy identifier and department identifier.
        /// </summary>
        /// <param name="hierarchyId">The hierarchy identifier.</param>
        /// <param name="departmentId">The department identifier.</param>
        /// <param name="includeInActiveStatus"></param>
        /// <returns>H_D.</returns>
        HierarchyDepartmentEntity GetHdByHierarchyIdAndDepartmentId(int hierarchyId, int departmentId, bool includeInActiveStatus = false);
        Task<HierarchyDepartmentEntity> GetHdByHierarchyIdAndDepartmentIdAsync(int hierarchyId, int departmentId, bool includeInActiveStatus = false);

        /// <summary>
        /// Gets the child HDS.
        /// </summary>
        /// <param name="parentPath">The parent path.</param>
        /// <param name="includeDepartment">if set to <c>true</c> [include department].</param>
        /// <param name="includeInActiveStatus"></param>
        /// <param name="departmentTypeId"></param>
        /// <param name="departmentIds"></param>
        /// <returns>IList{H_D}.</returns>
        IList<HierarchyDepartmentEntity> GetChildHds(string parentPath, bool includeDepartment = true, bool includeInActiveStatus = false, List<int> departmentTypeIds = null, List<int> departmentIds = null,
            int ownerId = 0, List<int> customerIds = null, List<EntityStatusEnum> departmentEntityStatuses = null, int? maxLevel = null, 
            bool includeChildren = false, string departmentName = null, bool includeDepartmentType = false, List<string> jsonDynamicData = null);

        /// <summary>
        /// Gets the child HDS.
        /// </summary>
        /// <param name="parentPath">The parent path.</param>
        /// <param name="includeDepartment">if set to <c>true</c> [include department].</param>
        /// <param name="includeInActiveStatus"></param>
        /// <param name="departmentTypeIds"></param>
        /// <param name="ownerId"></param>
        /// <param name="customerId"></param>
        /// <returns>IList{H_D}.</returns>
        IList<HierarchyDepartmentEntity> GetChildHds(string parentPath, List<int> departmentTypeIds, bool includeDepartment = true,
            bool includeInActiveStatus = false, int ownerId = 0, int customerId = 0);

        /// <summary>
        /// Gets the children h ds by hdid.
        /// </summary>
        /// <param name="hDId">The h d identifier.</param>
        /// <returns>List{H_D}.</returns>
        List<HierarchyDepartmentEntity> GetChildrenHDsByHDID(int hDId, int childrenDepartmentArchetype = 0);

        /// <summary>
        /// Get department by department type and current Hd
        /// </summary>
        /// <param name="departmentTypeIds">The department type ids</param>
        /// <param name="hdId">The HD id</param>
        /// <returns></returns>
        List<DepartmentEntity> GetDepartmentByDepartmentTypeAndHD(List<int> departmentTypeIds, int hdId);

        /// <summary>
        /// Get all department Ids from a hierachy department to below
        /// </summary>
        /// <param name="hierarchyDepartmentId">The hierarchy department id.</param>
        /// <returns>List of int H_D.</returns>
        List<int> GetAllDepartmentIdsFromAHierachyDepartmentToBelow(int hierarchyDepartmentId, bool getAllStatus = false);
        /// <summary>
        /// Get all department Ids from a hierachy department to the top
        /// </summary>
        /// <param name="hierarchyDepartmentId">The hierarchy department id.</param>
        /// <param name="getAllStatus">Get all status or not.</param>
        /// <returns>List of int H_D.</returns>
        List<int> GetAllDepartmentIdsFromAHierachyDepartmentToTheTop(int hierarchyDepartmentId, bool getAllStatus = false);

        Task<List<int>> GetAllDepartmentIdsFromAHierachyDepartmentToTheTopAsync(int hierarchyDepartmentId,
            bool getAllStatus = false);

        /// <summary>
        /// Get all hdIds from a hierachy department to below
        /// </summary>
        /// <param name="hierarchyDepartmentId">The hierarchy department id.</param>
        /// <param name="maxLevel">Maximum hd level should be get. if value maxLevel >0, we will get hd to that maxLevel, otherwise, we will get all level. Min value is 1</param>    
        /// <returns>List of int H_D.</returns>
        List<int> GetAllHDIdsFromAHierachyDepartmentToBelow(int hierarchyDepartmentId, bool getAllStatus = false, int? maxLevel = null);

        /// <summary>
        /// Get all hdIds from a hierachy department to below
        /// </summary>
        /// <param name="hierarchyDepartmentId">The hierarchy department id.</param>
        /// <param name="maxLevel">Maximum hd level should be get. if value maxLevel >0, we will get hd to that maxLevel, otherwise, we will get all level. Min value is 1</param>    
        /// <returns>List of int H_D.</returns>
        List<int> GetAllHDIdsFromAHierachyDepartmentToBelowWithDepartmentName(int hierarchyDepartmentId, bool getAllStatus = false, int? maxLevel = null, string departmentName = null);

        /// <summary>
        /// Get list HD by a list department
        /// </summary>
        /// <param name="hierarchyId">The hierarchy id.</param>
        /// <param name="departmentIds">The list department id.</param>
        /// <returns>H_D.</returns>
        List<HierarchyDepartmentEntity> GetListHierarchyDepartmentEntity(int hierarchyId, params int[] departmentIds);

        /// <summary>
        /// Get list HDs by a list department
        /// </summary>
        /// <param name="hdids"></param>
        /// <param name="includeDepartment"></param>
        /// <param name="includeParent"></param>
        /// <returns>H_D.</returns>
        Task<List<HierarchyDepartmentEntity>> GetListHierarchyDepartmentEntityAsync(List<int> hdids,
            bool includeDepartment, bool includeParent);

        /// <summary>
        /// Get list HD by a list hdid
        /// </summary>
        /// <param name="hierarchyId">The hierarchy id.</param>
        /// <param name="departmentIds">The list department id.</param>
        /// <returns>H_D.</returns>
        List<HierarchyDepartmentEntity> GetListHierarchyDepartmentEntity(int hierarchyId, List<int> hdId);

        /// <summary>
        /// Get all department Ids from a hierachy department to below
        /// </summary>
        /// <param name="hierarchyId"></param>
        /// <param name="departmentId">The hierarchy department id.</param>
        /// <param name="archetype"></param>
        /// <returns>List of int H_D.</returns>
        List<int> GetAllDepartmentIdsFromAHierachyDepartmentToBelow(int hierarchyId, int departmentId, ArchetypeEnum archetype);

        /// <summary>
        /// Get all department Ids from a hierachy department to below
        /// </summary>
        /// <param name="hierarchyId"></param>
        /// <param name="departmentId">The hierarchy department id.</param>
        /// <param name="archetype"></param>
        /// <returns>List of int H_D.</returns>
        List<int> GetAllDepartmentIdsFromAHierachyDepartmentToBelow(int hierarchyId, List<int> parentDepartmentIds, bool getAllStatus = false);

        Task<List<int>> GetAllDepartmentIdsFromAHierachyDepartmentToBelowAsync(int hierarchyId,
            List<int> parentDepartmentIds, bool getAllStatus = false);
        /// <summary>
        /// Get all department Ids from a hierachy department to top
        /// </summary>
        /// <param name="hierarchyId"></param>
        /// <param name="departmentId">The hierarchy department id.</param>
        /// <param name="archetype"></param>
        /// <returns>List of int H_D.</returns>
        List<int> GetAllDepartmentIdsFromAHierachyDepartmentToTheTop(int hierarchyId, List<int> departmentIds,
            bool getAllStatus = false);
        Task<List<int>> GetAllDepartmentIdsFromAHierachyDepartmentToTheTopAsync(int hierarchyId, List<int> departmentIds,
            bool getAllStatus = false);

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
        List<HierarchyDepartmentEntity> GetParentHDs(HierarchyDepartmentEntity hD, int ownerId = 0, List<int> customerIds = null, List<EntityStatusEnum> departmentEntityStatues = null, bool includeChildren = false, List<int> departmentTypeIds = null, bool includeDepartmentType = false);
        PaginatedList<HierarchyDepartmentEntity> GetHierarchyDepartments(string path, string departmentName,
            int pageIndex, int pageSize, string orderBy,
            List<string> jsonDynamicData = null,
            List<EntityStatusEnum> departmentEntityStatuses = null,
            List<int> departmentTypeIds = null,
            bool includeDepartmentType = false,
            bool includeParent = false);
        Task<List<HierarchyDepartmentEntity>> GetParentHDsAsync(HierarchyDepartmentEntity hD, int ownerId = 0,
            List<int> customerIds = null, List<EntityStatusEnum> departmentEntityStatues = null,
            bool includeChildren = false, List<int> departmentTypeIds = null,
            bool includeDepartmentType = false);
        Task<HierarchyDepartmentEntity> GetHierachyDepartmentByHierachyIdAndDepartmentIdAsync(int hierarchyId, int departmentId, int ownerId = 0, List<int> customerIds = null, List<EntityStatusEnum> departmentEntityStatuses = null, List<int> departmentTypeIds = null);
        Task<HierarchyDepartmentEntity> GetHierachyDepartmentByHierachyIdAndHdIdAsync(int hierarchyId, int hdId, int ownerId = 0, List<int> customerIds = null, List<EntityStatusEnum> departmentEntityStatuses = null, List<int> departmentTypeIds = null, bool includeDepartmentType = false);
        Task<List<HierarchyDepartmentEntity>> GetListHierarchyDepartmentEntityAsync(int hierarchyId, List<int> hdId);
        Task<IList<HierarchyDepartmentEntity>> GetChildHdsAsync(string parentPath,
            bool includeDepartment = true,
            bool includeInActiveStatus = false,
            List<int> departmentTypeIds = null,
            List<int> departmentIds = null,
            int ownerId = 0, List<int> customerIds = null,
            List<EntityStatusEnum> departmentEntityStatuses = null,
            int? maxLevel = null,
            bool includeChildren = false,
            string departmentName = null,
            bool includeDepartmentType = false,
            List<string> jsonDynamicData = null);
        Task<PaginatedList<HierarchyDepartmentEntity>> GetHierarchyDepartmentsAsync(string path, string departmentName,
            int pageIndex, int pageSize, string orderBy,
            List<string> jsonDynamicData = null,
            List<EntityStatusEnum> departmentEntityStatuses = null,
            List<int> departmentTypeIds = null,
            bool includeDepartmentType = false,
            bool includeParent = false,
            List<int> departmentIds = null);

        HierarchyInfo GetHierarchyInfo(int currentHdId, int departmentId,
            HierarchyDepartmentEntity hierarchyDepartmentEntity, bool getDepartmentPath = false);

        List<HierarchyInfo> GetHierarchyInfos(int currentHdId, List<int> departmentIds, bool getDepartmentPath = false, int? hierarchyId = null);

        Task<List<HierarchyInfo>> GetHierarchyInfosAsync(int currentHdId, List<int> departmentIds,
            bool getDepartmentPath = false, int? hierarchyId = null);

        Task<List<HierarchyInfo>> GetAllHierarchyInfoFromAHierachyDepartmentToTheTopAsync(int hierarchyId,
            List<int> departmentIds, bool getAllStatus = false, bool getDepartmentPath = false);
        List<HierarchyInfo> GetAllHierarchyInfoFromAHierachyDepartmentToTheTop(int hierarchyId,
            List<int> departmentIds, bool getAllStatus = false, bool getDepartmentPath = false);

        List<HierarchyInfo> GetAllHierarchyInfoFromAHierachyDepartmentToBelow(int hierarchyId,
            List<int> parentDepartmentIds, bool getAllStatus = false, bool getDepartmentPath = false);

  
        Task<List<HierarchyInfo>> GetAllHierarchyInfoFromAHierachyDepartmentToBelowAsync(int hierarchyId,
            List<int> parentDepartmentIds, bool getAllStatus = false, bool getDepartmentPath = false);

    }

}
