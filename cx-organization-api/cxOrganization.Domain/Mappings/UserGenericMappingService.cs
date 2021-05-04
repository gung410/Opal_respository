using cxOrganization.Client.UserGroups;
using cxOrganization.Client.UserTypes;
using cxOrganization.Domain.Dtos.UserGroups;
using cxOrganization.Domain.Dtos.Users;
using cxOrganization.Domain.Entities;
using cxOrganization.Domain.Repositories;
using cxOrganization.Domain.Security.User;
using cxOrganization.Domain.Services;
using cxOrganization.Domain.Settings;
using cxPlatform.Client.ConexusBase;
using cxPlatform.Core;
using cxPlatform.Core.Security;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using cxOrganization.Domain.Extensions;
using cxOrganization.Domain.AdvancedWorkContext;

namespace cxOrganization.Domain.Mappings
{
    public class UserGenericMappingService : UserMappingService
    {
        private readonly IAdvancedWorkContext _workContext;
        private readonly IDepartmentRepository _departmentRepository;
        private readonly IUserRepository _userRepository;
        private readonly IOwnerRepository _ownerRepository;
        private readonly IUserCryptoService _userCryptoService;
        private List<ArchetypeEnum> UserTypeArcheTypes = new List<ArchetypeEnum> { ArchetypeEnum.SystemRole, ArchetypeEnum.Role, ArchetypeEnum.CareerPath, ArchetypeEnum.Level,
            ArchetypeEnum.PersonnelGroup, ArchetypeEnum.ExperienceCategory, ArchetypeEnum.DevelopmentalRole, ArchetypeEnum.LearningFramework };

        public UserGenericMappingService(IUserTypeRepository userTypeRepository,
            IAdvancedWorkContext workContext,
            IDepartmentRepository departmentRepository,
            IPropertyService propertyService,
            IUserRepository userRepository,
            IHierarchyDepartmentRepository hierarchyDepartmentRepository,
            IOwnerRepository ownerRepository,
            IUserTypeMappingService userTypeMappingService,
            ILogger<UserGenericMappingService> logger,
            IOptions<AppSettings> options,
            IUserCryptoService userCryptoService)
            : base(workContext, propertyService, userTypeMappingService, userCryptoService, userTypeRepository, options)
        {
            _workContext = workContext;
            _departmentRepository = departmentRepository;
            _userRepository = userRepository;
            _ownerRepository = ownerRepository;
            _userCryptoService = userCryptoService;
        }

        public override ConexusBaseDto ToUserDto(UserEntity user,
            bool? getDynamicProperties = null,
            bool keepEncryptedSsn = false, bool keepDecryptedSsn = false,
            List<UGMemberEntity> ugMemberEntities = null)
        {
            if (user == null)
            {
                return null;
            }

            var userDtoBase = new UserGenericDto
            {
                Identity = ToIdentityDto(user),
                EntityStatus = ToEntityStatusDto(user),
                DepartmentName = user.Department == null ? string.Empty : user.Department.Name,
                DepartmentAddress = user.Department == null ? string.Empty : (user.Department.Adress ?? string.Empty),
                //This use to solve the confuse from DB: not null column
                FirstName = string.IsNullOrEmpty(user.FirstName) ? null : user.FirstName,
                LastName = string.IsNullOrEmpty(user.LastName) ? null : user.LastName,
                DateOfBirth = ShouldHideDateOfBirth() ? null : user.DateOfBirth,
                EmailAddress = string.IsNullOrEmpty(user.Email) ? null : user.Email,
                MobileNumber = string.IsNullOrEmpty(user.Mobile) ? null : user.Mobile,
                MobileCountryCode = user.CountryCode,
                SSN = user.SSN,
                Password = null,
                DepartmentId = user.DepartmentId,
                Gender = user.Gender,
                Tag = user.Tag,
                Created = user.Created,
                ForceLoginAgain = user.ForceUserLoginAgain,
                Groups = ExtractParentUserGroupDtos(user, ugMemberEntities, user.CustomerId),
                OwnGroups = user.UserGroups != null
                    ? user.UserGroups.Where(ug => ug.EntityStatusId == (int)EntityStatusEnum.Active)
                        .Select(ug => ToUserGroupDto(ug, user.CustomerId)).ToList()
                    : null,
                LoginServiceClaims =
                    user.LoginServiceUsers == null ? null : ToLoginServiceClaims(user.LoginServiceUsers)
            };

            MapSsnToUserDto(userDtoBase, user, keepEncryptedSsn, keepDecryptedSsn);

            if (user.DynamicAttributes != null)
            {
                userDtoBase.JsonDynamicAttributes =
                    new Dictionary<string, dynamic>(StringComparer.CurrentCultureIgnoreCase);
                //PopulateObject to map data to defined ignore case dictionary
                JsonConvert.PopulateObject(user.DynamicAttributes, userDtoBase.JsonDynamicAttributes);
            }

            if (string.IsNullOrEmpty(user.Mobile))
            {
                userDtoBase.MobileNumber = null;
            }

            var roles = ExtractToUserTypeDtos(user);
            if (roles != null)
            {
                userDtoBase.CustomData = new Dictionary<string, object>(StringComparer.CurrentCultureIgnoreCase);
                foreach (var archetype in UserTypeArcheTypes)
                {
                    var userTypeByArchetypes = roles.Where(x => x.Identity.Archetype == archetype).ToList();

                    if (userTypeByArchetypes.Any())
                    {
                        if (archetype == ArchetypeEnum.Role)
                        {
                            userDtoBase.Roles = userTypeByArchetypes;
                        }
                        else
                        {
                            var customDataKey = BuildCustomDataKey(archetype);
                            userDtoBase.CustomData.Add(customDataKey, userTypeByArchetypes);
                        }
                    }
                }
            }

            return userDtoBase;
        }

