using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using cxOrganization.Client.UserTypes;
using cxOrganization.Domain.Common;
using cxOrganization.Domain.Dtos.Users;
using cxOrganization.Domain.Entities;
using cxOrganization.Domain.Extensions;
using cxOrganization.Domain.Mappings;
using cxOrganization.Domain.Repositories;
using cxOrganization.Domain.Settings;
using cxPlatform.Client.ConexusBase;
using cxPlatform.Core;
using cxPlatform.Core.Common;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json.Linq;

namespace cxOrganization.Domain.Security.AccessServices
{
    public class UserAccessService: AccessServiceBase, IUserAccessService
    {
        private readonly IUserRepository _userRepository;
        private readonly IUserGroupRepository _userGroupRepository;
        private readonly IUGMemberRepository _ugMemberRepository;
        private readonly IUserTypeRepository _userTypeRepository;

        private readonly AccessSettings _accessSettings;
        private const string defaultReadUserAccessPolicy = "defaultPolicy";

        public UserAccessService(ILogger<UserAccessService> logger,
            IUserRepository userRepository,
            IHierarchyDepartmentRepository hierarchyDepartmentRepository,
            IOptions<AccessSettings> accessSettingsOption,
            IUserGroupRepository userGroupRepository,
            IDTDEntityRepository dtdEntityRepository,
            IUGMemberRepository ugMemberRepository,
            IUserTypeRepository userTypeRepository,
            IDepartmentTypeRepository departmentTypeRepository) :
            base(logger, hierarchyDepartmentRepository, dtdEntityRepository, departmentTypeRepository)
        {
            _userRepository = userRepository;
            _accessSettings = accessSettingsOption.Value;
            _userGroupRepository = userGroupRepository;
            _ugMemberRepository = ugMemberRepository;
            _userTypeRepository = userTypeRepository;
        }
      
   
        public UserAccessResult CheckReadUserAccess(IWorkContext workContext, int ownerId,
            List<int> customerIds,
            List<string> userExtIds,
            List<string> loginServiceClaims,
            List<int> userIds,
            List<int> userGroupIds,
            List<int> parentDepartmentIds,
            List<List<int>> multiUserGroupFilters,
            List<int> userTypeIdsFilter,
            List<string> userTypeExtIdsFilter,
            List<List<int>> multipleUserTypeIdsFilter,
            List<List<string>> multipleUserTypeExtIdsFilter,
            string accessPolicy = null)
        {
            var accessResultCheckingResult = CheckReadUserAccessInternal(workContext: workContext,
                ownerId: ownerId,
                customerIds: customerIds,
                userExtIdsFilter: userExtIds,
                loginServiceClaimsFilter: loginServiceClaims,
                userIdsFilter: userIds,
                userGroupIdsFilter: userGroupIds,
                parentDepartmentIdsFilter: parentDepartmentIds,
                multiUserGroupFilters: multiUserGroupFilters, userTypeIdsFilter: userTypeIdsFilter,
                userTypeExtIdsFilter: userTypeExtIdsFilter,
                multipleUserTypeIdsFilter: multipleUserTypeIdsFilter,
                multipleUserTypeExtIdsFilter: multipleUserTypeExtIdsFilter,
                executorUser: null,
                accessPolicy: accessPolicy);

            if (accessResultCheckingResult.AccessStatus == AccessStatus.AccessDenied)
            {
                _logger.LogWarning(
                    $"Logged-in user with extId {accessResultCheckingResult.ExecutorUser?.ExtId} (id {accessResultCheckingResult.ExecutorUser?.UserId}) does not have access on user.");
            }

            return accessResultCheckingResult;
        }
        public async Task<UserAccessResult> CheckReadUserAccessAsync(IWorkContext workContext, int ownerId,
            List<int> customerIds,
            List<string> userExtIds,
            List<string> loginServiceClaims,
            List<int> userIds,
            List<int> userGroupIds,
            List<int> parentDepartmentIds,
            List<List<int>> multiUserGroupFilters,
            List<int> userTypeIdsFilter,
            List<string> userTypeExtIdsFilter,
            List<List<int>> multipleUserTypeIdsFilter,
            List<List<string>> multipleUserTypeExtIdsFilter,
            string accessPolicy = null)
        {
            var accessResultCheckingResult = await CheckReadUserAccessInternalAsync(workContext: workContext,
                ownerId: ownerId,
                customerIds: customerIds,
                userExtIds: userExtIds,
                loginServiceClaims: loginServiceClaims,
                userIds: userIds,
                userGroupIds: userGroupIds,
                parentDepartmentIds: parentDepartmentIds,
                multiUserGroupFilters: multiUserGroupFilters, userTypeIdsFilter: userTypeIdsFilter,
                userTypeExtIdsFilter: userTypeExtIdsFilter,
                multipleUserTypeIdsFilter: multipleUserTypeIdsFilter,
                multipleUserTypeExtIdsFilter: multipleUserTypeExtIdsFilter,
                executorUser: null,
                accessPolicy: accessPolicy);

            if (accessResultCheckingResult.AccessStatus == AccessStatus.AccessDenied)
            {
                _logger.LogWarning(
                    $"Logged-in user with extId {accessResultCheckingResult.ExecutorUser?.ExtId} (id {accessResultCheckingResult.ExecutorUser?.UserId}) does not have access on user.");
            }

            return accessResultCheckingResult;
        }
        public UserAccessResult CheckReadUserAccess(IWorkContext workContext, UserEntity executorUser, int ownerId,
            List<int> customerIds,
            List<string> userExtIds,
            List<string> loginServiceClaims,
            List<int> userIds,
            List<int> userGroupIds,
            List<int> parentDepartmentIds,
            List<List<int>> multiUserGroupFilters,
            List<int> userTypeIdsFilter,
            List<string> userTypeExtIdsFilter,
            List<List<int>> multipleUserTypeIdsFilter,
            List<List<string>> multipleUserTypeExtIdsFilter,
            string accessPolicy = null)
        {
            var accessResultCheckingResult = CheckReadUserAccessInternal(workContext: workContext,
                ownerId: ownerId,
                customerIds: customerIds,
                userExtIdsFilter: userExtIds,
                loginServiceClaimsFilter: loginServiceClaims,
                userIdsFilter: userIds,
                userGroupIdsFilter: userGroupIds,
                parentDepartmentIdsFilter: parentDepartmentIds,
                multiUserGroupFilters: multiUserGroupFilters,
                userTypeIdsFilter: userTypeIdsFilter,
                userTypeExtIdsFilter: userTypeExtIdsFilter,
                multipleUserTypeIdsFilter: multipleUserTypeIdsFilter,
                multipleUserTypeExtIdsFilter: multipleUserTypeExtIdsFilter,
                executorUser: executorUser,
                accessPolicy: accessPolicy);

            if (accessResultCheckingResult.AccessStatus == AccessStatus.AccessDenied)
            {

                _logger.LogWarning($"Logged-in user with extId {accessResultCheckingResult.ExecutorUser?.ExtId} (id {accessResultCheckingResult.ExecutorUser?.UserId}) does not have access on user.");
            }
            return accessResultCheckingResult;
        }

        public async Task<UserAccessResult> CheckReadUserAccessAsync(IWorkContext workContext, UserEntity executorUser,
            int ownerId,
            List<int> customerIds,
            List<string> userExtIds,
            List<string> loginServiceClaims,
            List<int> userIds,
            List<int> userGroupIds,
            List<int> parentDepartmentIds,
            List<List<int>> multiUserGroupFilters,
            List<int> userTypeIdsFilter,
            List<string> userTypeExtIdsFilter,
            List<List<int>> multipleUserTypeIdsFilter,
            List<List<string>> multipleUserTypeExtIdsFilter,
            string accessPolicy = null)
        {
            var accessResultCheckingResult = await CheckReadUserAccessInternalAsync(workContext: workContext,
                ownerId: ownerId, customerIds: customerIds,
                userExtIds: userExtIds,
                loginServiceClaims: loginServiceClaims,
                userIds: userIds, userGroupIds: userGroupIds,
                parentDepartmentIds: parentDepartmentIds,
                multiUserGroupFilters: multiUserGroupFilters,
                userTypeIdsFilter: userTypeIdsFilter,
                userTypeExtIdsFilter: userTypeExtIdsFilter,
                multipleUserTypeIdsFilter: multipleUserTypeIdsFilter,
                multipleUserTypeExtIdsFilter: multipleUserTypeExtIdsFilter,
                executorUser: executorUser,
                accessPolicy: accessPolicy);

            if (accessResultCheckingResult.AccessStatus == AccessStatus.AccessDenied)
            {
                _logger.LogWarning(
                    $"Logged-in user with extId {accessResultCheckingResult.ExecutorUser?.ExtId} (id {accessResultCheckingResult.ExecutorUser?.UserId}) does not have access on user.");
            }

            return accessResultCheckingResult;
        }

        public UserAccessResult CheckEditUserAccess<T>(IWorkContext workContext, T editingUserDto,
            IUserMappingService userMappingService) where T : UserDtoBase
        {
            var editAccessCheckingResult = CheckEditUserAccessInternal(workContext, editingUserDto, userMappingService);
            if (editAccessCheckingResult.AccessStatus == AccessStatus.AccessDenied)
            {
                _logger.LogWarning(
                    $"Logged-in user with extId {editAccessCheckingResult.ExecutorUser?.ExtId} (id {editAccessCheckingResult.ExecutorUser?.UserId}) does not have access to edit user {editingUserDto.Identity.Id} (extId {editingUserDto.Identity.ExtId})");
            }

            return editAccessCheckingResult;
        }
        public async Task<UserAccessResult> CheckEditUserAccessAsync<T>(IWorkContext workContext, T editingUserDto,
            IUserMappingService userMappingService) where T : UserDtoBase
        {
            var editAccessCheckingResult = await CheckEditUserAccessInternalAsync(workContext, editingUserDto, userMappingService);
            if (editAccessCheckingResult.AccessStatus == AccessStatus.AccessDenied)
            {
                _logger.LogWarning(
                    $"Logged-in user with extId {editAccessCheckingResult.ExecutorUser?.ExtId} (id {editAccessCheckingResult.ExecutorUser?.UserId}) does not have access to edit user {editingUserDto.Identity.Id} (extId {editingUserDto.Identity.ExtId})");
            }

            return editAccessCheckingResult;
        }

