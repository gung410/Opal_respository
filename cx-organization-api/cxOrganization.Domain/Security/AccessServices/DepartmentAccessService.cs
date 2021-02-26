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

namespace cxOrganization.Domain.Security.AccessServices
{
    public class DepartmentAccessService : AccessServiceBase, IDepartmentAccessService
    {
        private readonly AccessSettings _accessSettings;
        private readonly IUserRepository _userRepository;
        private readonly IHierarchyDepartmentMappingService _hierarchyDepartmentMappingService;

        public DepartmentAccessService(ILogger<DepartmentAccessService> logger,
            IOptions<AccessSettings> accessSettingsOption,
            IUserRepository userRepository,
            IHierarchyDepartmentRepository hierarchyDepartmentRepository,
            IHierarchyDepartmentMappingService hierarchyDepartmentMappingService,
            IDTDEntityRepository dtdEntityRepository,
            IDepartmentTypeRepository departmentTypeRepository) : base(logger, hierarchyDepartmentRepository, dtdEntityRepository, departmentTypeRepository)
        {
            _userRepository = userRepository;
            _accessSettings = accessSettingsOption.Value;
            _hierarchyDepartmentMappingService = hierarchyDepartmentMappingService;
        }

        /// <summary>
        /// Gets the top hierarchy department which the currently logged-in user has access to.
        /// e.g: DLC could be in Branch level but he should be allowed to access his closet Division.
        /// </summary>
        /// <param name="workContext"></param>
        /// <returns></returns>
        public async Task<(HierachyDepartmentIdentityDto TopHierachyDepartmentIdentity, List<HierarchyInfo> AccessibleHierarchyInfos)> GetTopHierarchyDepartmentsByWorkContext(IWorkContext workContext)
        {
            var isAuthenticatedByToken = !string.IsNullOrEmpty(workContext.Sub);

            if (isAuthenticatedByToken
                && _accessSettings != null
                && !_accessSettings.DisableReadUserAccessChecking
                && _accessSettings.ReadUserAccess != null)
            {
                var executorUser = workContext.CurrentUser as UserEntity;
                if (executorUser == null || executorUser.UT_Us == null || executorUser.UT_Us.Count == 0)
                {
                    executorUser = await _userRepository.GetOrSetUserFromWorkContext(workContext);
                }

                var executorRoles = await _userRepository.GetOrSetUserRoleFromWorkContext(workContext);
                var accessSetting = GetFinalAccessSettingOfRoles(executorRoles, _accessSettings.ReadDepartmentAccess);
                if (accessSetting == null) { return (null, null); }

                var accessibleHierarchyInfos = await base.GetAccessibleHierarchyInfosAsync(workContext, executorUser.DepartmentId, accessSetting);

                var topHierarchyInfo = FindTopHierarchyInfo(accessibleHierarchyInfos);
                HierachyDepartmentIdentityDto topHierachyDepartmentIdentity = null;
                if (topHierarchyInfo != null)
                {
                    var topHierarchyDepartment = (await _hierarchyDepartmentRepository.GetListHierarchyDepartmentEntityAsync(
                            new List<int> { topHierarchyInfo.HdId }, true, true)).FirstOrDefault();
                    if (topHierarchyDepartment != null)
                    {
                        topHierachyDepartmentIdentity = _hierarchyDepartmentMappingService.ToDto(topHierarchyDepartment, topHierarchyDepartment.Parent);
                    }

                }

                return (topHierachyDepartmentIdentity, accessibleHierarchyInfos);
            }

            return (null, null);
        }

    }
}