        private string BuildCustomDataKey(ArchetypeEnum archetype)
        {
            var propertyName = archetype.ToString().ToCharArray();
            propertyName[0] = Char.ToLower(propertyName[0]);
            return $"{new string(propertyName)}s";
        }

        private bool IsApprovalGroup(UserGroupEntity userGroup)
        {
            return userGroup.ArchetypeId == (int)ArchetypeEnum.ApprovalGroup;
        }

        private List<UserGroupDto> ExtractParentUserGroupDtos(UserEntity user, List<UGMemberEntity> ugMemberEntities, int? customerId)
        {
            if (ugMemberEntities != null)
            {
                return ugMemberEntities
                    .Where(u => u.UserId == user.UserId && u.EntityStatusId == (int)EntityStatusEnum.Active &&
                                u.UserGroup?.EntityStatusId == (int)EntityStatusEnum.Active)
                    .Select(ugm => ToUserGroupDto(ugm.UserGroup, customerId)).ToList();
            }

            return user.UGMembers.Any()
                ? user.UGMembers.Where(x =>
                        x.EntityStatusId == (int)EntityStatusEnum.Active &&
                        x.UserGroup?.EntityStatusId == (int)EntityStatusEnum.Active)
                    .Select(x => ToUserGroupDto(x.UserGroup, user.CustomerId)).ToList()
                : null;
        }

        private UserGroupDto ToUserGroupDto(UserGroupEntity userGroup, int? customerId)
        {
            var userGroupDto = new UserGroupDto
            {
                Identity = new IdentityDto
                {
                    Archetype = userGroup.ArchetypeId.HasValue
                        ? (ArchetypeEnum)userGroup.ArchetypeId
                        : ArchetypeEnum.Unknown,
                    ExtId = userGroup.ExtId,
                    Id = userGroup.UserGroupId,
                    CustomerId = customerId.GetValueOrDefault(),
                    OwnerId = userGroup.OwnerId
                },
                UserIdentity = new IdentityDto
                {
                    Id = userGroup.UserId
                },
                Type = (GrouptypeEnum)(userGroup.UserGroupTypeId.HasValue ? userGroup.UserGroupTypeId.Value : 1),
                Name = userGroup.Name,
                Description = userGroup.Description,
                DepartmentId = userGroup.DepartmentId,
                EntityStatus = new EntityStatusDto
                {
                    LastExternallySynchronized = userGroup.LastSynchronized,
                    StatusReasonId = userGroup.EntityStatusReasonId.HasValue
                        ? (EntityStatusReasonEnum)userGroup.EntityStatusReasonId
                        : EntityStatusReasonEnum.Unknown,
                    StatusId = userGroup.EntityStatusId.HasValue
                        ? (EntityStatusEnum)userGroup.EntityStatusId
                        : EntityStatusEnum.Unknown,
                    LastUpdatedBy = userGroup.LastUpdatedBy.HasValue ? userGroup.LastUpdatedBy.Value : 0,
                    ExternallyMastered = false
                }
            };

            if (userGroup.User != null)
            {
                userGroupDto.UserIdentity.Archetype = (ArchetypeEnum)(userGroup.User.ArchetypeId ?? 0);
                userGroupDto.UserIdentity.ExtId = userGroup.User.ExtId;
                userGroupDto.UserIdentity.CustomerId = userGroup.User.CustomerId ?? userGroupDto.Identity.CustomerId;
                userGroupDto.UserIdentity.OwnerId = userGroup.User.OwnerId;

                if (IsApprovalGroup(userGroup))
                    userGroupDto.Name = userGroup.User.GetFullName();
            }
            return userGroupDto;
        }

