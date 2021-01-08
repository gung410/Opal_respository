using cxOrganization.Client.UserGroups;
using cxOrganization.Client.UserTypes;
using cxOrganization.Domain.Dtos.UserGroups;
using cxOrganization.Domain.Dtos.Users;
using cxOrganization.Domain.Entities;
using cxOrganization.Domain.Repositories;
using cxOrganization.Domain.Security.User;
using cxOrganization.Domain.Services;
using cxPlatform.Client.ConexusBase;
using cxPlatform.Core;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using cxOrganization.Client.DepartmentTypes;
using cxOrganization.Domain.Extensions;
using cxOrganization.Domain.Settings;
using Microsoft.Extensions.Options;

namespace cxOrganization.Domain.Mappings
{
    public class UserInfoMappingService : IUserWithIdpInfoMappingService
    {
        private readonly IUserTypeRepository _userTypeRepository;
        private readonly IDepartmentTypeRepository _departmentTypeRepository;
        private readonly IWorkContext _workContext;
        private readonly IDepartmentRepository _departmentRepository;
        private readonly IPropertyService _propertyService;
        private readonly IUserRepository _userRepository;
        private readonly IUserTypeMappingService _userTypeMappingService;
        private readonly IDepartmentTypeMappingService _departmentTypeMappingService;
        private readonly IHierarchyDepartmentRepository _hierarchyDepartmentRepository;
        private readonly IOwnerRepository _ownerRepository;
        private readonly ILogger<UserGenericMappingService> _logger;
        private readonly IUserCryptoService _userCryptoService;
        private List<UserTypeEntity> _userTypeEntities;
        private List<DepartmentTypeEntity> _departmentTypeEntities;

        private List<ArchetypeEnum> UserTypeArcheTypes = new List<ArchetypeEnum> { ArchetypeEnum.SystemRole, ArchetypeEnum.Role, ArchetypeEnum.CareerPath, ArchetypeEnum.Level, ArchetypeEnum.PersonnelGroup, ArchetypeEnum.ExperienceCategory };
        private readonly AppSettings _appSettings;
        public UserInfoMappingService(IUserTypeRepository userTypeRepository,
            IWorkContext workContext,
            IDepartmentRepository departmentRepository,
            IPropertyService propertyService,
            IUserRepository userRepository,
            IHierarchyDepartmentRepository hierarchyDepartmentRepository,
            IOwnerRepository ownerRepository,
            IUserTypeMappingService userTypeMappingService, 
            ILogger<UserGenericMappingService> logger, 
            IUserMappingService userMappingService, 
            IDepartmentTypeMappingService departmentTypeMappingService,
            IUserCryptoService userCryptoService,
            IDepartmentTypeRepository departmentTypeRepository,
            IOptions<AppSettings> appSettingOption)
        {
            _userTypeRepository = userTypeRepository;
            _workContext = workContext;
            _departmentRepository = departmentRepository;
            _propertyService = propertyService;
            _userRepository = userRepository;
            _hierarchyDepartmentRepository = hierarchyDepartmentRepository;
            _ownerRepository = ownerRepository;
            _logger = logger;
            //_userTypeEntities = 
            _userTypeMappingService = userTypeMappingService;
            _departmentTypeMappingService = departmentTypeMappingService;
            _userCryptoService = userCryptoService;
            _departmentTypeRepository = departmentTypeRepository;
            _appSettings = appSettingOption.Value;
        }
        protected List<UserTypeEntity> UserTypeEntities
        {
            get
            {
                if (_userTypeEntities == null)
                    _userTypeEntities = _userTypeRepository.GetAllUserTypesInCache();
                return _userTypeEntities;
            }
        }
        protected List<DepartmentTypeEntity> DepartmentTypeEntities
        {
            get
            {
                if (_departmentTypeEntities == null)
                    _departmentTypeEntities = _departmentTypeRepository.GetAllDepartmentTypesInCache();
                return _departmentTypeEntities;
            }
        }

        public UserWithIdpInfoDto ToUserDto(UserEntity user,  List<DTDEntity> dtdEntities, List<UGMemberEntity> ugMemberEntities)
        {
            
            if (user == null || !user.LoginServiceUsers.Any())
            {
                return null;
            }

            var userDtoBase = new UserWithIdpInfoDto
            {
                Identity = ToIdentityDto(user),
                EntityStatus = ToEntityStatusDto(user),
                DepartmentName = user.Department.Name,
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
                ExtId = user.ExtId,
                Created = user.Created,
                ForceLoginAgain = user.ForceUserLoginAgain,
                Groups = ExtractParentUserGroupDtos(user, ugMemberEntities, user.CustomerId),
                JsonDynamicAttributes = user.DynamicAttributes == null
                    ? null
                    : JsonConvert.DeserializeObject<IDictionary<string, dynamic>>(user.DynamicAttributes),
            };

            HideOrDecryptSSN(userDtoBase);

            if (string.IsNullOrEmpty(user.Mobile))
            {
                userDtoBase.MobileNumber = null;
            }
            var roles = ExtractToUserTypeDtos(user);
            if (roles != null)
            {
                userDtoBase.CustomData = new Dictionary<string, object>();
                foreach (var item in UserTypeArcheTypes)
                {
                    var userTypeByArcheTypes = roles.Where(x => x.Identity.Archetype == item).ToList();
                    if (userTypeByArcheTypes.Any())
                    {
                        var propertyName = item.ToString().ToCharArray();
                        propertyName[0] = Char.ToLower(propertyName[0]);
                        userDtoBase.CustomData.Add($"{new string(propertyName)}s", userTypeByArcheTypes);
                    }
                }
            }

            userDtoBase.DepartmentTypes = ExtractDepartmentTypeDtos(user, dtdEntities);
            return userDtoBase;
        }

    