        public bool CheckCreateUserAccess<T>(IWorkContext workContext, T editingUserDto) where T : UserDtoBase
        {
            var isAuthenticatedByToken = !string.IsNullOrEmpty(workContext.Sub);
            if (isAuthenticatedByToken
                && _accessSettings != null
                && !_accessSettings.DisableCreateUserAccessChecking
                && _accessSettings.CreateUserAccess != null)
            {
                var executorUser = DomainHelper.GetUserEntityFromWorkContextSub(workContext, _userRepository, true);
                var executorRoles = MapToUserRoles(executorUser.UT_Us);
                var accessSetting = GetEditabilityAccessSettingElement(executorRoles, _accessSettings.CreateUserAccess);

                if (!accessSetting.HasFullAccessOnHierarchy)
                {
                    var accessibleDepartmentIds =
                        GetAccessibleHierarchyInfos(workContext, executorUser.DepartmentId, accessSetting)
                            .Select(hd => hd.DepartmentId).ToList();

                    if (!accessibleDepartmentIds.Contains(editingUserDto.GetParentDepartmentId()))
                    {
                        _logger.LogWarning(
                            $"Logged-in user with extId {executorUser.ExtId} (id {executorUser.UserId}) does not have access to department {editingUserDto.GetParentDepartmentId()} to create user with extId {editingUserDto.Identity.ExtId}");
                        return false;
                    }
                }


                return ValidateRestrictedPropertyWhenCreating(editingUserDto, accessSetting, executorUser);
            }

            return true;
        }

        public EditabilityAccessSettingElement GetAssignRolePermission(IWorkContext workContext)
        {
            var executorUser = DomainHelper.GetUserEntityFromWorkContextSub(workContext, _userRepository, true);
            var executorRoles = MapToUserRoles(executorUser.UT_Us);
            var accessSetting = GetEditabilityAccessSettingElement(executorRoles, _accessSettings.CreateUserAccess);

           return accessSetting;
        }

        public async Task<bool> CheckCreateUserAccessAsync<T>(IWorkContext workContext, T editingUserDto) where T : UserDtoBase
        {
            var isAuthenticatedByToken = !string.IsNullOrEmpty(workContext.Sub);
            if (isAuthenticatedByToken
                && _accessSettings != null
                && !_accessSettings.DisableCreateUserAccessChecking
                && _accessSettings.CreateUserAccess != null)
            {
                var executorUser = await _userRepository.GetOrSetUserFromWorkContext(workContext);
                var executorRoles = await _userRepository.GetOrSetUserRoleFromWorkContext(workContext);
                var accessSetting = GetEditabilityAccessSettingElement(executorRoles, _accessSettings.CreateUserAccess);

                if (!accessSetting.HasFullAccessOnHierarchy)
                {
                    var accessibleDepartmentIds = await GetAccessibleHierarchyInfosAsync(workContext, executorUser.DepartmentId, accessSetting);

                    if (accessibleDepartmentIds.All(hd => hd.DepartmentId != editingUserDto.GetParentDepartmentId()))
                    {
                        _logger.LogWarning(
                            $"Logged-in user with extId {executorUser.ExtId} (id {executorUser.UserId}) does not have access to department {editingUserDto.GetParentDepartmentId()} to create user with extId {editingUserDto.Identity.ExtId}");
                        return false;
                    }
                }


                return ValidateRestrictedPropertyWhenCreating(editingUserDto, accessSetting, executorUser);
            }

            return true;
        }
        public UserAccessResult CheckEditUserAccessInternal<T>(IWorkContext workContext, T editingUserDto, IUserMappingService userMappingService) where T : UserDtoBase
        {
            var isAuthenticatedByToken = !string.IsNullOrEmpty(workContext.Sub);
            UserEntity executorUser = null;
            if (isAuthenticatedByToken
                && _accessSettings != null
                && !_accessSettings.DisableEditUserAccessChecking
                && _accessSettings.EditUserAccess != null)
            {
                executorUser = DomainHelper.GetUserEntityFromWorkContextSub(workContext, _userRepository, true);
                var executorUserId = executorUser.UserId;
                var isSelfAccess = IsSelfAccess(executorUser, (int)editingUserDto.Identity.Id);
                var executorRoles = MapToUserRoles(executorUser.UT_Us);
                var accessSetting = GetEditabilityAccessSettingElement(executorRoles, _accessSettings.EditUserAccess);

                if (isSelfAccess && accessSetting.NotAllowSelfAccess)
                {
                    return UserAccessResult.CreateAccessDeniedResult(executorUser);
                }

                var existUserEntity = _userRepository.GetUsers(workContext.CurrentOwnerId,
                        (int)(editingUserDto.Identity.Id),
                        string.Empty,
                        editingUserDto.Identity.ExtId)
                    .FirstOrDefault();

                if (existUserEntity == null)
                {
                    _logger.LogError(
                        $"User with id {editingUserDto.Identity.Id} (extId '{editingUserDto.Identity.ExtId}') is not found.");
                    return UserAccessResult.CreateDataNotFoundResult(executorUser);
                }

                if (!IsAbleToAccessUserTypeWhenEditingUser(existUserEntity, accessSetting))
                {
                    _logger.LogWarning($"Logged-in user with extId {executorUser.ExtId} (id {executorUser.UserId}) does not have access on any user type o edit user {editingUserDto.Identity.Id} (extId {editingUserDto.Identity.ExtId})");
                    return UserAccessResult.CreateAccessDeniedResult(executorUser);
                }

                if (!accessSetting.HasFullAccessOnHierarchy)
                {
                    var accessibleDepartmentIds =
                        GetAccessibleHierarchyInfos(workContext, executorUser.DepartmentId, accessSetting)
                            .Select(hd => hd.DepartmentId).ToList();
                    var ownerId = workContext.CurrentOwnerId;
                    var customerIds = new List<int> { workContext.CurrentCustomerId };

                    if (!isSelfAccess)
                    {
                        if (!IsAbleToAccessUserDepartmentWhenEditingUser(editingUserDto.GetParentDepartmentId(),
                            existUserEntity.DepartmentId,
                            accessibleDepartmentIds))
                        {
                            _logger.LogWarning(
                                $"Logged-in user with extId {executorUser.ExtId} (id {executorUser.UserId}) does not have access to department {editingUserDto.GetParentDepartmentId()} to  to user {editingUserDto.Identity.Id} (extId {editingUserDto.Identity.ExtId})");
                            return UserAccessResult.CreateAccessDeniedResult(executorUser);
                        }

                        if (!IsAbleToAccessUserGroupWhenEditingUser<T>(existUserEntity, ownerId, customerIds,
                            accessSetting, executorUserId, accessibleDepartmentIds))
                        {
                            _logger.LogWarning(
                                $"Logged-in user with extId {executorUser.ExtId} (id {executorUser.UserId}) does not have access to user group to edit user {editingUserDto.Identity.Id} (extId {editingUserDto.Identity.ExtId})");
                            return UserAccessResult.CreateAccessDeniedResult(executorUser);
                        }
                    }
                }

                var validateProperty = ValidateRestrictedPropertyWhenEditing(editingUserDto, existUserEntity, userMappingService, accessSetting, executorUser, isSelfAccess);

                return validateProperty
                    ? UserAccessResult.CreateAccessGrantedResult(executorUser)
                    : UserAccessResult.CreateAccessDeniedResult(executorUser);
            }

            return UserAccessResult.CreateAccessGrantedResult(executorUser);
        }

