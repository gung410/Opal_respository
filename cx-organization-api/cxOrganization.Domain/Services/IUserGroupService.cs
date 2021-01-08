using System;
using System.Collections.Generic;
using cxOrganization.Client.UserGroups;
using cxOrganization.Domain.Dtos.UserGroups;
using cxOrganization.Domain.Entities;
using cxOrganization.Domain.Validators;
using cxPlatform.Client.ConexusBase;

namespace cxOrganization.Domain.Services
{
    public interface IUserGroupService
    {
        ConexusBaseDto InsertUserGroup(HierarchyDepartmentValidationSpecification validationSpecification,
            UserGroupDtoBase usergroup);

        ConexusBaseDto UpdateUserGroup(HierarchyDepartmentValidationSpecification validationSpecification,
            UserGroupDtoBase usergroup);

        /// <summary>
        /// Get UserGroup
        /// </summary>
        /// <param name="userGroupId">The user group identifier.</param>
        /// <returns>UserGroup</returns>
        ConexusBaseDto GetUserGroup(HierarchyDepartmentValidationSpecification validationSpecification,
            int userGroupId);

        /// <summary>
        /// Get UserGroup
        /// </summary>
        /// <param name="userGroupId"></param>
        /// <returns></returns>
        ConexusBaseDto GetUserGroup(int userGroupId);

        /// <summary>
        /// Gets the user groups.
        /// </summary>
        /// <param name="departmentId">The Department Id.</param>
        /// <returns>List{UserGroupDtoBase}.</returns>
        List<ConexusBaseDto> GetUserGroups(HierarchyDepartmentValidationSpecification validationSpecification,
            int departmentId);

        void DeleteUserGroupById(HierarchyDepartmentValidationSpecification validationSpecification, int userGroupId);
        List<IdentityStatusDto> GetUserGroupIdentifiers(List<int> departmentIds);

        List<IdentityStatusDto> UpdateUserGroupIdentifiers(List<IdentityStatusDto> userGroupIdentities,
            List<int> allowArchetypeIds, string hdPath, params EntityStatusEnum[] filters);

        PaginatedList<T> GetUserGroups<T>(int ownerId = 0,
            List<int> customerIds = null,
            List<int> userGroupIds = null,
            List<int> memberUserIds = null,
            List<int> departmentIds = null,
            List<EntityStatusEnum> statusIds = null,
            List<EntityStatusEnum> userStatusIds = null,
            List<GrouptypeEnum> groupTypes = null,
            List<int> archetypeIds = null,
            List<string> extIds = null,
            List<int> groupUserIds = null,
            DateTime? lastUpdatedBefore = null,
            DateTime? lastUpdatedAfter = null,
            bool? getDynamicProperties = null,
            int pageIndex = 0,
            int pageSize = 0,
            string orderBy = "",
            string searchKey = null,
            bool includeDepartment = false,
            bool includeUser = false) where T : ConexusBaseDto;

        ConexusBaseDto GetUserGroupIdentityStatusByExtId(string extId, int customerId);
        List<ConexusBaseDto> GetListUserGroupIdentityStatusByExtId(string extId);

        List<UserGroupEntity> GetUserGroupByDepartmentId(int departmentId, int userGroupTypeId = 0,
            bool isIncludeUsers = true,
            int departmentTypeId = 0,
            params EntityStatusEnum[] filters);

        UserGroupEntity GetUserGroupByExtId(string extId);

        UserGroupEntity UpdateUserGroupDepartmentType(UserGroupEntity userGroup,
            DepartmentTypeEntity addingDepartmentType, bool isUniqueDepartmentType = false);

        List<TeachingSubjectDto> GetUserGroupsByArchetypes(List<ArchetypeEnum> archetypeIds);

        PaginatedList<UserGroupEntity> GetUserGroupEntities(int ownerId = 0,
            List<int> customerIds = null,
            List<int> userGroupIds = null,
            List<int> memberUserIds = null,
            List<int> departmentIds = null,
            List<EntityStatusEnum> statusIds = null,
            List<EntityStatusEnum> userStatusIds = null,
            List<GrouptypeEnum> groupTypes = null,
            List<int> archetypeIds = null,
            List<string> extIds = null,
            List<int> groupUserIds = null,
            DateTime? lastUpdatedBefore = null,
            DateTime? lastUpdatedAfter = null,
            bool? getDynamicProperties = null,
            int pageIndex = 0,
            int pageSize = 0,
            string orderBy = "",
            string searchKey = null,
            bool includeDepartment = false,
            bool includeUser = false);

        ConexusBaseDto MapToUserGroupService(UserGroupEntity userGroupEntity, bool? getDynamicProperties = null);
    }
}