        public override UserEntity ToUserEntity(DepartmentEntity parentDepartment,
            HierarchyDepartmentEntity parentHd,
            UserEntity userEntity,
            UserDtoBase userDto,
            bool skipCheckingEntityVersion = false,
            int? currentOwnerId = null)
        {
            var userGeneric = (UserGenericDto)userDto;
            //Initial  entity
            userGeneric.EntityStatus.LastUpdatedBy = userGeneric.EntityStatus.LastUpdatedBy > 0
                ? userGeneric.EntityStatus.LastUpdatedBy
                : _workContext.CurrentUserId;

            if (userDto.Identity.Id == 0)
            {
                return NewUserEntity(parentHd, userGeneric, currentOwnerId);
            }

            //Entity is existed
            if (userEntity == null)
                return null;
            if (!skipCheckingEntityVersion)
            {
                CheckEntityVersion(userGeneric.EntityStatus.EntityVersion, userEntity.EntityVersion);
            }

            var oldStatus = userEntity.EntityStatusId;
            var oldStatusReason = userEntity.EntityStatusReasonId;

            userEntity.FirstName = userGeneric.FirstName ?? string.Empty;
            userEntity.LastName = userGeneric.LastName ?? string.Empty;
            userEntity.DateOfBirth = userGeneric.DateOfBirth ?? userEntity.DateOfBirth;
            userEntity.Email = userGeneric.EmailAddress?.ToLower() ?? string.Empty;
            userEntity.Mobile = userGeneric.MobileNumber ?? string.Empty;
            userEntity.CountryCode = userGeneric.MobileCountryCode;
            userEntity.ExtId = userGeneric.Identity.ExtId ?? string.Empty;
            if (userGeneric.SSN != null)
            {
                userEntity.SSN = _userCryptoService.EncryptSSN(userGeneric.SSN);
                userEntity.SSNHash = _userCryptoService.ComputeHashSsn(userDto.SSN);
            }

            userEntity.Gender = userGeneric.Gender;
            userEntity.EntityExpirationDate = userGeneric.EntityStatus.ExpirationDate;

            // prevent change status from active to new
            bool isChangeStatusFromActiveToNew = userEntity.EntityStatusId == (int)EntityStatusEnum.Active &&
                                                 userGeneric.EntityStatus.StatusId == EntityStatusEnum.New;
            if (!isChangeStatusFromActiveToNew)
                userEntity.EntityStatusId = (short)userGeneric.EntityStatus.StatusId;

            userEntity.EntityStatusReasonId =
                Enum.IsDefined(typeof(EntityStatusReasonEnum), userDto.EntityStatus.StatusReasonId)
                    ? (int)userDto.EntityStatus.StatusReasonId
                    : userEntity.EntityStatusReasonId;
            userEntity.Deleted = userGeneric.EntityStatus.Deleted ? DateTime.UtcNow : default(DateTime?);
            userEntity.LastUpdated = DateTime.Now;
            userEntity.Tag = userGeneric.Tag ?? string.Empty;
            userEntity.LastUpdatedBy = userDto.EntityStatus.LastUpdatedBy > 0
                ? userDto.EntityStatus.LastUpdatedBy
                : _workContext.CurrentUserId;
            userEntity.EntityActiveDate = userDto.EntityStatus.ActiveDate;

            var parentDepartmentInDb = _departmentRepository.GetById(userEntity.DepartmentId);
            if (parentDepartmentInDb != null)
            {
                userEntity.DepartmentId = userGeneric.DepartmentId;
            }

            //Set default country code
            if (!userGeneric.MobileCountryCode.HasValue || userGeneric.MobileCountryCode <= 0)
            {
                var department = _departmentRepository.GetById(userEntity.DepartmentId);
                userEntity.CountryCode = department.CountryCode;
            }

            if (userGeneric.EntityStatus.LastExternallySynchronized.HasValue)
            {
                userEntity.LastSynchronized = userGeneric.EntityStatus.LastExternallySynchronized.Value;
            }

            userEntity.Locked = (short)(userGeneric.EntityStatus.ExternallyMastered ? 1 : 0);

            // don't allow to change user finishOnBoarding Flag value when user have already finish onboarding step
            if (userGeneric.EntityStatus.StatusId != EntityStatusEnum.Active
                && !string.IsNullOrEmpty(userEntity.DynamicAttributes)
                && userGeneric.JsonDynamicAttributes != null)
            {
                var jsonDynamic =
                    JsonConvert.DeserializeObject<IDictionary<string, dynamic>>(userEntity.DynamicAttributes);
                if (jsonDynamic.TryGetValue(UserJsonDynamicAttributeName.FinishOnBoarding, out var value) &&
                    value is bool && value == true)
                {
                    userGeneric.AddOrUpdateJsonProperty(UserJsonDynamicAttributeName.FinishOnBoarding, value);
                }
            }

            // Get status before unlock
            var statusBeforeChangeStatus = userGeneric.GetJsonPropertyValue(UserJsonDynamicAttributeName.LastEntityStatusId);
            var isFinishOnBoarding = userGeneric.GetJsonPropertyValue(UserJsonDynamicAttributeName.FinishOnBoarding);

            // Add latest status when status changed
            bool isUserStatusChanged = oldStatus.Value != (int)userGeneric.EntityStatus.StatusId;

            // If Unlock/Activate the user has status before is New. Then must return New status to that user
            if (oldStatus.HasValue 
                && isUserStatusChanged
                && (oldStatus.Value == (int) EntityStatusEnum.IdentityServerLocked || oldStatus.Value == (int)EntityStatusEnum.Inactive) 
                && userEntity.EntityStatusId != (int)EntityStatusEnum.Archived
                && (statusBeforeChangeStatus == (int)EntityStatusEnum.New || isFinishOnBoarding == false))
            {
                userEntity.EntityStatusId = (int)EntityStatusEnum.New;
                userEntity.EntityStatusReasonId = (int) EntityStatusReasonEnum.Unknown;
            }

            if (isUserStatusChanged)
            {
                userGeneric.AddOrUpdateJsonProperty(UserJsonDynamicAttributeName.LastEntityStatusId, oldStatus);
                userGeneric.AddOrUpdateJsonProperty(UserJsonDynamicAttributeName.LastEntityStatusReasonId, oldStatusReason);
            }

            userGeneric.AddOrUpdateJsonProperty(UserJsonDynamicAttributeName.ExpirationDate, userGeneric.EntityStatus.ExpirationDate);
            userEntity.DynamicAttributes = userGeneric.JsonDynamicAttributes == null
                ? null
                : JsonConvert.SerializeObject(userGeneric.JsonDynamicAttributes);

            if (userGeneric.ForceLoginAgain.HasValue)
            {
                userEntity.ForceUserLoginAgain = userGeneric.ForceLoginAgain.Value;
            }

            foreach (ArchetypeEnum userTypeArchetype in UserTypeArcheTypes)
            {
                List<UserTypeDto> receivedUserTypes = null;

                if (userTypeArchetype == ArchetypeEnum.Role)
                {
                    receivedUserTypes = userGeneric.Roles;
                }
                else if (userGeneric.CustomData != null && userGeneric.CustomData.Any())
                {
                    var customDataKey = BuildCustomDataKey(userTypeArchetype);
                    if (userDto.CustomData.ContainsKey(customDataKey))
                    {
                        var jArray = userDto.CustomData[customDataKey] as JArray;
                        if (jArray == null) continue;

                        receivedUserTypes = jArray.ToObject<List<UserTypeDto>>();
                    }
                }

                if (receivedUserTypes == null) continue;

                //Need to check archetype of input user-type to make sure client does not lack to put an archetype usertype into another custom data key
                var validReceivedUserTypes = receivedUserTypes
                    .Where(x => x.Identity.Archetype == userTypeArchetype)
                    .ToList();

                var receiveUserTypeIds = validReceivedUserTypes
                    .Select(x => (int)x.Identity.Id)
                    .ToList();

                var currentUserTypeIds = userEntity.UT_Us
                    .Where(u => u.UserType?.ArchetypeId == (int)userTypeArchetype)
                    .Select(x => x.UserTypeId).ToList();

                foreach (var validReceivedUserType in validReceivedUserTypes)
                {
                    if (!currentUserTypeIds.Contains((int)validReceivedUserType.Identity.Id))
                    {
                        var nonExistingUserType = !string.IsNullOrEmpty(validReceivedUserType.Identity.ExtId)
                            ? UserTypeEntities.FirstOrDefault(x => x.ExtId == validReceivedUserType.Identity.ExtId)
                            : UserTypeEntities.FirstOrDefault(x =>
                                x.UserTypeId == (int)validReceivedUserType.Identity.Id);
                        if (nonExistingUserType != null)
                        {
                            userEntity.UT_Us.Add(new UTUEntity
                            { UserTypeId = nonExistingUserType.UserTypeId, UserId = userEntity.UserId });
                        }
                    }
                }

                foreach (var currentUserTypeId in currentUserTypeIds)
                {
                    if (!receiveUserTypeIds.Contains(currentUserTypeId))
                    {
                        var removeUt = userEntity.UT_Us.FirstOrDefault(x => x.UserTypeId == currentUserTypeId);
                        userEntity.UT_Us.Remove(removeUt);
                    }
                }

            }
            return userEntity;
        }