        public async Task<UserAccessResult> CheckEditUserAccessInternalAsync<T>(IWorkContext workContext, T editingUserDto, IUserMappingService userMappingService) where T : UserDtoBase
        {
            var isAuthenticatedByToken = !string.IsNullOrEmpty(workContext.Sub);
            UserEntity executorUser = null;
            if (isAuthenticatedByToken
                && _accessSettings != null
                && !_accessSettings.DisableEditUserAccessChecking
                && _accessSettings.EditUserAccess != null)
            {
                executorUser =  await _userRepository.GetOrSetUserFromWorkContext(workContext);
                var executorUserId = executorUser.UserId;
                var isSelfAccess = IsSelfAccess(executorUser, (int)editingUserDto.Identity.Id);
                var executorRoles = await _userRepository.GetOrSetUserRoleFromWorkContext(workContext);
                var accessSetting = GetEditabilityAccessSettingElement(executorRoles, _accessSettings.EditUserAccess);

                if (isSelfAccess && accessSetting.NotAllowSelfAccess)
                {
                    return UserAccessResult.CreateAccessDeniedResult(executorUser);
                }

                var existUserEntity = (await _userRepository.GetUsersAsync(workContext.CurrentOwnerId,
                        (int)(editingUserDto.Identity.Id),
                        string.Empty,
                        editingUserDto.Identity.ExtId))
                    .FirstOrDefault();

                if (existUserEntity == null)
                {
                    _logger.LogError(
                        $"User with id {editingUserDto.Identity.Id} (extId '{editingUserDto.Identity.ExtId}') is not found.");
                    return UserAccessResult.CreateDataNotFoundResult(executorUser);
                }


                if (!IsAbleToAccessUserTypeWhenEditingUser(existUserEntity, accessSetting))
                {
                    _logger.LogWarning($"Logged-in user with extId {executorUser.ExtId} (id {executorUser.UserId}) does not have access on any user type o edit user {editingUserDto.Identity.Id} (extId {editingUserDto.Identity.ExtId})");
                    return UserAccessResult.CreateAccessDeniedResult(executorUser);
                }

                if (!accessSetting.HasFullAccessOnHierarchy)
                {
                    var accessibleDepartmentIds =
                        (await GetAccessibleHierarchyInfosAsync(workContext, executorUser.DepartmentId, accessSetting))
                        .Select(hd => hd.DepartmentId).ToList();

                    var ownerId = workContext.CurrentOwnerId;
                    var customerIds = new List<int> { workContext.CurrentCustomerId };

                    if (!isSelfAccess)
                    {
                        if (!IsAbleToAccessUserDepartmentWhenEditingUser(editingUserDto.GetParentDepartmentId(),
                            existUserEntity.DepartmentId,
                            accessibleDepartmentIds))
                        {
                            _logger.LogWarning(
                                $"Logged-in user with extId {executorUser.ExtId} (id {executorUser.UserId}) does not have access to department {editingUserDto.GetParentDepartmentId()} to  to user {editingUserDto.Identity.Id} (extId {editingUserDto.Identity.ExtId})");
                            return UserAccessResult.CreateAccessDeniedResult(executorUser);
                        }

                        var isAbleToAccessToUserGroup = await IsAbleToAccessUserGroupWhenEditingUserAsync<T> (existUserEntity, ownerId, customerIds, accessSetting, executorUserId, accessibleDepartmentIds);
                        if (!isAbleToAccessToUserGroup)
                        {
                            _logger.LogWarning(
                                $"Logged-in user with extId {executorUser.ExtId} (id {executorUser.UserId}) does not have access to user group to edit user {editingUserDto.Identity.Id} (extId {editingUserDto.Identity.ExtId})");
                            return UserAccessResult.CreateAccessDeniedResult(executorUser);
                        }
                    }
                }

                var validateProperty = ValidateRestrictedPropertyWhenEditing(editingUserDto, existUserEntity, userMappingService, accessSetting, executorUser, isSelfAccess);
                return validateProperty
                    ? UserAccessResult.CreateAccessGrantedResult(executorUser)
                    : UserAccessResult.CreateAccessDeniedResult(executorUser);
            }

            return UserAccessResult.CreateAccessGrantedResult(executorUser);
        }

        private bool ValidateRestrictedPropertyWhenCreating<T>(T editingUserDto,  EditabilityAccessSettingElement accessSetting, UserEntity executorUser) where T : UserDtoBase
        {
            {
                if (accessSetting.RestrictedProperties != null && accessSetting.RestrictedProperties.Count > 0)
                {

                    foreach (var restrictedPropertyKeyValue in accessSetting.RestrictedProperties)
                    {
                        var propertyName = restrictedPropertyKeyValue.Key;
                        var restrictedProperty = restrictedPropertyKeyValue.Value;

                        if (MatchProperty(propertyName, "entityStatus"))
                        {
                            if (!ValidateRestrictedEntityStatusWhenCreating(executorUser, editingUserDto, restrictedProperty))
                                return false;
                        }
                        else if (MatchProperty(propertyName, "externallyMastered"))
                        {
                            if (!ValidateRestrictedExternalMasteredWhenCreating(editingUserDto, restrictedProperty,
                                executorUser))
                                return false;
                        }                     
                    }
                }

                return true;
            }
        }
        private bool ValidateRestrictedPropertyWhenEditing<T>(T editingUserDto, UserEntity existUserEntity,
            IUserMappingService userMappingService, EditabilityAccessSettingElement accessSetting, UserEntity executorUser,
            bool isSelfAccess) where T : UserDtoBase
        {
            {
                if (accessSetting.RestrictedProperties != null && accessSetting.RestrictedProperties.Count > 0)
                {
                    if (userMappingService.ToUserDto(existUserEntity, null, keepDecryptedSsn: true) is T currentUserDto)
                    {
                        foreach (var restrictedPropertyKeyValue in accessSetting.RestrictedProperties)
                        {
                            var propertyName = restrictedPropertyKeyValue.Key;
                            var restrictedProperty = restrictedPropertyKeyValue.Value;
                            if (SkipCheckingRestrictedPropertyWhenEditing<T>(editingUserDto, currentUserDto, propertyName,
                                restrictedPropertyKeyValue.Value, isSelfAccess, existUserEntity)) continue;
                    
                            if (MatchProperty(propertyName, "entityStatus"))
                            {
                                if (!ValidateRestrictedEntityStatusWhenEditing(executorUser, editingUserDto,
                                    currentUserDto, restrictedProperty, isSelfAccess))
                                    return false;
                            }
                            else if (MatchProperty(propertyName, "delete"))
                            {
                                if (!ValidateRestrictedDelete(editingUserDto, currentUserDto, restrictedProperty,
                                    executorUser, isSelfAccess))
                                    return false;
                            }
                            else if (MatchProperty(propertyName, "emailAddress"))
                            {
                                if (!ValidateRestrictedEmailAddressWhenEditing(executorUser, editingUserDto,
                                    restrictedProperty, currentUserDto, isSelfAccess))
                                    return false;
                            }
                            else if (MatchProperty(propertyName, "ssn"))
                            {
                                if (!ValidateRestrictedSsnWhenEditing(executorUser, editingUserDto,
                                    restrictedProperty, currentUserDto, isSelfAccess))
                                    return false;
                            }
                        }
                    }
                }

                return true;
            }
        }

        private bool SkipCheckingRestrictedPropertyWhenEditing<T>(T newUser, T oldUser, string name, RestrictedProperty restrictedProperty, bool isSelfAccess, UserEntity existUserEntity) where T : UserDtoBase
        {
            if (MatchProperty(name, "entityStatus") && oldUser.EntityStatus.StatusId == EntityStatusEnum.New && newUser.EntityStatus.StatusId == EntityStatusEnum.Active)
            {
                //This case is normal control by system when user login to system,
                //Hard check for now.
                return true;
            }

            return (isSelfAccess && restrictedProperty.AllowSelfAccess) || (restrictedProperty.RestrictForExternallyMasteredUserOnly == true && !oldUser.EntityStatus.ExternallyMastered);
        }

        private bool MatchProperty(string property, string name)
        {
            return string.Equals(property, name, StringComparison.CurrentCultureIgnoreCase);
        }

        private bool ValidateRestrictedDelete<T>(T editingUserDto, T currentUserDto,
            RestrictedProperty restrictedProperty,
            UserEntity executorUser, bool isSelfAccess) where T : UserDtoBase
        {
            if (!currentUserDto.EntityStatus.Deleted && editingUserDto.EntityStatus.Deleted)
            {
                if ((isSelfAccess && !restrictedProperty.AllowSelfAccess) || restrictedProperty.AllowedValues == null || !restrictedProperty.AllowedValues.Contains(
                        editingUserDto.EntityStatus.Deleted.ToString(),
                        StringComparer.CurrentCultureIgnoreCase))
                {
                    _logger.LogWarning(
                        $"Logged-in user with extId {executorUser.ExtId} (id {executorUser.UserId}) is not allowed to delete user {currentUserDto.Identity.Id} (extId '{currentUserDto.Identity.ExtId}').");
                    return false;
                }
            }

            return true;
        }

        private bool ValidateRestrictedExternalMasteredWhenCreating<T>(T editingUserDto,
            RestrictedProperty restrictedProperty,
            UserEntity executorUser) where T : UserDtoBase
        {

            if (restrictedProperty.AllowedValues == null
                || !restrictedProperty.AllowedValues.Contains(
                    editingUserDto.EntityStatus.ExternallyMastered.ToString(),
                    StringComparer.CurrentCultureIgnoreCase))
            {
                _logger.LogWarning(
                    $"Logged-in user with extId {executorUser.ExtId} (id {executorUser.UserId}) is not allowed to create user with external mastered is {editingUserDto.EntityStatus.ExternallyMastered}");
                return false;
            }

            return true;
        }

        private bool ValidateRestrictedEntityStatusWhenEditing<T>(UserEntity executorUser, T editingUserDto, T currentUserDto, RestrictedProperty restrictedProperty, bool isSelfAccess)
            where T : UserDtoBase
        {
            if (editingUserDto.EntityStatus.StatusId != currentUserDto.EntityStatus.StatusId)
            {
                if (isSelfAccess)
                {
                    if (!restrictedProperty.AllowSelfAccess ||
                        restrictedProperty.AllowedSelfChangeEntityStatuses.IsNullOrEmpty() ||
                        !restrictedProperty.AllowedSelfChangeEntityStatuses.Any(
                            c => (c.From == EntityStatusEnum.All || c.From == currentUserDto.EntityStatus.StatusId) &&
                                 (c.To == EntityStatusEnum.All || c.To == editingUserDto.EntityStatus.StatusId)))
                    {
                        _logger.LogWarning(
                            $"Logged-in user with extId {executorUser.ExtId} (id {executorUser.UserId}) is not allowed to self-change entity status from {currentUserDto.EntityStatus.StatusId}  to {editingUserDto.EntityStatus.StatusId}.");
                        return false;
                    }
                }
                else if (
                    restrictedProperty.AllowedChangeEntityStatuses.IsNullOrEmpty() ||
                    !restrictedProperty.AllowedChangeEntityStatuses.Any(
                        c => (c.From == EntityStatusEnum.All || c.From == currentUserDto.EntityStatus.StatusId) &&
                             (c.To == EntityStatusEnum.All || c.To == editingUserDto.EntityStatus.StatusId)))
                {
                    _logger.LogWarning(
                        $"Logged-in user with extId {executorUser.ExtId} (id {executorUser.UserId}) is not allowed to change entity status of user {currentUserDto.Identity.Id} (extId '{currentUserDto.Identity.ExtId}') from {currentUserDto.EntityStatus.StatusId}  to {editingUserDto.EntityStatus.StatusId}.");
                    return false;
                }
            }

            return true;
        }

