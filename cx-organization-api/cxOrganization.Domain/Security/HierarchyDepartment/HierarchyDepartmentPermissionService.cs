using cxOrganization.Client.Departments;
using cxOrganization.Domain.AdvancedWorkContext;
using cxOrganization.Domain.Common;
using cxOrganization.Domain.Entities;
using cxOrganization.Domain.Extensions;
using cxOrganization.Domain.Repositories;
using cxPlatform.Core;
using cxPlatform.Core.Common;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace cxOrganization.Domain.Security.HierarchyDepartment
{
    /// <summary>
    /// While waiting for the Permission API so this class is created to handle the simple logic on checking access of hierarchy departments.
    /// Later on it will be removed.
    /// </summary>
    public class HierarchyDepartmentPermissionService : IHierarchyDepartmentPermissionService
    {
        private readonly IAdvancedWorkContext _workContext;
        private readonly HierarchyDepartmentPermissionSettings _hierarchyDepartmentPermissionSettings;
        private readonly IUserRepository _userRepository;
        private readonly IDepartmentTypeRepository _departmentTypeRepository;
        private UserEntity _workContextUser;
        private IList<UserRole> _workContextUserRoles;

        public HierarchyDepartmentPermissionService(
            IAdvancedWorkContext workContext,
            IOptions<HierarchyDepartmentPermissionSettings> hierarchyDepartmentPermissionSettings,
            IDepartmentTypeRepository departmentTypeRepository,
            IUserRepository userRepository)
        {
            _workContext = workContext;
            _hierarchyDepartmentPermissionSettings = hierarchyDepartmentPermissionSettings.Value;
            _userRepository = userRepository;
            _departmentTypeRepository = departmentTypeRepository;
        }
        private List<DepartmentTypeEntity> departmentTypeEntities
        {
            get
            {
                if (_departmentTypeEntities == null)
                    _departmentTypeEntities = _departmentTypeRepository.GetAllDepartmentTypesInCache();
                return _departmentTypeEntities;
            }
        }

        private List<DepartmentTypeEntity> _departmentTypeEntities { get; set; }

        public ICollection<HierarchyDepartmentEntity> SecurityCheck(ICollection<HierarchyDepartmentEntity> hierarchyDepartmentEntities)
        {
            var user = GetWorkContextUser();

            var userDepartmentId = user.DepartmentId;
            var userRoleExtIds = user.UT_Us.Select(r => r.UserType.ExtId).ToList();

            if (UserHasFullAccessOnDescendentDepartments(userRoleExtIds) || IgnoreCheckDepartmentDeny())
            {
                return hierarchyDepartmentEntities;
            }

            var allowedHds = FilterOnDepartmentTypes(userDepartmentId, hierarchyDepartmentEntities);

            foreach (var allowedHd in allowedHds)
            {
                // Filter on children H_Ds.
                allowedHd.H_Ds = FilterOnDepartmentTypes(userDepartmentId, allowedHd.H_Ds);
            }

            return allowedHds;
        }
        public async Task<ICollection<HierarchyDepartmentEntity>> SecurityCheckAsync(ICollection<HierarchyDepartmentEntity> hierarchyDepartmentEntities)
        {
            var user = await GetWorkContextUserAsync();

            var userDepartmentId = user.DepartmentId;
            var userRoleExtIds = await GetWorkContextUserRoleExtIdsAsync();

            if (UserHasFullAccessOnDescendentDepartments(userRoleExtIds)
                || IgnoreCheckDepartmentDeny())
            {
                return hierarchyDepartmentEntities;
            }

            var allowedHds = FilterOnDepartmentTypes(userDepartmentId, hierarchyDepartmentEntities);

            foreach (var allowedHd in allowedHds)
            {
                // Filter on children H_Ds.
                allowedHd.H_Ds = FilterOnDepartmentTypes(userDepartmentId, allowedHd.H_Ds);
            }

            return allowedHds;
        }

        public async Task<bool> IgnoreSecurityCheckAsync()
        {
            var isNotAuthenticatedByToken = string.IsNullOrEmpty(_workContext.Sub);
            if (isNotAuthenticatedByToken || _hierarchyDepartmentPermissionSettings == null || !_hierarchyDepartmentPermissionSettings.HasConfig())
            {
                return true;
            }

            return await HasFullAccessOnHierarchyDepartmentAsync();

        }

        public bool UserIsAuthenticatedByToken()
        {
            return !string.IsNullOrEmpty(_workContext.Sub);
        }

        public ICollection<HierarchyDepartmentEntity> FilterOnDepartmentTypes(int userDepartmentId, ICollection<HierarchyDepartmentEntity> hierarchyDepartmentEntities)
        {
            var deniedHds = hierarchyDepartmentEntities
                .Where(hd =>
                    hd.DepartmentId != userDepartmentId
                    && MatchAnyDepartmentTypes(hd, _hierarchyDepartmentPermissionSettings.DenyDepartmentTypeExtIdsIfNotFullAccess))
                .ToList();

            return hierarchyDepartmentEntities.Except(deniedHds).ToList();
        }

        public UserEntity GetWorkContextUser()
        {
            if (_workContextUser != null) return _workContextUser;
            _workContextUser = DomainHelper.GetUserEntityFromWorkContextSub(_workContext, _userRepository);

            return _workContextUser;
        }
        public async Task<UserEntity> GetWorkContextUserAsync()
        {
            if (_workContextUser != null) return _workContextUser;
            _workContextUser = await _userRepository.GetOrSetUserFromWorkContext(_workContext);

            return _workContextUser;
        }
        public async Task<IList<UserRole>> GetWorkContextUserRolesAsync()
        {
            if (_workContextUserRoles != null) return _workContextUserRoles;
            _workContextUserRoles = await _userRepository.GetOrSetUserRoleFromWorkContext(_workContext);

            return _workContextUserRoles;
        }
        public async Task<List<string>> GetWorkContextUserRoleExtIdsAsync()
        {
            if (_workContextUserRoles == null)
            {
                _workContextUserRoles = await _userRepository.GetOrSetUserRoleFromWorkContext(_workContext);
            }

            return _workContextUserRoles.Select(u => u.ExtId).ToList();
        }
        public async Task<bool> HasFullAccessOnHierarchyDepartmentAsync()
        {
            if (_hierarchyDepartmentPermissionSettings == null || _hierarchyDepartmentPermissionSettings.FullAccessOnHierarchyDepartment.IsNullOrEmpty())
                return true;

            var currentUserRoleExtIds = await GetWorkContextUserRoleExtIdsAsync();
            return currentUserRoleExtIds.Any(userRoleExtId => _hierarchyDepartmentPermissionSettings.FullAccessOnHierarchyDepartment.Contains(userRoleExtId, StringComparer.CurrentCultureIgnoreCase));
        }

        public int GetRootDepartmentId()
        {
            return _hierarchyDepartmentPermissionSettings.RootDepartmentId;
        }

        public List<HierachyDepartmentIdentityDto> ProcessRemovingTheRootDepartment(ICollection<HierachyDepartmentIdentityDto> hierachyDepartmentIdentityDtos)
        {
            if (hierachyDepartmentIdentityDtos.Count == 0 || !_hierarchyDepartmentPermissionSettings.ExcludeTheRootDepartment)
            {
                return hierachyDepartmentIdentityDtos.ToList();
            }

            // Remove the top node.
            hierachyDepartmentIdentityDtos = hierachyDepartmentIdentityDtos.Where(p => p.Identity.Id != _hierarchyDepartmentPermissionSettings.RootDepartmentId).ToList();

            // Remove the path of the top node.
            foreach (var hierarchyDepartment in hierachyDepartmentIdentityDtos)
            {
                var pathNames = hierarchyDepartment.PathName.Split("\\");
                var pathNamesExcludingTheTopNode = pathNames.Skip(1).ToList();
                hierarchyDepartment.PathName = string.Join("\\", pathNamesExcludingTheTopNode);
            }

            return hierachyDepartmentIdentityDtos.ToList();
        }

        private bool IgnoreCheckDepartmentDeny()
        {
            return _hierarchyDepartmentPermissionSettings.DenyDepartmentTypeExtIdsIfNotFullAccess.IsNullOrEmpty();
        }

        private bool UserHasFullAccessOnDescendentDepartments(List<string> userRoleExtIds)
        {
            return _hierarchyDepartmentPermissionSettings.FullAccessOnDescendentDepartmentUserTypeExtIds != null
                   && _hierarchyDepartmentPermissionSettings.FullAccessOnDescendentDepartmentUserTypeExtIds.Any(
                       fullAccess => userRoleExtIds.Contains(fullAccess, StringComparer.CurrentCultureIgnoreCase));
        }

        private bool MatchAnyDepartmentTypes(HierarchyDepartmentEntity hierachyDepartmentEntity, List<string> departmentTypeExtIds)
        {
            var deptTypeIds = departmentTypeEntities.Where(t => departmentTypeExtIds.Contains(t.ExtId, StringComparer.CurrentCultureIgnoreCase)).Select(t => t.DepartmentTypeId).ToList();
            return hierachyDepartmentEntity.Department != null &&
                hierachyDepartmentEntity.Department.DT_Ds.Any(deptype => deptTypeIds.Contains(deptype.DepartmentTypeId));
        }
    }
}
