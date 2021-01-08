using System.Collections.Generic;
using cxOrganization.Domain.Entities;

namespace cxOrganization.Domain.Security.AccessServices
{
    public class UserAccessResult
    {
        public static UserAccessResult CreateAccessDeniedResult(UserEntity executor)
        {
            return new UserAccessResult(AccessStatus.AccessDenied, executor: executor);
        }
        public static UserAccessResult CreateDataNotFoundResult(UserEntity executor)
        {
            return new UserAccessResult(AccessStatus.DataNotFound, executor: executor);
        }
        public static UserAccessResult CreateAccessGrantedResult(List<int> userIds, List<int> userGroupIds,
            List<int> parentDepartmentIds, List<List<int>> multiUserGroupFilters, List<List<int>> multiUserTypeFilters,
            UserEntity executor)
        {
            return new UserAccessResult(AccessStatus.AccessGranted,
                userIds: userIds,
                userGroupIds: userGroupIds,
                parentDepartmentIds: parentDepartmentIds,
                multiUserGroupFilters: multiUserGroupFilters,
                multiUserTypeFilters: multiUserTypeFilters,
                executor: executor);
        }

        public static UserAccessResult CreateAccessGrantedResult( UserEntity executor)
        {
            return new UserAccessResult(AccessStatus.AccessGranted, executor);
        }
        public UserAccessResult(AccessStatus accessStatus, List<int> userIds, List<int> userGroupIds,
            List<int> parentDepartmentIds, List<List<int>> multiUserGroupFilters, List<List<int>> multiUserTypeFilters, UserEntity executor)
        {
            AccessStatus = accessStatus;
            UserIds = userIds;
            UserGroupIds = userGroupIds;
            ParentDepartmentIds = parentDepartmentIds;
            MultiUserGroupFilters = multiUserGroupFilters;
            MultiUserTypeFilters = multiUserTypeFilters;
            ExecutorUser = executor;
        }

        public UserAccessResult(AccessStatus accessStatus, UserEntity executor)
        {
            ExecutorUser = executor;
            AccessStatus = accessStatus;
        }

        public AccessStatus AccessStatus { get; set; }
        public List<int> UserIds { get; set; }
        public List<int> UserGroupIds { get; set; }
        public List<int> ParentDepartmentIds { get; set; }
        public List<List<int>> MultiUserGroupFilters { get; set; }
        public List<List<int>> MultiUserTypeFilters { get; set; }

        public UserEntity ExecutorUser { get; set; }

        public bool IsAllowedAccess => AccessStatus.IsAllowedAccess();
    }
}