        private bool ValidateRestrictedEntityStatusWhenCreating<T>(UserEntity executorUser, T editingUserDto,
            RestrictedProperty restrictedProperty)
            where T : UserDtoBase
        {

            if (!restrictedProperty.AllowedValues.IsNullOrEmpty() && restrictedProperty.AllowedValues.Contains("All", StringComparer.CurrentCultureIgnoreCase)) return true;

            var restrictAnyValue = restrictedProperty.AllowedValues.IsNullOrEmpty();

            if (restrictAnyValue || !restrictedProperty.AllowedValues.Contains( editingUserDto.EntityStatus.StatusId.ToString(), StringComparer.CurrentCultureIgnoreCase))
            {
                _logger.LogWarning(
                    $"Logged-in user with extId {executorUser.ExtId} (id {executorUser.UserId}) is not allowed to create user with given entity status '{editingUserDto.EntityStatus.StatusId}'.");
                return false;
            }

            return true;
        }

        private bool ValidateRestrictedEmailAddressWhenEditing<T>(UserEntity executorUser, T editingUserDto, 
            RestrictedProperty restrictedProperty, T currentUserDto, bool isSelfAccess) where T : UserDtoBase
        {
            var valueIsNotChanged = (string.IsNullOrEmpty(editingUserDto.EmailAddress) &&
                                     string.IsNullOrEmpty(currentUserDto.EmailAddress))
                                    || (editingUserDto.EmailAddress == currentUserDto.EmailAddress);
            if (valueIsNotChanged)
            {
                return true;
            }

            if ((isSelfAccess || !restrictedProperty.AllowSelfAccess) ||
                restrictedProperty.AllowedValues.IsNullOrEmpty() || !restrictedProperty.AllowedValues.Contains(
                    editingUserDto.EmailAddress, StringComparer.CurrentCultureIgnoreCase)
            )
            {
                _logger.LogWarning(
                    $"Logged-in user with extId {executorUser.ExtId} (id {executorUser.UserId}) is not allowed to change email address of  user {currentUserDto.Identity.Id} (extId '{currentUserDto.Identity.ExtId}') .");
                return false;

            }

            return true;
        }
        private bool ValidateRestrictedSsnWhenEditing<T>(UserEntity executorUser, T editingUserDto,
            RestrictedProperty restrictedProperty, T currentUserDto, bool isSelfAccess) where T : UserDtoBase
        {
            //The current logic we handle that if client set NULL for SSN, we do not update it, then we don't need restrict here.
            if (editingUserDto.SSN == null) return true;

            var valueIsNotChanged =
                (string.IsNullOrEmpty(editingUserDto.SSN) && string.IsNullOrEmpty(currentUserDto.SSN))
                || (editingUserDto.SSN == currentUserDto.SSN);

            if (valueIsNotChanged)
            {
                return true;
            }

            if ((isSelfAccess && !restrictedProperty.AllowSelfAccess) ||
                restrictedProperty.AllowedValues.IsNullOrEmpty() ||
                !restrictedProperty.AllowedValues.Contains(editingUserDto.SSN, StringComparer.CurrentCultureIgnoreCase))
            {
                _logger.LogWarning(
                    $"Logged-in user with extId {executorUser.ExtId} (id {executorUser.UserId}) is not allowed to change SSN of  user {currentUserDto.Identity.Id} (extId '{currentUserDto.Identity.ExtId}') .");
                return false;
            }

            return true;
        }

        private List<UserTypeDto> GetUserTypeFromCustomData<T>(string customDataKey, T userDto) where T : UserDtoBase
        {
            if (userDto.CustomData != null && userDto.CustomData.ContainsKey(customDataKey))
            {
                if (userDto.CustomData[customDataKey] is JArray userTypeJArray)
                {
                    return userTypeJArray.ToObject<List<UserTypeDto>>();
                }
                else if(userDto.CustomData[customDataKey] is IEnumerable<UserTypeDto> userTypeDtos)
                {
                    return userTypeDtos.ToList();
                }
            }

            return null;
        }

      

        private EditabilityAccessSettingElement GetEditabilityAccessSettingElement(IList<UserRole> executorRoles,
            Dictionary<string, EditabilityAccessSettingElement> accessSettingByRole)
        {
            if (accessSettingByRole == null) return null;
            var editUserAccessSettings = GetAccessSettings(executorRoles, accessSettingByRole);
               
            if (editUserAccessSettings.Count == 0)
            {
                return null;
            }


            if (editUserAccessSettings.Count == 1)
            {
                return editUserAccessSettings.First();
            }

            var finalAccessSetting = UnionAccessSettings(editUserAccessSettings);
            finalAccessSetting.NotAllowSelfAccess = editUserAccessSettings.All(a => a.NotAllowSelfAccess);
            var allRestrictedPropertyNames = editUserAccessSettings
                .Where(s => !s.RestrictedProperties.IsNullOrEmpty())
                .SelectMany(r => r.RestrictedProperties).Select(r => r.Key).Distinct().ToList();
            if (allRestrictedPropertyNames.Count > 0)
            {
                finalAccessSetting.RestrictedProperties = new Dictionary<string, RestrictedProperty>();

                foreach (var restrictedPropertyName in allRestrictedPropertyNames)
                {
                    //For an user, if a property is restricted for any specific role, but not for all, we consider it is not restricted at all
                    var allowAccessInAnyRole = editUserAccessSettings.Any(e => e.RestrictedProperties.IsNullOrEmpty()
                                                                               || !e.RestrictedProperties.ContainsKey(restrictedPropertyName));
                    if (allowAccessInAnyRole)
                    {
                        continue;
                    }

                    var allConfiguredValueOfRestrictedProperty = editUserAccessSettings
                        .Select(e => e.RestrictedProperties[restrictedPropertyName])
                        .ToList();

                    //For an user, if each role he is allowed to edit some values on restrict property, so with his all roles, he must be able to edit on union values.
                    var restrictedProperty = new RestrictedProperty
                    {
                        AllowSelfAccess = allConfiguredValueOfRestrictedProperty.Any(c => c.AllowSelfAccess),
                        AllowedValues = allConfiguredValueOfRestrictedProperty
                            .Where(c => c.AllowedValues != null)
                            .SelectMany(a => a.AllowedValues).Distinct().ToList(),
                        RestrictForExternallyMasteredUserOnly =
                            allConfiguredValueOfRestrictedProperty.Any(c => c.RestrictForExternallyMasteredUserOnly == true),
                        AllowedChangeEntityStatuses = allConfiguredValueOfRestrictedProperty
                            .Where(c => c.AllowedChangeEntityStatuses != null)
                            .SelectMany(a => a.AllowedChangeEntityStatuses).ToList(),
                        AllowedSelfChangeEntityStatuses = allConfiguredValueOfRestrictedProperty
                            .Where(c => c.AllowedSelfChangeEntityStatuses != null)
                            .SelectMany(a => a.AllowedSelfChangeEntityStatuses).ToList()


                    };

                    finalAccessSetting.RestrictedProperties.Add(restrictedPropertyName, restrictedProperty);
                }

            }
            return finalAccessSetting;
        }

        private bool IsAbleToAccessUserGroupWhenEditingUser<T>(UserEntity existUserEntity, int ownerId,
            List<int> customerIds,
            AccessSettingElement accessSetting, int executorUserId, List<int> accessibleDepartmentIds)
            where T : UserDtoBase
        {
            var userGroupAccess = GetAccessibleUserGroupIds(ownerId, customerIds,
                null, null, accessSetting,
                executorUserId, accessibleDepartmentIds);
          
            if (!userGroupAccess.AllowAccess)
            {
                return false;
            }

            var multiRequiredUserGroupIds = userGroupAccess.MultiUserGroupFilters;

            var needToCheckUserExistInRequiredUserGroup = !multiRequiredUserGroupIds.IsNullOrEmpty()
                                                          && multiRequiredUserGroupIds.Any(m => !m.IsNullOrEmpty());
            if (needToCheckUserExistInRequiredUserGroup)
            {
                var doNotExistInRequiredUserGroupIds = false;
                var userIds = new List<int> {existUserEntity.UserId};
                foreach (var requireUserGroupIds in multiRequiredUserGroupIds)
                {
                    if (!requireUserGroupIds.IsNullOrEmpty())
                    {
                        if (_ugMemberRepository.CountUGMembers(ownerId, customerIds, userGroupIds: requireUserGroupIds, userIds: userIds) == 0)
                        {
                            doNotExistInRequiredUserGroupIds = true;
                            break;
                        }
                    }
                }

                if (doNotExistInRequiredUserGroupIds)
                {
                    return false;
                }
            }

            return true;
        }
        private async Task<bool> IsAbleToAccessUserGroupWhenEditingUserAsync<T>(UserEntity existUserEntity, int ownerId,
            List<int> customerIds,
            AccessSettingElement accessSetting, int executorUserId, List<int> accessibleDepartmentIds)
            where T : UserDtoBase
        {
            var userGroupAccess = await GetAccessibleUserGroupIdsAsync(ownerId, customerIds,
                null, null, accessSetting,
                executorUserId, accessibleDepartmentIds);

            if (!userGroupAccess.AllowAccess)
            {
                return false;
            }

            var multiRequiredUserGroupIds = userGroupAccess.MultiUserGroupFilters;

            var needToCheckUserExistInRequiredUserGroup = !multiRequiredUserGroupIds.IsNullOrEmpty()
                                                          && multiRequiredUserGroupIds.Any(m => !m.IsNullOrEmpty());
            if (needToCheckUserExistInRequiredUserGroup)
            {
                var doNotExistInRequiredUserGroupIds = false;
                var userIds = new List<int> { existUserEntity.UserId };
                foreach (var requireUserGroupIds in multiRequiredUserGroupIds)
                {
                    if (!requireUserGroupIds.IsNullOrEmpty())
                    {
                        if ( (await _ugMemberRepository.CountUGMembersAsync(ownerId, customerIds, userGroupIds: requireUserGroupIds, userIds: userIds)) == 0)
                        {
                            doNotExistInRequiredUserGroupIds = true;
                            break;
                        }
                    }
                }

                if (doNotExistInRequiredUserGroupIds)
                {
                    return false;
                }
            }

            return true;
        }


