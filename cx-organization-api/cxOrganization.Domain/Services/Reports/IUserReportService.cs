using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using cxOrganization.Domain.AdvancedWorkContext;
using cxPlatform.Client.ConexusBase;
using cxPlatform.Core;

namespace cxOrganization.Domain.Services.Reports
{
    public interface IUserReportService
    {
        Task<PaginatedList<UserEventLogInfo>> GetPaginatedUserEventLogInfosAsync(IAdvancedWorkContext workContext, List<UserEventType> eventTypes, DateTime? eventCreatedAfter,
            DateTime? eventCreatedBefore, int pageSize, int pageIndex, bool getDepartment, bool getRole);

        Task<List<UserEventLogInfo>> GetUserEventLogInfosAsync(IAdvancedWorkContext workContext, List<UserEventType> eventTypes,
            DateTime? eventCreatedAfter, DateTime? eventCreatedBefore, bool getDepartment, bool getRole);

        Task<UserStatisticsDto> GetUserStatisticsAsync(IAdvancedWorkContext workContext,
            List<EntityStatusEnum> accountStatisticsEntityStatuses,
            List<UserEventType> eventStatisticsTypes,
            bool getOnBoardingStatistics,
            DateTime? fromDate, DateTime? toDate);

        Task<int> CountUserEventAsync(IAdvancedWorkContext workContext,
            List<UserEventType> eventTypes,
            DateTime? eventCreatedAfter,
            DateTime? eventCreatedBefore);

        Task<List<ApprovingOfficerInfo>> GetApprovingOfficerInfosAsync(
            IAdvancedWorkContext workContext,
            List<int> parentDepartmentIds,
            bool filterOnSubDepartment,
            bool getRole, bool getDepartment,
            DateTime? userCreatedAfter,
            DateTime? userCreatedBefore,
            DateTime? countMemberCreatedAfter,
            DateTime? countMemberCreatedBefore,
             List<EntityStatusEnum> userEntityStatuses);

        Task<PaginatedList<ApprovingOfficerInfo>> GetPaginatedApprovingOfficerInfosAsync(
            IAdvancedWorkContext workContext,
            List<int> parentDepartmentIds,
            bool filterOnSubDepartment,
            bool getRole, bool getDepartment,
            DateTime? userCreatedAfter,
            DateTime? userCreatedBefore,
            DateTime? countMemberCreatedAfter,
            DateTime? countMemberCreatedBefore,
            List<EntityStatusEnum> userEntityStatuses,
            int pageSize,
            int pageIndex);

        Task<PaginatedList<UserAccountDetailsInfo>> GetPaginatedUserAccountDetailsInfosAsync(
            IAdvancedWorkContext workContext,
            List<int> parentDepartmentIds,
            bool? filterOnSubDepartment = null,
            List<EntityStatusEnum> userEntityStatuses = null,
            DateTime? createdAfter = null,
            DateTime? createdBefore = null,
            DateTime? lastUpdatedAfter = null,
            DateTime? lastUpdatedBefore = null,
            DateTime? expirationDateAfter = null,
            DateTime? expirationDateBefore = null,
            DateTime? entityActiveDateAfter = null,
            DateTime? entityActiveDateBefore = null,
            DateTime? firstLoginAfter = null,
            DateTime? firstLoginBefore = null,
            DateTime? lastLoginAfter = null,
            DateTime? lastLoginBefore = null,
            DateTime? onboardingAfter = null,
            DateTime? onboardingBefore = null,
            int pageSize = 0,
            int pageIndex = 0, 
            string orderBy = null);

        Task<List<UserAccountDetailsInfo>> GetUserAccountDetailsInfosAsync(
            IAdvancedWorkContext workContext,
            List<int> parentDepartmentIds,
            bool? filterOnSubDepartment = null,
            List<EntityStatusEnum> userEntityStatuses = null,
            DateTime? createdAfter = null,
            DateTime? createdBefore = null,
            DateTime? lastUpdatedAfter = null,
            DateTime? lastUpdatedBefore = null,
            DateTime? expirationDateAfter = null,
            DateTime? expirationDateBefore = null,
            DateTime? entityActiveDateAfter = null,
            DateTime? entityActiveDateBefore = null,
            DateTime? firstLoginAfter = null,
            DateTime? firstLoginBefore = null,
            DateTime? lastLoginAfter = null,
            DateTime? lastLoginBefore = null,
            DateTime? onboardingAfter = null,
            DateTime? onboardingBefore = null,
            string orderBy = null);

        Task<List<PrivilegedUserAccountInfo>> GetPrivilegedUserAccountInfosAsync(
            IAdvancedWorkContext workContext,
            List<int> parentDepartmentIds,
            bool? filterOnSubDepartment = null,
            List<EntityStatusEnum> userEntityStatuses = null,
            DateTime? createdAfter = null,
            DateTime? createdBefore = null,
            DateTime? lastUpdatedAfter = null,
            DateTime? lastUpdatedBefore = null,
            DateTime? expirationDateAfter = null,
            DateTime? expirationDateBefore = null,
            DateTime? entityActiveDateAfter = null,
            DateTime? entityActiveDateBefore = null,
            DateTime? firstLoginAfter = null,
            DateTime? firstLoginBefore = null,
            DateTime? lastLoginAfter = null,
            DateTime? lastLoginBefore = null,
            DateTime? onboardingAfter = null,
            DateTime? onboardingBefore = null,
            string orderBy = null,
            bool? needDepartmentPathName = null);

        Task<PaginatedList<PrivilegedUserAccountInfo>> GetPaginatedPrivilegedUserAccountInfosAsync(
            IAdvancedWorkContext workContext,
            List<int> parentDepartmentIds,
            bool? filterOnSubDepartment = null,
            List<EntityStatusEnum> userEntityStatuses = null,
            DateTime? createdAfter = null,
            DateTime? createdBefore = null,
            DateTime? lastUpdatedAfter = null,
            DateTime? lastUpdatedBefore = null,
            DateTime? expirationDateAfter = null,
            DateTime? expirationDateBefore = null,
            DateTime? entityActiveDateAfter = null,
            DateTime? entityActiveDateBefore = null,
            DateTime? firstLoginAfter = null,
            DateTime? firstLoginBefore = null,
            DateTime? lastLoginAfter = null,
            DateTime? lastLoginBefore = null,
            DateTime? onboardingAfter = null,
            DateTime? onboardingBefore = null,
            int pageSize = 0,
            int pageIndex = 0, 
            string orderBy = null,
            bool? needDepartmentPathName = null);
    }
}