        private UserEntity NewUserEntity(HierarchyDepartmentEntity parentHd, UserGenericDto userDto, int? currentOwnerId = null)
        {

            var userNew = new UserEntity
            {
                UserName = GenerateUserName(userDto, currentOwnerId),
                FirstName = userDto.FirstName ?? string.Empty,
                LastName = userDto.LastName ?? string.Empty,
                Email = userDto.EmailAddress?.ToLower() ?? string.Empty,
                Mobile = userDto.MobileNumber ?? string.Empty,
                CountryCode = userDto.MobileCountryCode,
                ExtId = userDto.Identity.ExtId ?? string.Empty,
                DepartmentId = userDto.DepartmentId,
                Tag = userDto.Tag ?? string.Empty,
                SaltPassword = string.Empty,
                HashPassword = string.Empty,
                Created = DateTime.Now,
                LastUpdated = DateTime.Now,
                LastUpdatedBy = userDto.EntityStatus.LastUpdatedBy,
                EntityStatusId = (int)userDto.EntityStatus.StatusId,
                EntityStatusReasonId = (int)userDto.EntityStatus.StatusReasonId,
                OwnerId = userDto.Identity.OwnerId,
                CustomerId = userDto.Identity.CustomerId,
                ArchetypeId = (int)ArchetypeEnum.Employee,
                DateOfBirth = userDto.DateOfBirth,
                Gender = userDto.Gender,
                EntityExpirationDate = userDto.EntityStatus.ExpirationDate,
                EntityActiveDate = userDto.EntityStatus.ActiveDate,
                ForceUserLoginAgain = userDto.ForceLoginAgain ?? false,
                DynamicAttributes = userDto.JsonDynamicAttributes == null ? null : JsonConvert.SerializeObject(userDto.JsonDynamicAttributes),
                Department = _departmentRepository.GetById(userDto.DepartmentId)
            };
            if (userDto.SSN != null)
            {
                userNew.SSN = _userCryptoService.EncryptSSN(userDto.SSN);
                userNew.SSNHash = _userCryptoService.ComputeHashSsn(userDto.SSN);
            }
            else
            {
                userNew.SSN = string.Empty;
            }
            var defaultUserType = UserTypeEntities.FirstOrDefault(u => u.UserTypeId == UserTypeId);
            if (defaultUserType != null)
            {
                userNew.UT_Us.Add(new UTUEntity { UserTypeId = defaultUserType.UserTypeId });
            }

            if (userDto.EntityStatus.LastExternallySynchronized.HasValue)
            {
                userNew.LastSynchronized = userDto.EntityStatus.LastExternallySynchronized.Value;
            }
            userNew.Locked = (short)(userDto.EntityStatus.ExternallyMastered ? 1 : 0);

            if (parentHd != null && parentHd.Department != null)
            {
                //Set default customer
                if (!userNew.CustomerId.HasValue && parentHd.Department.CustomerId > 0)
                {
                    userNew.CustomerId = parentHd.Department.CustomerId;
                }
                //Set default language
                if (userNew.LanguageId <= 0)
                {
                    userNew.LanguageId = parentHd.Department.LanguageId;
                }
                //Set default country code
                if (!userNew.CountryCode.HasValue || userNew.CountryCode <= 0)
                {
                    userNew.CountryCode = parentHd.Department.CountryCode;
                }
            }

            var currentOwner = _ownerRepository.GetById(currentOwnerId ?? _workContext.CurrentOwnerId);
            //Handle password
            if (currentOwner.UseOTP)
            {
                userNew.Password = string.IsNullOrEmpty(userDto.Password)
                    ? Encryption.Encrypt(Generator.GenerateRandomPassword(currentOwner.OTPLength, "ABCDEFGHJKLMNPQRSTWXZ23456789", false, true, bIncludeNumbers: false))
                    : Encryption.Encrypt(userDto.Password);
                userNew.OneTimePassword = Encryption.Encrypt(userDto.Password);
            }
            else
            {
                userNew.Password = string.IsNullOrEmpty(userDto.Password)
                    ? Encryption.Encrypt(Generator.GenerateRandomPassword(currentOwner.OTPLength, "ABCDEFGHJKLMNPQRSTWXZ23456789", false, true, bIncludeNumbers: false))
                    : Encryption.Encrypt(userDto.Password);
                userNew.OneTimePassword = Encryption.Encrypt(string.Empty);
            }
            if (userDto.CustomData.Any())
            {
                List<UTUEntity> utus = new List<UTUEntity>();
                foreach (ArchetypeEnum userTypeArchetype in UserTypeArcheTypes)
                {
                    var customDataKey = BuildCustomDataKey(userTypeArchetype);
                    if (userDto.CustomData.ContainsKey(customDataKey))
                    {
                        var jArray = userDto.CustomData[customDataKey] as JArray;
                        if (jArray == null) continue;

                        var receivedUserTypes = jArray.ToObject<List<UserTypeDto>>();
                        if (receivedUserTypes == null) continue;

                        //Need to check archetype of input user-type to make sure client does not lack to put an archetype usertype into another custom data key
                        var validReceivedUserTypes = receivedUserTypes
                                .Where(x => x.Identity.Archetype == userTypeArchetype)
                                .ToList();

                        foreach (var ut in validReceivedUserTypes)
                        {
                            UserTypeEntity userType;
                            if (!string.IsNullOrEmpty(ut.Identity.ExtId))
                                userType = UserTypeEntities.FirstOrDefault(x => x.ExtId == ut.Identity.ExtId);
                            else
                                userType = UserTypeEntities.FirstOrDefault(x => x.UserTypeId == (int)ut.Identity.Id);
                            if (userType != null)
                            {
                                utus.Add(new UTUEntity { UserTypeId = (int)userType.UserTypeId, UserId = userNew.UserId });
                            }
                        }
                    }

                }
                userNew.UT_Us = utus;

            }
            return userNew;
        }