        private bool IsAbleToAccessUserDepartmentWhenEditingUser(int newDepartmentId, int existingDepartmentId, List<int> accessibleDepartmentIds)
        {
            if (newDepartmentId == existingDepartmentId)
            {
                if (!accessibleDepartmentIds.Contains(existingDepartmentId))
                {
                    return false;
                }
            }
            else
            {
                if (!accessibleDepartmentIds.Contains(newDepartmentId)
                    || !accessibleDepartmentIds.Contains(existingDepartmentId))
                    return false;
            }

            return true;
        }

        private UserAccessResult CheckReadUserAccessInternal(IWorkContext workContext, int ownerId,
            List<int> customerIds,
            List<string> userExtIdsFilter,
            List<string> loginServiceClaimsFilter,
            List<int> userIdsFilter,
            List<int> userGroupIdsFilter,
            List<int> parentDepartmentIdsFilter,
            List<List<int>> multiUserGroupFilters,
            List<int> userTypeIdsFilter,
            List<string> userTypeExtIdsFilter,
            List<List<int>> multipleUserTypeIdsFilter,
            List<List<string>> multipleUserTypeExtIdsFilter,
            UserEntity executorUser, string accessPolicy)
        {
            if (string.IsNullOrEmpty(accessPolicy))
                accessPolicy = defaultReadUserAccessPolicy;

            var userTypesOnCache = _userTypeRepository.GetAllUserTypesInCache();

            var unitedMultipleUserTypeIdsFilter = UnionUserTypeIdsFilters(userTypeIdsFilter, userTypeExtIdsFilter, multipleUserTypeIdsFilter, multipleUserTypeExtIdsFilter, userTypesOnCache);

            if (unitedMultipleUserTypeIdsFilter.DataNotFound)
            {
                _logger.LogWarning("Given user-type filter is not found");
                return UserAccessResult.CreateDataNotFoundResult(executorUser);
            }
            multipleUserTypeIdsFilter = unitedMultipleUserTypeIdsFilter.MultipleUserTypeIds;

            var isAuthenticatedByToken = !string.IsNullOrEmpty(workContext.Sub);
            if (isAuthenticatedByToken
                && _accessSettings != null
                && !_accessSettings.DisableReadUserAccessChecking
                && _accessSettings.ReadUserAccess != null)
            {
                if (executorUser == null || executorUser.UT_Us == null || executorUser.UT_Us.Count == 0)
                {
                    executorUser = DomainHelper.GetUserEntityFromWorkContextSub(workContext, _userRepository, true);
                }

                if (unitedMultipleUserTypeIdsFilter.DataNotFound)
                    return UserAccessResult.CreateAccessDeniedResult(executorUser);

                var executorUserId = executorUser.UserId;
                workContext.IsSelfAccess = IsSelfAccess(executorUser, workContext.Sub, userExtIdsFilter, userIdsFilter, loginServiceClaimsFilter);

                if (workContext.IsSelfAccess)
                {
                    return UserAccessResult.CreateAccessGrantedResult(userIdsFilter, userGroupIdsFilter,parentDepartmentIdsFilter, multiUserGroupFilters, multipleUserTypeIdsFilter, executorUser);
                }


                var executorRoles = MapToUserRoles(executorUser.UT_Us);

                if (!_accessSettings.ReadUserAccess.TryGetValue(accessPolicy, out var readUserAccessSettings))
                {
                    _logger.LogWarning($"Access setting on user with policy '{accessPolicy}' is not found");
                    return UserAccessResult.CreateAccessDeniedResult(executorUser);
                }

                var accessSetting = GetFinalAccessSettingOfRoles(executorRoles, readUserAccessSettings);
                if (accessSetting == null) { return UserAccessResult.CreateAccessDeniedResult(executorUser); }

                var accessibleUserTypeResult = GetAccessibleUserTypeIds(multipleUserTypeIdsFilter, userTypesOnCache, accessSetting);
                if (!accessibleUserTypeResult.AllowAccess)
                {
                    _logger.LogWarning($"Logged-in user with extId {executorUser.ExtId} (id {executorUser.UserId}) does not have access on user type to retrieve user");
                    return UserAccessResult.CreateAccessDeniedResult(executorUser);
                }

                multipleUserTypeIdsFilter = accessibleUserTypeResult.MultipleUserTypeIds;


                if (accessSetting.HasFullAccessOnHierarchy)
                {
                    return UserAccessResult.CreateAccessGrantedResult(userIdsFilter, userGroupIdsFilter,
                        parentDepartmentIdsFilter, multiUserGroupFilters, multipleUserTypeIdsFilter, executorUser);

                }

                var accessibleDepartmentIds =
                    GetAccessibleHierarchyInfos(workContext, executorUser.DepartmentId, accessSetting)
                        .Select(hd => hd.DepartmentId).ToList();


                if (!parentDepartmentIdsFilter.IsNullOrEmpty())
                {
                    parentDepartmentIdsFilter = parentDepartmentIdsFilter
                        .Where(departmentId => accessibleDepartmentIds.Contains(departmentId))
                        .ToList();
                    if (parentDepartmentIdsFilter.Count == 0)
                    {
                        _logger.LogWarning(
                            $"Logged-in user with extId {executorUser.ExtId} (id {executorUser.UserId}) does not have access on department to retrieve user");
                    
                        return UserAccessResult.CreateAccessDeniedResult(executorUser);

                    }
                }
                else
                {
                    parentDepartmentIdsFilter = accessibleDepartmentIds;
                }

                var userGroupAccess = GetAccessibleUserGroupIds(ownerId, customerIds, userGroupIdsFilter,
                    multiUserGroupFilters,
                    accessSetting, executorUserId, accessibleDepartmentIds);

                userGroupIdsFilter = userGroupAccess.UserGroupIds;
                multiUserGroupFilters = userGroupAccess.MultiUserGroupFilters;

                if (!userGroupAccess.AllowAccess)
                {
                    _logger.LogWarning(
                        $"Logged-in user with extId {executorUser.ExtId} (id {executorUser.UserId}) does not have access on user group to retrieve user");
                    return UserAccessResult.CreateAccessDeniedResult(executorUser);

                }

                var hasAnyAccessOnDepartmentOrUserGroup = !parentDepartmentIdsFilter.IsNullOrEmpty()
                                                          || !userGroupIdsFilter.IsNullOrEmpty()
                                                          || (!multiUserGroupFilters.IsNullOrEmpty() &&
                                                              multiUserGroupFilters.Any(m => !m.IsNullOrEmpty()));
                if (!hasAnyAccessOnDepartmentOrUserGroup)
                {
                    //If executor user does not have access on department and user group, he is only able to get himself.
                    if (userIdsFilter.IsNullOrEmpty())
                    {
                        userIdsFilter = new List<int> {executorUserId};
                    }
                    else
                    {
                        userIdsFilter = userIdsFilter.Where(uid => uid == executorUserId).ToList();
                        if (userIdsFilter.Count == 0)
                        {
                            _logger.LogWarning(
                                $"Logged-in user with extId {executorUser.ExtId} (id {executorUser.UserId}) does not have access on user to retrieve info");
                            return UserAccessResult.CreateAccessDeniedResult(executorUser);
                          
                        }
                    }
                }
            }
          
            return UserAccessResult.CreateAccessGrantedResult(userIdsFilter, userGroupIdsFilter,
                parentDepartmentIdsFilter, multiUserGroupFilters, multipleUserTypeIdsFilter, executorUser);
        }

        private bool IsAbleToAccessUserTypeWhenEditingUser(UserEntity userEntity, AccessSettingElement accessSetting)
        {
            if (!accessSetting.OnlyUserWithUserTypeExtIds.IsNullOrEmpty())
            {
                var userTypeGroups = userEntity.UT_Us
                    .Select(u => u.UserType)
                    .GroupBy(a => a.ArchetypeId ?? 0)
                    .ToDictionary(a => a.Key, b => b.ToList());


                foreach (var grantedUserTypeExtIdsGroup in accessSetting.OnlyUserWithUserTypeExtIds) { 

                    var grantedUserTypeExtIds = grantedUserTypeExtIdsGroup.Value;
                    if (grantedUserTypeExtIds.IsNullOrEmpty())
                        return false;

                    if (!AccessSettingElement.ContainsAllSymbol(grantedUserTypeExtIds))
                    {
                        var archetypeId = (int)grantedUserTypeExtIdsGroup.Key;
                        if (userTypeGroups.TryGetValue(archetypeId, out var userTypeOfOfUser))
                        {
                            if(!userTypeOfOfUser.Any(ut=>grantedUserTypeExtIds.Contains(ut.ExtId, StringComparer.CurrentCultureIgnoreCase)))
                            {
                                return false;
                            }
                        }
                    }                  

                }
            }
            return true;
        }

