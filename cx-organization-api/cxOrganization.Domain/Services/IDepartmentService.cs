using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using cxOrganization.Client;
using cxOrganization.Client.Departments;
using cxOrganization.Domain.Entities;
using cxOrganization.Domain.Validators;
using cxPlatform.Client.ConexusBase;

namespace cxOrganization.Domain.Services
{
    public interface IDepartmentService
    {
        /// <summary>
        /// Get department by ext id
        /// </summary>
        /// <param name="extId"></param>
        /// <param name="customerId"></param>
        /// <returns></returns>
        ConexusBaseDto GetDepartment(string extId, int customerId);
        EntityStatusDto UpdateDepartmentStatus(HierarchyDepartmentValidationSpecification validationSpecification, int departmentId, EntityStatusDto departmentStatus);
        List<IdentityStatusDto> UpdateDepartmentIdentifiers(List<IdentityStatusDto> departmentidentities, List<int> allowArchetypeIds, string hdPath);
        List<HierachyDepartmentIdentityDto> GetDepartmentHierachyDepartmentIdentities(int departmentId, bool includeParentHDs = true, bool includeChildrenHDs = false, 
            int ownerId = 0, List<int> customerIds = null, List < EntityStatusEnum > departmentEntityStatuses = null, 
            int? maxChildrenLevel = null, bool countChildren= false, List<int> departmentTypeIds = null, 
            string departmentName = null, bool includeDepartmentType = false, bool getParentNode = false, 
            bool countUser = false, List<EntityStatusEnum> countUserEntityStatuses = null, List<string> jsonDynamicData = null,
            bool checkPermission = false);
        Task<List<HierachyDepartmentIdentityDto>> GetDepartmentHierachyDepartmentIdentitiesAsync(string departmentExtId, bool includeParentHDs = true, bool includeChildrenHDs = false,
            int ownerId = 0, List<int> customerIds = null, List<EntityStatusEnum> departmentEntityStatuses = null, int? maxChildrenLevel = null, bool countChildren = false,
           List<int> departmentTypeIds = null, string departmentName = null, bool includeDepartmentType = false,
           bool countUser = false, List<EntityStatusEnum> countUserEntityStatuses = null);
        List<HierachyDepartmentIdentityDto> GetDepartmentHierachyDepartmentIdentities(string departmentExtId, bool includeParentHDs = true, bool includeChildrenHDs = false, int ownerId = 0, List<int> customerIds = null, List<EntityStatusEnum> departmentEntityStatuses = null, int? maxChildrenLevel = null, bool countChildren= false, List<int> departmentTypeIds = null, string departmentName = null, bool includeDepartmentType = false, bool countUser = false, List<EntityStatusEnum> countUserEntityStatuses = null);
        List<ConexusBaseDto> GetDepartments(HierarchyDepartmentValidationSpecification validationSpecification, int parentId);
        ConexusBaseDto UpdateDepartment(HierarchyDepartmentValidationSpecification validationSpecification, DepartmentDtoBase departmentDto);
        ConexusBaseDto InsertDepartment(HierarchyDepartmentValidationSpecification validationSpecification, DepartmentDtoBase departmentDto);
        ConexusBaseDto GetDepartment(HierarchyDepartmentValidationSpecification validationSpecification, int departmentId);
        ConexusBaseDto GetDepartment(int departmentId);

        PaginatedList<T> GetDepartments<T>(int ownerId = 0,
            List<int> userIds = null,
            List<int> userGroupIds = null,
            List<int> customerIds = null,
            List<int> departmentIds = null,
            List<EntityStatusEnum> statusIds = null,
            List<int> archetypeIds = null,
            int parentDepartmentId = 0,
            int childDepartmentId = 0,
            List<string> departmentTypeExtIds = null,
            List<string> extIds = null,
            DateTime? lastUpdatedBefore = null,
            DateTime? lastUpdatedAfter = null,
            bool? getDynamicProperties = null,
            int pageIndex = 0,
            int pageSize = 0,
            string orderBy = "") where T : ConexusBaseDto;

