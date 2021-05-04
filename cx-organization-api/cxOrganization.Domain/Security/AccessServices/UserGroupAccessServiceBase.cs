using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using cxOrganization.Domain.AdvancedWorkContext;
using cxOrganization.Domain.Common;
using cxOrganization.Domain.Entities;
using cxOrganization.Domain.Extensions;
using cxOrganization.Domain.Repositories;
using cxOrganization.Domain.Settings;
using cxPlatform.Core;
using Microsoft.Extensions.Logging;

namespace cxOrganization.Domain.Security.AccessServices
{
    public abstract class UserGroupAccessServiceBase : AccessServiceBase
    {
        private readonly IUserRepository _userRepository;


        protected UserGroupAccessServiceBase(ILogger logger,
            IUserRepository userRepository,
            IHierarchyDepartmentRepository hierarchyDepartmentRepository, IDTDEntityRepository dtdEntityRepository,
            IDepartmentTypeRepository departmentTypeRepository) 
            : base(logger, hierarchyDepartmentRepository, dtdEntityRepository, departmentTypeRepository)
        {
            _userRepository = userRepository;
        }

      

        protected UserAccessResult CheckUserGroupAccessInternal(IAdvancedWorkContext workContext,
            Dictionary<string, AccessSettingElement> accessSettingGroup,
            List<int> userIds,
            List<int> parentDepartmentIds)
        {
           UserEntity executorUser = null;
            var isAuthenticatedByToken = !string.IsNullOrEmpty(workContext.Sub);
            if (isAuthenticatedByToken && accessSettingGroup != null)
            {
                executorUser = DomainHelper.GetUserEntityFromWorkContextSub(workContext, _userRepository, true);
                var executorUserId = executorUser.UserId;
                var isSelfAccess = IsSelfAccess(executorUser, workContext.Sub, null, userIds, null);

                if (isSelfAccess)
                {
                    return UserAccessResult.CreateAccessGrantedResult(userIds: userIds, userGroupIds: null,
                        parentDepartmentIds: parentDepartmentIds, multiUserGroupFilters: null,
                        multiUserTypeFilters: null,
                        executor: executorUser);
                }

                var executorRoles = MapToUserRoles(executorUser.UT_Us);
                var accessSetting = GetFinalAccessSettingOfRoles(executorRoles, accessSettingGroup);
                if (accessSetting == null) { return UserAccessResult.CreateAccessDeniedResult(executorUser); }

                if (accessSetting.HasFullAccessOnHierarchy)
                {
                    return UserAccessResult.CreateAccessGrantedResult(userIds: userIds, userGroupIds: null,
                        parentDepartmentIds: parentDepartmentIds, multiUserGroupFilters: null,
                        multiUserTypeFilters: null,
                        executor: executorUser);
                }

                var accessibleDepartmentIds =
                    GetAccessibleHierarchyInfos(workContext, executorUser.DepartmentId, accessSetting)
                        .Select(hd => hd.DepartmentId).ToList();

                if (!parentDepartmentIds.IsNullOrEmpty())
                {
                    parentDepartmentIds = parentDepartmentIds
                        .Where(departmentId => accessibleDepartmentIds.Contains(departmentId))
                        .ToList();
                    if (parentDepartmentIds.Count == 0)
                    {
                        return UserAccessResult.CreateAccessDeniedResult(executorUser);
                    }
                }
                else
                {
                    parentDepartmentIds = accessibleDepartmentIds;
                }

                var hasAnyAccessOnDepartment = !parentDepartmentIds.IsNullOrEmpty();

                if (!hasAnyAccessOnDepartment)
                {
                    //If executor user does not have access on department and user group, he is only able to get himself.
                    if (userIds.IsNullOrEmpty())
                    {
                        userIds = new List<int> { executorUserId };
                    }
                    else
                    {
                        userIds = userIds
                            .Where(uid => uid == executorUserId)
                            .ToList();
                        if (userIds.Count == 0)
                            return UserAccessResult.CreateAccessDeniedResult(executorUser);;
                    }
                }
            }

            return UserAccessResult.CreateAccessGrantedResult(userIds: userIds, userGroupIds: null,
                parentDepartmentIds: parentDepartmentIds, multiUserGroupFilters: null, multiUserTypeFilters: null,
                executor: executorUser);

        }
        protected async Task<UserAccessResult> CheckUserGroupAccessInternalAsync(IAdvancedWorkContext workContext,
           Dictionary<string, AccessSettingElement> accessSettingGroup,
            List<int> userIds,
            List<int> parentDepartmentIds)
        {
            UserEntity executorUser = null;
            var isAuthenticatedByToken = !string.IsNullOrEmpty(workContext.Sub);
            if (isAuthenticatedByToken && accessSettingGroup != null)
            {
                executorUser = await _userRepository.GetOrSetUserFromWorkContext(workContext); 
                var executorUserId = executorUser.UserId;
                var isSelfAccess = IsSelfAccess(executorUser, workContext.Sub, null, userIds, null);

                if (isSelfAccess)
                {
                    return UserAccessResult.CreateAccessGrantedResult(userIds: userIds, userGroupIds: null,
                        parentDepartmentIds: parentDepartmentIds, multiUserGroupFilters: null,
                        multiUserTypeFilters: null, executor: executorUser);
                }

                var executorRoles = await _userRepository.GetOrSetUserRoleFromWorkContext(workContext);
                var accessSetting = GetFinalAccessSettingOfRoles(executorRoles, accessSettingGroup);
                if (accessSetting == null) { return UserAccessResult.CreateAccessDeniedResult(executorUser); }

                if (accessSetting.HasFullAccessOnHierarchy)
                {
                    return UserAccessResult.CreateAccessGrantedResult(userIds: userIds, userGroupIds: null,
                        parentDepartmentIds: parentDepartmentIds, multiUserGroupFilters: null,
                        multiUserTypeFilters: null, executor: executorUser);
                }

                var accessibleDepartmentIds =
                    (await GetAccessibleHierarchyInfosAsync(workContext, executorUser.DepartmentId, accessSetting))
                    .Select(hd => hd.DepartmentId).ToList();

                if (!parentDepartmentIds.IsNullOrEmpty())
                {
                    parentDepartmentIds = parentDepartmentIds
                        .Where(departmentId => accessibleDepartmentIds.Contains(departmentId))
                        .ToList();
                    if (parentDepartmentIds.Count == 0)
                    {
                        return  UserAccessResult.CreateAccessDeniedResult(executorUser);
                    }
                }
                else
                {
                    parentDepartmentIds = accessibleDepartmentIds;
                }

                var hasAnyAccessOnDepartment = !parentDepartmentIds.IsNullOrEmpty();

                if (!hasAnyAccessOnDepartment)
                {
                    //If executor user does not have access on department and user group, he is only able to get himself.
                    if (userIds.IsNullOrEmpty())
                    {
                        userIds = new List<int> { executorUserId };
                    }
                    else
                    {
                        userIds = userIds
                            .Where(uid => uid == executorUserId)
                            .ToList();
                        if (userIds.Count == 0)
                            return UserAccessResult.CreateAccessDeniedResult(executorUser); 
                    }
                }
            }

            return UserAccessResult.CreateAccessGrantedResult(userIds: userIds, userGroupIds: null,
                parentDepartmentIds: parentDepartmentIds, multiUserGroupFilters: null, multiUserTypeFilters: null,
                executor: executorUser);

        }
    }
}