        private static (bool AllowAccess, List<List<int>> MultipleUserTypeIds) GetAccessibleUserTypeIds(List<List<int>> multipleUserTypeIdsFilter, List<UserTypeEntity> userTypesOnCache, AccessSettingElement accessSetting)
        {
            if (!accessSetting.OnlyUserWithUserTypeExtIds.IsNullOrEmpty())
            {
                var givenUserTypeArchetypes = new List<ArchetypeEnum>();
                if (!multipleUserTypeIdsFilter.IsNullOrEmpty())
                {
                    //check access on given usertypeIds
                    for (int i = 0; i < multipleUserTypeIdsFilter.Count; i++)
                    {
                        var userTypeIds = multipleUserTypeIdsFilter[i];
                        if (!userTypeIds.IsNullOrEmpty())
                        {
                            var allowedUserTypeIds = new List<int>();

                            var existingUserTypeEntitiesGroups = userTypesOnCache.Where(u => userTypeIds.Contains(u.UserTypeId))
                                .GroupBy(u => u.ArchetypeId);

                            foreach (var existingUserTypeEntitiesGroup in existingUserTypeEntitiesGroups)
                            {
                                var archetype = (ArchetypeEnum)(existingUserTypeEntitiesGroup.Key ?? 0);
                                givenUserTypeArchetypes.Add(archetype);

                                if (accessSetting.OnlyUserWithUserTypeExtIds.TryGetValue(archetype, out var grantedUserTypeExtIds) && !AccessSettingElement.ContainsAllSymbol(grantedUserTypeExtIds))
                                {
                                    var grantedUserTypeIds = existingUserTypeEntitiesGroup.Where(ut => grantedUserTypeExtIds.Contains(ut.ExtId, StringComparer.CurrentCultureIgnoreCase)).Select(ut => ut.UserTypeId);

                                    allowedUserTypeIds.AddRange(grantedUserTypeIds);
                                }
                                else
                                {
                                    //There is no configuration access  or config to allow all value for  user type for this archetype, it meant, we allow any user-type with this archetype
                                    allowedUserTypeIds.AddRange(existingUserTypeEntitiesGroup.Select(ut => ut.UserTypeId));
                                }
                            }

                            if (allowedUserTypeIds.Count == 0)
                            {
                                return (false, multipleUserTypeIdsFilter);
                            }

                            multipleUserTypeIdsFilter[i] = allowedUserTypeIds;
                        }
                    }
                }


                //If there is no given on user type id filter, we get accessible user type from configuration of access setting
                multipleUserTypeIdsFilter = multipleUserTypeIdsFilter ?? new List<List<int>>();

                foreach (var grantedUserTypeExtIdsGroup in accessSetting.OnlyUserWithUserTypeExtIds)
                {
                    var configuredUserTypeArchetype = grantedUserTypeExtIdsGroup.Key;
                    if (givenUserTypeArchetypes.Contains(configuredUserTypeArchetype))
                    {
                        //This user typpe archetype is already check access above
                        continue;
                    }

                    var grantedUserTypeExtIds = grantedUserTypeExtIdsGroup.Value;
                    if (grantedUserTypeExtIds.IsNullOrEmpty())
                    {
                        return (false, multipleUserTypeIdsFilter);
                    }
                    else if (!AccessSettingElement.ContainsAllSymbol(grantedUserTypeExtIds))
                    {
                        var allowedUserTypeIds = userTypesOnCache
                            .Where(u => u.ArchetypeId ==(int) configuredUserTypeArchetype && grantedUserTypeExtIds.Contains(u.ExtId, StringComparer.CurrentCultureIgnoreCase))
                            .Select(ut => ut.UserTypeId).ToList();

                        if (allowedUserTypeIds.Count == 0)
                            return (false, multipleUserTypeIdsFilter);

                        multipleUserTypeIdsFilter.Add(allowedUserTypeIds);
                    }
                }
            }


            return (true, multipleUserTypeIdsFilter);
        }

        private static (bool DataNotFound, List<List<int>> MultipleUserTypeIds) UnionUserTypeIdsFilters(List<int> userTypeIdsFilter, List<string> userTypeExtIdsFilter,
            List<List<int>> multipleUserTypeIdsFilter, List<List<string>> multipleUserTypeExtIdsFilter, List<UserTypeEntity> userTypesOnCache)

        {
            var multipleUserTypeIds = new List<List<int>>();

            if (multipleUserTypeIdsFilter != null)
            {
                multipleUserTypeIds.AddRange(multipleUserTypeIdsFilter);
            }

            if (userTypeIdsFilter != null)
            {
                multipleUserTypeIds.Add(userTypeIdsFilter);
            }

            if (!userTypeExtIdsFilter.IsNullOrEmpty())
            {
                var userTypeIds = GetUserTypeIds(userTypeExtIdsFilter, userTypesOnCache);

                if (userTypeIds.Count == 0)
                {
                    return (true, multipleUserTypeIds);
                }

                multipleUserTypeIds.Add(userTypeIds);
            }

            if (!multipleUserTypeExtIdsFilter.IsNullOrEmpty())
            {
                foreach (var userTypeExtIds in multipleUserTypeExtIdsFilter)
                {
                    if (!userTypeExtIds.IsNullOrEmpty())
                    {
                        var userTypeIds = GetUserTypeIds(userTypeExtIds, userTypesOnCache);

                        if (userTypeIds.Count == 0)
                        {
                            return (true, multipleUserTypeIds);
                        }

                        multipleUserTypeIds.Add(userTypeIds);
                    }
                }
            }

            return (false, multipleUserTypeIds);
        }

        private static List<int> GetUserTypeIds(List<string> userTypeExtIdsFilter, List<UserTypeEntity> userTypesOnCache)
        {
            var userTypeIds = userTypesOnCache
                .Where(u => userTypeExtIdsFilter.Contains(u.ExtId, StringComparer.CurrentCultureIgnoreCase))
                .Select(u => u.UserTypeId).ToList();
            return userTypeIds;
        }