        Task<PaginatedList<T>> GetDepartmentsAsync<T>(int ownerId = 0,
            List<int> userIds = null,
            List<int> userGroupIds = null,
            List<int> customerIds = null,
            List<int> departmentIds = null,
            List<EntityStatusEnum> statusIds = null,
            List<int> archetypeIds = null,
            int parentDepartmentId = 0,
            List<int> parentDepartmentIds = null,
            string parentDepartmentExtId = null,
            int childDepartmentId = 0,
            List<string> departmentTypeExtIds = null,
            List<string> extIds = null,
            DateTime? lastUpdatedBefore = null,
            DateTime? lastUpdatedAfter = null,
            bool? externallyMastered = null,
            bool? getDynamicProperties = null,
            bool? includeDepartmentType = true,
            string searchText = "",
            int pageIndex = 0,
            int pageSize = 0,
            string orderBy = "") where T : ConexusBaseDto;
        IdentityStatusDto GetDepartmentIdentityStatusByExtId(string extId, int customerId);
        List<IdentityStatusDto> GetListDepartmentIdentityStatusByExtId(string extId);
        IdentityStatusDto GetDepartmentIdentityStatusById(int departmentId);
        List<ConexusBaseDto> GetParentDepartments(int departmentId);
        List<LevelDto> GetLevels(List<ArchetypeEnum> archytypeIds = null, List<int> departmentIds = null, List<int> departmentTypeIds = null);
        MemberDto AddUser(int departmentId, MemberDto user);
        MemberDto RemoveUser(int departmentId, MemberDto user);
        DepartmentEntity GetDepartmentById(int Id);
        DepartmentEntity GetDepartment(int departmentId, int ownerId, bool includeInActiveStatus = false);
        DepartmentEntity GetDepartmentByExtIdIncludeHd(string extId, int ownerId, bool includeInActiveStatus = false);
        DepartmentEntity GetDepartmentByExtId(string departmentExtId, string customerExtId, bool includeInActiveStatus = false);
        List<DepartmentDto> GetListChildDepartmentByDepartmentId(int departmentId);
        DepartmentEntity UpdateDepartmentType(DepartmentEntity department, DepartmentTypeEntity addingDepartmentType, bool isUniqueDepartmentType = false);
        List<IdentityStatusDto> UpdateDepartmentLastSyncDate(List<IdentityStatusDto> departments);
        DepartmentEntity UpdateDepartmentStatus(DepartmentEntity departmentEntity, EntityStatusEnum newEntityStatus, EntityStatusReasonEnum newEntitySatusReason, int? updatedById);
        DepartmentEntity UpdateDepartmentStatus(int departmentId, EntityStatusEnum newEntityStatus, EntityStatusReasonEnum newEntitySatusReason, int? updatedById);
        void UpdateObjectMappingsEmployee(List<string> classExtIds, long employeeId, int customerId);

        List<DepartmentEntity> GetDepartmentByNames(List<string> departmentNames);
        DepartmentEntity GetDepartmentByIdIncludeHd(int departmentId, int ownerId, int customerId);
        Task<List<HierachyDepartmentIdentityDto>> GetDepartmentHierachyDepartmentIdentitiesAsync(int departmentId, bool includeParentHDs = true, bool includeChildrenHDs = false,
            int ownerId = 0, List<int> customerIds = null, List<EntityStatusEnum> departmentEntityStatuses = null, int? maxChildrenLevel = null, bool countChildren = false,
            List<int> departmentTypeIds = null, string departmentName = null, bool includeDepartmentType = false, bool getParentNode = false,
            bool countUser = false, List<EntityStatusEnum> countUserEntityStatuses = null, List<string> jsonDynamicData = null,
            bool checkPermission = false);

    }
}