        private IdpIdentityDto ToIdentityDto(UserEntity user)
        {
            return new IdpIdentityDto
            {
                Id = user.LoginServiceUsers.FirstOrDefault().PrimaryClaimValue,
                Archetype = user.ArchetypeId.HasValue ? (ArchetypeEnum)user.ArchetypeId : ArchetypeEnum.Unknown,
                CustomerId = user.CustomerId ?? 0,
                OwnerId = user.OwnerId
            };
        }

        private EntityStatusDto ToEntityStatusDto(UserEntity user)
        {
            return new EntityStatusDto
            {
                EntityVersion = user.EntityVersion,
                LastUpdated = user.LastUpdated,
                LastUpdatedBy = user.LastUpdatedBy ?? 0,
                StatusId = (EntityStatusEnum)user.EntityStatusId,
                StatusReasonId = user.EntityStatusReasonId.HasValue ? (EntityStatusReasonEnum)user.EntityStatusReasonId : EntityStatusReasonEnum.Unknown,
                LastExternallySynchronized = user.LastSynchronized,
                ExternallyMastered = user.Locked == 1,
                Deleted = user.Deleted.HasValue,
                ExpirationDate = user.EntityExpirationDate
            };
        }


        private List<UserGroupDto> ExtractParentUserGroupDtos(UserEntity user, List<UGMemberEntity> ugMemberEntities, int? customerId)
        {
            if (ugMemberEntities != null)
            {
                return ugMemberEntities
                    .Where(u => u.UserId == user.UserId
                                && u.EntityStatusId == (int) EntityStatusEnum.Active &&
                                u.UserGroup?.EntityStatusId == (int) EntityStatusEnum.Active)
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
            var userGroupDto =new UserGroupDto
            {
                Identity = new IdentityDto
                {
                    Archetype = userGroup.ArchetypeId.HasValue ? (ArchetypeEnum)userGroup.ArchetypeId : ArchetypeEnum.Unknown,
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
                    StatusReasonId = (EntityStatusReasonEnum)userGroup.EntityStatusReasonId,
                    StatusId = (EntityStatusEnum)userGroup.EntityStatusId,
                    LastUpdatedBy = userGroup.LastUpdatedBy.HasValue ? userGroup.LastUpdatedBy.Value : 0,
                    ExternallyMastered = false
                },
            };
            if (userGroup.User != null)
            {
                userGroupDto.UserIdentity.Archetype = (ArchetypeEnum) (userGroup.User.ArchetypeId ?? 0);
                userGroupDto.UserIdentity.ExtId = userGroup.User.ExtId;
                userGroupDto.UserIdentity.CustomerId = userGroup.User.CustomerId ?? userGroupDto.Identity.CustomerId;
                userGroupDto.UserIdentity.OwnerId = userGroup.User.OwnerId;

                if (IsApprovalGroup(userGroup))
                    userGroupDto.Name = userGroup.User.GetFullName();
            }

            return userGroupDto;
        }
        private bool IsApprovalGroup(UserGroupEntity userGroup)
        {
            return userGroup.ArchetypeId == (int) ArchetypeEnum.ApprovalGroup;
        }
        protected List<UserTypeDto> ExtractToUserTypeDtos(UserEntity user)
        {
            if (user.UT_Us != null)
            {
                if (user.UT_Us.Count == 0) return new List<UserTypeDto>();
                var userTypes = UserTypeEntities.Where(t => user.UT_Us.Any(u => u.UserTypeId == t.UserTypeId)).ToList();
                var roles = _userTypeMappingService.ToUserTypeDtos(userTypes, _workContext.CurrentLanguageId);
                return roles;
            }

            return null;
        }
        protected List<DepartmentTypeDto> ExtractDepartmentTypeDtos(UserEntity user, List<DTDEntity> dtdEntities)
        {
            if (!dtdEntities.IsNullOrEmpty())
            {
                var departmentTypeIds = dtdEntities
                    .Where(d => d.DepartmentId == user.DepartmentId)
                    .Select(d => d.DepartmentTypeId).ToList();

                var departmentTypes = DepartmentTypeEntities.Where(t => departmentTypeIds.Contains(t.DepartmentTypeId))
                    .ToList();

                var departmentTypeDtos = _departmentTypeMappingService.ToDepartmentTypeDtos(departmentTypes, _workContext.CurrentLanguageId);
                return departmentTypeDtos;
            }

            return null;
        }
        public void HideOrDecryptSSN(UserWithIdpInfoDto userDto)
        {
            if (ShouldHideSsn())
            {
                userDto.SSN = null;
            }
            else
            {
                userDto.SSN = _userCryptoService.DecryptSSN(userDto.SSN);
            }
        }
        private bool ShouldHideSsn()
        {
            //We should only hide SSN when api is authenticate by user token to keep backward compatibility for system to system integration 
            return _appSettings.HideSSN && !string.IsNullOrEmpty(_workContext.Sub);
        }

        private bool ShouldHideDateOfBirth()
        {
            //We should only hide date of birth when api is authenticate by user token to keep backward compatibility for system to system integration 
            return _appSettings.HideDateOfBirth && !string.IsNullOrEmpty(_workContext.Sub);
        }
    }
}