        private async Task<UserAccessResult>  CheckReadUserAccessInternalAsync(IWorkContext workContext, int ownerId,
           List<int> customerIds,
           List<string> userExtIds,
           List<string> loginServiceClaims,
           List<int> userIds,
           List<int> userGroupIds,
           List<int> parentDepartmentIds,
           List<List<int>> multiUserGroupFilters,
           List<int> userTypeIdsFilter,
           List<string> userTypeExtIdsFilter,
           List<List<int>> multipleUserTypeIdsFilter,
           List<List<string>> multipleUserTypeExtIdsFilter,
           UserEntity executorUser,
           string accessPolicy)
        {
            if (string.IsNullOrEmpty(accessPolicy))
                accessPolicy = defaultReadUserAccessPolicy;

            var userTypesOnCache = await _userTypeRepository.GetAllUserTypesInCacheAsync();

            var unitedMultipleUserTypeIdsFilter = UnionUserTypeIdsFilters(userTypeIdsFilter, userTypeExtIdsFilter, multipleUserTypeIdsFilter, multipleUserTypeExtIdsFilter, userTypesOnCache);
            if (unitedMultipleUserTypeIdsFilter.DataNotFound)
            {
                _logger.LogWarning("Given user-type filter is not found");
                return UserAccessResult.CreateDataNotFoundResult(executorUser);
            }

            multipleUserTypeIdsFilter = unitedMultipleUserTypeIdsFilter.MultipleUserTypeIds;


            var isAuthenticatedByToken = !string.IsNullOrEmpty(workContext.Sub);
            if (isAuthenticatedByToken
                && _accessSettings != null
                && !_accessSettings.DisableReadUserAccessChecking
                && _accessSettings.ReadUserAccess != null)
            {
                if (executorUser == null || executorUser.UT_Us == null || executorUser.UT_Us.Count == 0)
                {
                    executorUser = await _userRepository.GetOrSetUserFromWorkContext(workContext);
                }

              

                var executorUserId = executorUser.UserId;
                workContext.IsSelfAccess = IsSelfAccess(executorUser, workContext.Sub, userExtIds, userIds, loginServiceClaims);

                if (workContext.IsSelfAccess)
                {
                    return UserAccessResult.CreateAccessGrantedResult( userIds, userGroupIds, parentDepartmentIds, multiUserGroupFilters, multipleUserTypeIdsFilter, executorUser);
                }


                var executorRoles = await _userRepository.GetOrSetUserRoleFromWorkContext(workContext);

                if (!_accessSettings.ReadUserAccess.TryGetValue(accessPolicy, out var readUserAccessSettings))
                {
                    _logger.LogWarning($"Access setting on user with policy '{accessPolicy}' is not found");
                    return UserAccessResult.CreateAccessDeniedResult(executorUser);
                }


                var accessSetting = GetFinalAccessSettingOfRoles(executorRoles, readUserAccessSettings);
                if (accessSetting == null) { return UserAccessResult.CreateAccessDeniedResult(executorUser); }

                var accessibleUserTypeResult = GetAccessibleUserTypeIds(multipleUserTypeIdsFilter, userTypesOnCache, accessSetting);
                if (!accessibleUserTypeResult.AllowAccess)
                {
                    _logger.LogWarning($"Logged-in user with extId {executorUser.ExtId} (id {executorUser.UserId}) does not have access on user type to retrieve user");
                    return UserAccessResult.CreateAccessDeniedResult(executorUser);
                }

                multipleUserTypeIdsFilter = accessibleUserTypeResult.MultipleUserTypeIds;


                if (accessSetting.HasFullAccessOnHierarchy)
                {
                    return UserAccessResult.CreateAccessGrantedResult(userIds, userGroupIds, parentDepartmentIds, multiUserGroupFilters, multipleUserTypeIdsFilter, executorUser);
                }

                var accessibleDepartmentIds =
                    (await GetAccessibleHierarchyInfosAsync(
                        workContext,
                        executorUser.DepartmentId,
                        accessSetting,
                        parentDepartmentIds))
                    .Select(hd => hd.DepartmentId).ToList();


                if (!parentDepartmentIds.IsNullOrEmpty())
                {
                    parentDepartmentIds = parentDepartmentIds
                        .Where(departmentId => accessibleDepartmentIds.Contains(departmentId))
                        .ToList();
                    if (parentDepartmentIds.Count == 0)
                    {
                        _logger.LogWarning($"Logged-in user with extId {executorUser.ExtId} (id {executorUser.UserId}) does not have access on department to retrieve user");
                        return UserAccessResult.CreateAccessDeniedResult(executorUser);
                    }
                }
                else
                {
                    parentDepartmentIds = accessibleDepartmentIds;
                }

                var userGroupAccess = await GetAccessibleUserGroupIdsAsync(ownerId, customerIds, userGroupIds,
                    multiUserGroupFilters, accessSetting, executorUserId, accessibleDepartmentIds);

                userGroupIds = userGroupAccess.UserGroupIds;
                multiUserGroupFilters = userGroupAccess.MultiUserGroupFilters;

                if (!userGroupAccess.AllowAccess)
                {
                    _logger.LogWarning($"Logged-in user with extId {executorUser.ExtId} (id {executorUser.UserId}) does not have access on user group to retrieve user");
                    return UserAccessResult.CreateAccessDeniedResult(executorUser);

                }

                var hasAnyAccessOnDepartmentOrUserGroup = !parentDepartmentIds.IsNullOrEmpty()
                                                          || !userGroupIds.IsNullOrEmpty()
                                                          || (!multiUserGroupFilters.IsNullOrEmpty() &&
                                                              multiUserGroupFilters.Any(m => !m.IsNullOrEmpty()));
                if (!hasAnyAccessOnDepartmentOrUserGroup)
                {
                    //If executor user does not have access on department and user group, he is only able to get himself.
                    if (userIds.IsNullOrEmpty())
                    {
                        userIds = new List<int> { executorUserId };
                    }
                    else
                    {
                        userIds = userIds.Where(uid => uid == executorUserId).ToList();
                        if (userIds.Count == 0)
                        {
                            _logger.LogWarning(
                                $"Logged-in user with extId {executorUser.ExtId} (id {executorUser.UserId}) does not have access on user to retrieve info");
                            return UserAccessResult.CreateAccessDeniedResult(executorUser);
                        }
                    }
                }
            }

            return UserAccessResult.CreateAccessGrantedResult(userIds, userGroupIds, parentDepartmentIds, multiUserGroupFilters, multipleUserTypeIdsFilter, executorUser);
        }
        private (bool AllowAccess, List<int> UserGroupIds, List<List<int>> MultiUserGroupFilters) GetAccessibleUserGroupIds(
            int ownerId,
            List<int> customerIds,
            List<int> userGroupIds,
            List<List<int>> multiUserGroupFilters,
            AccessSettingElement accessSetting,
            int executorUserId,
            List<int> accessibleDepartmentIds)
        {
            //var isAccessibleOnAllOwnedUserGroupArchetype =
            //    AccessSettingElement.ContainsAllSymbol(accessSetting.InOwnedUserGroupArchetypes);
            //var isAccessibleOnAllDepartmentUserGroupArchetype =
            //    AccessSettingElement.ContainsAllSymbol(accessSetting.InDepartmentUserGroupArchetypes);

            //if (!isAccessibleOnAllOwnedUserGroupArchetype
            //    || !isAccessibleOnAllDepartmentUserGroupArchetype)
            //{
            //    var accessibleOwnedUserGroupArchetypeIds = accessSetting.InOwnedUserGroupArchetypes
            //        .Select(DomainHelper.ParseToArchetype)
            //        .Where(archetype => archetype != ArchetypeEnum.Unknown).Select(a => (int) a).ToList();

            //    var accessibleDepartmentUserGroupArchetypeIds = accessSetting.InDepartmentUserGroupArchetypes
            //        .Select(DomainHelper.ParseToArchetype)
            //        .Where(archetype => archetype != ArchetypeEnum.Unknown).Select(a => (int) a).ToList();

            //    List<int> givenUserGroupIds = new List<int>();
            //    if (userGroupIds != null)
            //    {
            //        givenUserGroupIds.AddRange(userGroupIds);
            //    }

            //    if (multiUserGroupFilters != null)
            //    {
            //        foreach (var userGroupFilters in multiUserGroupFilters)
            //        {
            //            if (userGroupFilters != null)
            //            {
            //                givenUserGroupIds.AddRange(userGroupFilters);
            //            }
            //        }
            //    }

            //    if (!givenUserGroupIds.IsNullOrEmpty())
            //    {  
            //        var userGroupAccess = GetAccessibleUserGroupIdsFromArgument(ownerId, customerIds, executorUserId,
            //            givenUserGroupIds,
            //            userGroupIds, multiUserGroupFilters,
            //            accessibleDepartmentIds, isAccessibleOnAllOwnedUserGroupArchetype,
            //            accessibleOwnedUserGroupArchetypeIds, isAccessibleOnAllDepartmentUserGroupArchetype,
            //            accessibleDepartmentUserGroupArchetypeIds);
                  
            //        userGroupIds = userGroupAccess.UserGroupIds;
            //        multiUserGroupFilters = userGroupAccess.MultiUserGroupFilters;

            //        if (!userGroupAccess.AllowAccess)
            //        {
            //            return (false, userGroupIds, multiUserGroupFilters);
            //        }

            //    }
            //    else
            //    {
            //        var userGroupAccess = GetAccessibleUserGroupIdsFromConfiguration(ownerId, customerIds,
            //            executorUserId, accessibleDepartmentIds, multiUserGroupFilters,
            //            isAccessibleOnAllOwnedUserGroupArchetype, accessibleOwnedUserGroupArchetypeIds,
            //            isAccessibleOnAllDepartmentUserGroupArchetype, accessibleDepartmentUserGroupArchetypeIds);
                
            //        multiUserGroupFilters = userGroupAccess.MultiUserGroupFilters;
                   
            //        if (!userGroupAccess.AllowAccess)
            //           return (false, userGroupIds, multiUserGroupFilters);
            //    }
            //}

            return (true, userGroupIds, multiUserGroupFilters);
        }
        private async Task<(bool AllowAccess, List<int> UserGroupIds, List<List<int>> MultiUserGroupFilters)> GetAccessibleUserGroupIdsAsync(int ownerId, List<int> customerIds, List<int> userGroupIds,
           List<List<int>> multiUserGroupFilters, AccessSettingElement accessSetting, int executorUserId,
           List<int> accessibleDepartmentIds)
        {
            //var isAccessibleOnAllOwnedUserGroupArchetype =
            //    AccessSettingElement.ContainsAllSymbol(accessSetting.InOwnedUserGroupArchetypes);
            //var isAccessibleOnAllDepartmentUserGroupArchetype =
            //    AccessSettingElement.ContainsAllSymbol(accessSetting.InDepartmentUserGroupArchetypes);

            //if (!isAccessibleOnAllOwnedUserGroupArchetype
            //    || !isAccessibleOnAllDepartmentUserGroupArchetype)
            //{
            //    var accessibleOwnedUserGroupArchetypeIds = accessSetting.InOwnedUserGroupArchetypes
            //        .Select(DomainHelper.ParseToArchetype)
            //        .Where(archetype => archetype != ArchetypeEnum.Unknown).Select(a => (int)a).ToList();

            //    var accessibleDepartmentUserGroupArchetypeIds = accessSetting.InDepartmentUserGroupArchetypes
            //        .Select(DomainHelper.ParseToArchetype)
            //        .Where(archetype => archetype != ArchetypeEnum.Unknown).Select(a => (int)a).ToList();

            //    List<int> givenUserGroupIds = new List<int>();
            //    if (userGroupIds != null)
            //    {
            //        givenUserGroupIds.AddRange(userGroupIds);
            //    }

            //    if (multiUserGroupFilters != null)
            //    {
            //        foreach (var userGroupFilters in multiUserGroupFilters)
            //        {
            //            if (userGroupFilters != null)
            //            {
            //                givenUserGroupIds.AddRange(userGroupFilters);
            //            }
            //        }
            //    }

            //    if (!givenUserGroupIds.IsNullOrEmpty())
            //    {
            //        var userGroupAccess = await GetAccessibleUserGroupIdsFromArgumentAsync(ownerId, customerIds, executorUserId,
            //            givenUserGroupIds,
            //            userGroupIds, multiUserGroupFilters,
            //            accessibleDepartmentIds, isAccessibleOnAllOwnedUserGroupArchetype,
            //            accessibleOwnedUserGroupArchetypeIds, isAccessibleOnAllDepartmentUserGroupArchetype,
            //            accessibleDepartmentUserGroupArchetypeIds);

            //        userGroupIds = userGroupAccess.UserGroupIds;
            //        multiUserGroupFilters = userGroupAccess.MultiUserGroupFilters;

            //        if (!userGroupAccess.AllowAccess)
            //        {
            //            return (false, userGroupIds, multiUserGroupFilters);
            //        }

            //    }
            //    else
            //    {
            //        var userGroupAccess = await GetAccessibleUserGroupIdsFromConfigurationAsync(ownerId, customerIds,
            //            executorUserId, accessibleDepartmentIds, multiUserGroupFilters,
            //            isAccessibleOnAllOwnedUserGroupArchetype, accessibleOwnedUserGroupArchetypeIds,
            //            isAccessibleOnAllDepartmentUserGroupArchetype, accessibleDepartmentUserGroupArchetypeIds);

            //        multiUserGroupFilters = userGroupAccess.MultiUserGroupFilters;

            //        if (!userGroupAccess.AllowAccess)
            //            return (false, userGroupIds, multiUserGroupFilters);
            //    }
            //}

            return (true, userGroupIds, multiUserGroupFilters);
        }