        private string GenerateUserName(UserGenericDto empDto, int? currentOwnerId = null)
        {
            //Take from email
            if (!string.IsNullOrEmpty(empDto.EmailAddress))
            {
                var usersByUserName = _userRepository.GetUsersByUsername(currentOwnerId ?? _workContext.CurrentOwnerId, empDto.EmailAddress, EntityStatusEnum.All);
                if (!usersByUserName.Any())
                {
                    return empDto.EmailAddress;
                }
            }
            if (!empDto.SkipGenerateUserName)
            {
                //Using 4 first letters in firstname and 4 first letters in lastname
                try
                {
                    var fourFirstLettersOfFirstName = empDto.FirstName;
                    if (!string.IsNullOrEmpty(empDto.FirstName) && empDto.FirstName.Length >= 4)
                    {
                        fourFirstLettersOfFirstName = empDto.FirstName.Substring(0, 4);
                    }
                    var fourFirstLettersOfLastName = empDto.LastName;
                    if (!string.IsNullOrEmpty(empDto.LastName) && empDto.LastName.Length >= 4)
                    {
                        fourFirstLettersOfLastName = empDto.LastName.Substring(0, 4);
                    }
                    var username = RemoveSpecialCharactersAndSpaces(string.Format("{0}{1}", fourFirstLettersOfFirstName, fourFirstLettersOfLastName));
                    if (!string.IsNullOrEmpty(username))
                    {
                        var usersByUserName = _userRepository.GetUsersByUsername(currentOwnerId ?? _workContext.CurrentOwnerId, username, EntityStatusEnum.All);
                        if (!usersByUserName.Any())
                        {
                            return username;
                        }
                    }
                }
                catch (Exception exception)
                {
                    throw exception;
                }
            }
            //Generate a GUID as username
            return Guid.NewGuid().ToString();
        }
    }
}

