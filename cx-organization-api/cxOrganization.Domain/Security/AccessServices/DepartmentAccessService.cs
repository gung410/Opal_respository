using cxOrganization.Client.Departments;
using cxOrganization.Domain.Entities;
using cxOrganization.Domain.Mappings;
using cxOrganization.Domain.Repositories;
using cxOrganization.Domain.Settings;
using cxPlatform.Core;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using cxOrganization.Domain.Dtos.Departments;
using cxOrganization.Domain.Services;

namespace cxOrganization.Domain.Security.AccessServices
{
    public class DepartmentAccessService : AccessServiceBase, IDepartmentAccessService
    {
        private readonly AccessSettings _accessSettings;
        private readonly IUserRepository _userRepository;
        private readonly IHierarchyDepartmentMappingService _hierarchyDepartmentMappingService;
        private readonly IHierarchyDepartmentService _hierarchyDepartmentService;
        public DepartmentAccessService(ILogger<DepartmentAccessService> logger,
            IOptions<AccessSettings> accessSettingsOption,
            IUserRepository userRepository,
            IHierarchyDepartmentRepository hierarchyDepartmentRepository,
            IHierarchyDepartmentMappingService hierarchyDepartmentMappingService,
            IDTDEntityRepository dtdEntityRepository,
            IDepartmentTypeRepository departmentTypeRepository,
            IHierarchyDepartmentService hierarchyDepartmentService) : base(logger, hierarchyDepartmentRepository, dtdEntityRepository, departmentTypeRepository)
        {
            _userRepository = userRepository;
            _accessSettings = accessSettingsOption.Value;
            _hierarchyDepartmentMappingService = hierarchyDepartmentMappingService;
            _hierarchyDepartmentService = hierarchyDepartmentService;
        }

        /// <summary>
        /// Gets the top hierarchy department which the currently logged-in user has access to.
        /// e.g: DLC could be in Branch level but he should be allowed to access his closet Division.
        /// </summary>
        /// <param name="workContext"></param>
        /// <returns></returns>
        public async Task<(HierachyDepartmentIdentityDto TopHierachyDepartmentIdentity, List<HierarchyInfo> AccessibleHierarchyInfos)> GetTopHierarchyDepartmentsByWorkContext(IWorkContext workContext)
        {
            /*Please don't blame me because of these stuff, 
             * the complete requirements is not defined for the current release yet. THANKS!*/
            var isAuthenticatedByToken = !string.IsNullOrEmpty(workContext.Sub);
            if (!isAuthenticatedByToken)
            {
                return (null, null);
            }

            var executorUser = workContext.CurrentUser as UserEntity;
            if (executorUser == null || executorUser.UT_Us == null || executorUser.UT_Us.Count == 0)
            {
                executorUser = await _userRepository.GetOrSetUserFromWorkContext(workContext);
            }

            var adminRoleExtIds = new List<string> {
                "overallsystemadministrator",
                "useraccountadministrator",
                "divisionadmin",
                "branchadmin",
                "schooladmin",
                "divisiontrainingcoordinator",
                "schooltrainingcoordinator",
                "approvingofficer"};

            // Handle for custom roles
            bool onlyHasAccessToCurrentDepartment = false;

            var currentUserRoles = await _userRepository.GetOrSetUserRoleFromWorkContext(workContext);
            var currentAdminRoleExtIds = currentUserRoles.Select(currentUserRole => currentUserRole.ExtId);
            onlyHasAccessToCurrentDepartment = adminRoleExtIds
               .TrueForAll(adminRoleExtId
               => !currentAdminRoleExtIds.Contains(adminRoleExtId));
            
            HierachyDepartmentIdentityDto topHierachyDepartmentIdentity = null;
            
            if (onlyHasAccessToCurrentDepartment)
            {
                var currentUserHD = _hierarchyDepartmentService.GetH_DByDepartmentID(executorUser.DepartmentId);
                topHierachyDepartmentIdentity = await GetTopHierachyDepartmentByHdId(currentUserHD.HDId);

                var allCurrentToTopHierarchyInfos = await _hierarchyDepartmentRepository.GetAllHierarchyInfoFromAHierachyDepartmentToTheTopAsync(
                    currentUserHD.HierarchyId, new List<int> { executorUser.DepartmentId });

                var currentHierarchyInfo = allCurrentToTopHierarchyInfos.FirstOrDefault(hd => hd.DepartmentId == executorUser.DepartmentId);

                return (topHierachyDepartmentIdentity, new List<HierarchyInfo>() { currentHierarchyInfo });
            }

            // Handle for default roles
            if (_accessSettings == null
                || _accessSettings.ReadUserAccess == null
                || _accessSettings.DisableReadUserAccessChecking)
            {
                return (null, null);
            }

            var executorRoles = await _userRepository.GetOrSetUserRoleFromWorkContext(workContext);

            // Users with both custom roles and admin roles will have the access based on the custom roles only.
            var currentAdminRoles = executorRoles.Where(executorRole => adminRoleExtIds.Contains(executorRole.ExtId)).ToList();
            var accessSetting = GetFinalAccessSettingOfRoles(currentAdminRoles, _accessSettings.ReadDepartmentAccess);

            var accessibleHierarchyInfos = await base.GetAccessibleHierarchyInfosAsync(
                workContext,
                executorUser.DepartmentId,
                accessSetting,
                null);

            var topHierarchyInfo = FindTopHierarchyInfo(accessibleHierarchyInfos);

            if (topHierarchyInfo != null)
            {
                topHierachyDepartmentIdentity = await GetTopHierachyDepartmentByHdId(topHierarchyInfo.HdId);
            }

            return (topHierachyDepartmentIdentity, accessibleHierarchyInfos);
        }

        private async Task<HierachyDepartmentIdentityDto> GetTopHierachyDepartmentByHdId(int HdId)
        {
            var topHierarchyDepartment = (await _hierarchyDepartmentRepository.GetListHierarchyDepartmentEntityAsync(
                new List<int> { HdId }, true, true)).FirstOrDefault();

            if (topHierarchyDepartment != null)
            {
                return _hierarchyDepartmentMappingService.ToDto(topHierarchyDepartment, topHierarchyDepartment.Parent);
            }

            return null;
        }

    }
}