        private (bool AllowAccess, List<List<int>> MultiUserGroupFilters) GetAccessibleUserGroupIdsFromConfiguration(
            int ownerId, List<int> customerIds,
            int executorUserId, List<int> accessibleDepartmentIds, List<List<int>> multiUserGroupFilters,
            bool isAccessibleOnAllOwnedUserGroupArchetype,
            List<int> accessibleOwnedUserGroupArchetypeIds, bool isAccessibleOnAllDepartmentUserGroupArchetype,
            List<int> accessibleDepartmentUserGroupArchetypeIds)
        {
            var configuredMultiUserGroupFilters = new List<List<int>>();
            if (!isAccessibleOnAllOwnedUserGroupArchetype)
            {
                var validUserGroupOfExecutor = accessibleOwnedUserGroupArchetypeIds.Count == 0
                    ? new List<UserGroupEntity>()
                    : _userGroupRepository.GetUserGroups(ownerId, customerIds,
                        parentUserIds: new List<int> {executorUserId},
                        archetypeIds: accessibleOwnedUserGroupArchetypeIds).Items;
                if (validUserGroupOfExecutor.Count == 0)
                {
                    return (false, multiUserGroupFilters);
                }

                configuredMultiUserGroupFilters.Add(validUserGroupOfExecutor.Select(ug => ug.UserGroupId).ToList());
            }

            if (!isAccessibleOnAllDepartmentUserGroupArchetype)
            {
                var validUserGroupOfDepartment = accessibleDepartmentUserGroupArchetypeIds.Count == 0
                    ? new List<UserGroupEntity>()
                    : _userGroupRepository.GetUserGroups(ownerId, customerIds,
                        parentDepartmentIds: accessibleDepartmentIds,
                        archetypeIds: accessibleDepartmentUserGroupArchetypeIds).Items;
                if (validUserGroupOfDepartment.Count == 0)
                {
                    return (false, multiUserGroupFilters);

                }

                configuredMultiUserGroupFilters.Add(validUserGroupOfDepartment.Select(ug => ug.UserGroupId).ToList());
            }

            multiUserGroupFilters = configuredMultiUserGroupFilters;
            return (true, multiUserGroupFilters);

        }
        private async Task< (bool AllowAccess, List<List<int>> MultiUserGroupFilters)> GetAccessibleUserGroupIdsFromConfigurationAsync(
           int ownerId, List<int> customerIds,
           int executorUserId, List<int> accessibleDepartmentIds, List<List<int>> multiUserGroupFilters,
           bool isAccessibleOnAllOwnedUserGroupArchetype,
           List<int> accessibleOwnedUserGroupArchetypeIds, bool isAccessibleOnAllDepartmentUserGroupArchetype,
           List<int> accessibleDepartmentUserGroupArchetypeIds)
        {
            var configuredMultiUserGroupFilters = new List<List<int>>();
            if (!isAccessibleOnAllOwnedUserGroupArchetype)
            {
                var validUserGroupOfExecutor = accessibleOwnedUserGroupArchetypeIds.Count == 0
                    ? new List<UserGroupEntity>()
                    : (await _userGroupRepository.GetUserGroupsAsync(ownerId, customerIds,
                        parentUserIds: new List<int> {executorUserId},
                        archetypeIds: accessibleOwnedUserGroupArchetypeIds)).Items;

                if (validUserGroupOfExecutor.Count == 0)
                {
                    return (false, multiUserGroupFilters);
                }

                configuredMultiUserGroupFilters.Add(validUserGroupOfExecutor.Select(ug => ug.UserGroupId).ToList());
            }

            if (!isAccessibleOnAllDepartmentUserGroupArchetype)
            {
                var validUserGroupOfDepartment = accessibleDepartmentUserGroupArchetypeIds.Count == 0
                    ? new List<UserGroupEntity>()
                    : (await _userGroupRepository.GetUserGroupsAsync(ownerId, customerIds,
                        parentDepartmentIds: accessibleDepartmentIds,
                        archetypeIds: accessibleDepartmentUserGroupArchetypeIds)).Items;
                if (validUserGroupOfDepartment.Count == 0)
                {
                    return (false, multiUserGroupFilters);

                }

                configuredMultiUserGroupFilters.Add(validUserGroupOfDepartment.Select(ug => ug.UserGroupId).ToList());
            }

            multiUserGroupFilters = configuredMultiUserGroupFilters;
            return (true, multiUserGroupFilters);

        }
        private (bool AllowAccess, List<int> UserGroupIds, List<List<int>> MultiUserGroupFilters) GetAccessibleUserGroupIdsFromArgument(int ownerId, List<int> customerIds, int executorUserId,
                List<int> allGivenUserGroupIds, List<int> userGroupIds,
                List<List<int>> multiUserGroupFilters, List<int> accessibleDepartmentIds,
                bool isAccessibleOnAllOwnedUserGroupArchetype, List<int> accessibleOwnedUserGroupArchetypeIds,
                bool isAccessibleOnAllDepartmentUserGroupArchetype, List<int> accessibleDepartmentUserGroupArchetypeIds)
        {
            var existingUserGroups = _userGroupRepository
                .GetUserGroups(ownerId, customerIds, userGroupIds: allGivenUserGroupIds).Items;

            if (!userGroupIds.IsNullOrEmpty())
            {
                var validUserGroupIds = GetValidUserGroupIds(userGroupIds, existingUserGroups,
                    accessibleDepartmentIds, executorUserId, isAccessibleOnAllOwnedUserGroupArchetype,
                    accessibleOwnedUserGroupArchetypeIds, isAccessibleOnAllDepartmentUserGroupArchetype,
                    accessibleDepartmentUserGroupArchetypeIds);
                if (validUserGroupIds.Count == 0)
                {
                    return (false, userGroupIds, multiUserGroupFilters);
                }

                userGroupIds = validUserGroupIds;
            }

            if (multiUserGroupFilters != null)
            {
                for (int i = 0; i < multiUserGroupFilters.Count; i++)
                {
                    var userGroupFilters = multiUserGroupFilters[i];
                    if (!userGroupFilters.IsNullOrEmpty())
                    {
                        List<int> validUserGroupIds = GetValidUserGroupIds(userGroupFilters, existingUserGroups,
                            accessibleDepartmentIds, executorUserId, isAccessibleOnAllOwnedUserGroupArchetype,
                            accessibleOwnedUserGroupArchetypeIds, isAccessibleOnAllDepartmentUserGroupArchetype,
                            accessibleDepartmentUserGroupArchetypeIds);
                        if (validUserGroupIds.Count == 0)
                        {
                            return (false, userGroupIds, multiUserGroupFilters);

                        }

                        multiUserGroupFilters[i] = validUserGroupIds;
                    }
                }
            }

            return (true, userGroupIds, multiUserGroupFilters);
        }
        private async Task<(bool AllowAccess, List<int> UserGroupIds, List<List<int>> MultiUserGroupFilters)> GetAccessibleUserGroupIdsFromArgumentAsync(int ownerId, List<int> customerIds, int executorUserId,
               List<int> allGivenUserGroupIds, List<int> userGroupIds,
               List<List<int>> multiUserGroupFilters, List<int> accessibleDepartmentIds,
               bool isAccessibleOnAllOwnedUserGroupArchetype, List<int> accessibleOwnedUserGroupArchetypeIds,
               bool isAccessibleOnAllDepartmentUserGroupArchetype, List<int> accessibleDepartmentUserGroupArchetypeIds)
        {
            var existingUserGroups = (await _userGroupRepository
                .GetUserGroupsAsync(ownerId, customerIds, userGroupIds: allGivenUserGroupIds)).Items;

            if (!userGroupIds.IsNullOrEmpty())
            {
                var validUserGroupIds = GetValidUserGroupIds(userGroupIds, existingUserGroups,
                    accessibleDepartmentIds, executorUserId, isAccessibleOnAllOwnedUserGroupArchetype,
                    accessibleOwnedUserGroupArchetypeIds, isAccessibleOnAllDepartmentUserGroupArchetype,
                    accessibleDepartmentUserGroupArchetypeIds);
                if (validUserGroupIds.Count == 0)
                {
                    return (false, userGroupIds, multiUserGroupFilters);
                }

                userGroupIds = validUserGroupIds;
            }

            if (multiUserGroupFilters != null)
            {
                for (int i = 0; i < multiUserGroupFilters.Count; i++)
                {
                    var userGroupFilters = multiUserGroupFilters[i];
                    if (!userGroupFilters.IsNullOrEmpty())
                    {
                        List<int> validUserGroupIds = GetValidUserGroupIds(userGroupFilters, existingUserGroups,
                            accessibleDepartmentIds, executorUserId, isAccessibleOnAllOwnedUserGroupArchetype,
                            accessibleOwnedUserGroupArchetypeIds, isAccessibleOnAllDepartmentUserGroupArchetype,
                            accessibleDepartmentUserGroupArchetypeIds);
                        if (validUserGroupIds.Count == 0)
                        {
                            return (false, userGroupIds, multiUserGroupFilters);

                        }

                        multiUserGroupFilters[i] = validUserGroupIds;
                    }
                }
            }

            return (true, userGroupIds, multiUserGroupFilters);
        }
        private List<int> GetValidUserGroupIds(List<int> checkingUserGroupIds,
            List<UserGroupEntity> existingUserGroups,
            List<int> accessibleDepartmentIds,
            int executorUserId,
            bool isAccessibleOnAllOwnedUserGroupArchetype,
            List<int> accessibleOwnedUserGroupArchetypeIds,
            bool isAccessibleOnAllDepartmentUserGroupArchetype,
            List<int> accessibleDepartmentUserGroupArchetypeIds)
        {
            List<int> validUserGroupIds = new List<int>();
            foreach (var userGroupId in checkingUserGroupIds)
            {
                var existingUserGroup = existingUserGroups.FirstOrDefault(e => e.UserGroupId == userGroupId);
                if (existingUserGroup != null)
                {
                    var validUserGroup = false;
                    if (existingUserGroup.UserId == executorUserId)
                    {
                        if (isAccessibleOnAllOwnedUserGroupArchetype ||
                            (existingUserGroup.ArchetypeId != null &&
                             accessibleOwnedUserGroupArchetypeIds.Contains(existingUserGroup.ArchetypeId.Value)))
                        {
                            validUserGroup = true;
                        }
                    }
                    else if (accessibleDepartmentIds.Contains(existingUserGroup.DepartmentId ?? 0))
                    {
                        if (isAccessibleOnAllDepartmentUserGroupArchetype ||
                            (existingUserGroup.ArchetypeId != null &&
                             accessibleDepartmentUserGroupArchetypeIds.Contains(existingUserGroup.ArchetypeId.Value)))
                        {
                            validUserGroup = true;
                        }
                    }

                    if (validUserGroup)
                    {
                        validUserGroupIds.Add(userGroupId);
                    }
                }
            }

            return validUserGroupIds;
        }
     
    }
}
