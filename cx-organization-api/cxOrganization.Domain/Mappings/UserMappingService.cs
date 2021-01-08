using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using cxOrganization.Client;
using cxOrganization.Client.UserTypes;
using cxOrganization.Domain.Dtos.Users;
using cxOrganization.Domain.Entities;
using cxOrganization.Domain.Repositories;
using cxOrganization.Domain.Security.User;
using cxOrganization.Domain.Services;
using cxOrganization.Domain.Settings;
using cxPlatform.Client.ConexusBase;
using cxPlatform.Core;
using Microsoft.Extensions.Options;

namespace cxOrganization.Domain.Mappings
{
    public class UserMappingService : IUserMappingService
    {
        private readonly IPropertyService _propertyService;
        private readonly IUserTypeMappingService _userTypeMappingService;
        private readonly IUserCryptoService _userCryptoService;
        private readonly IUserTypeRepository _userTypeRepository;
        private readonly AppSettings _appSettings;
        private readonly IWorkContext _workContext;
        private List<UserTypeEntity> _userTypeEntities;
        public UserMappingService(
            IWorkContext workContext,
            IPropertyService propertyService,
            IUserTypeMappingService userTypeMappingService,
            IUserCryptoService userCryptoService,
            IUserTypeRepository userTypeRepository,
            IOptions<AppSettings> appSettingOption)
        {
            _propertyService = propertyService;
            _userTypeMappingService = userTypeMappingService;
            _userCryptoService = userCryptoService;
            _userTypeRepository = userTypeRepository;
            _appSettings = appSettingOption.Value;
            _workContext = workContext;
        }

        public int? UserTypeId
        {
            get; set;
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

        protected List<EntityKeyValueDto> GetDynamicProperties(int userId, bool? getDynamicProperties = null)
        {
            return getDynamicProperties.HasValue && getDynamicProperties.Value ?
                 _propertyService.GetDynamicProperties(userId, TableTypes.User) :
                 new List<EntityKeyValueDto>();
        }

        public virtual ConexusBaseDto ToUserDto(UserEntity user,
            bool? getDynamicProperties = null, 
            bool keepEncryptedSsn = false, bool keepDecryptedSsn = false,
            List<UGMemberEntity> ugMemberEntities = null)
        {
            var userDto = ToIdentityStatusDto(user);
            if (userDto == null) return null;
            if(getDynamicProperties.HasValue)
                userDto.DynamicAttributes = GetDynamicProperties(user.UserId, getDynamicProperties);
            return userDto;
        }

        public virtual UserEntity ToUserEntity(DepartmentEntity parentDepartment,
            HierarchyDepartmentEntity parentHd,
            UserEntity userEntity,
            UserDtoBase department,
            bool skipCheckingEntityVersion = false,
            int? currentOwnerId = null)
        {
            throw new NotImplementedException();
        }

        public virtual IdentityStatusDto ToIdentityStatusDto(UserEntity user)
        {
            //Don't map Entity that have Archetype is NULL
            if (!user.ArchetypeId.HasValue) return null;

            return new IdentityStatusDto
            {
                Identity = ToIdentityDto(user),
                EntityStatus = ToEntityStatusDto(user)
            };
        }

        public virtual MemberDto ToMemberDto(UserEntity user)
        {
            return new MemberDto
            {
                Identity = ToIdentityDto(user),
                EntityStatus = ToEntityStatusDto(user),
                Role = string.Empty
            };
        }

        protected IdentityDto ToIdentityDto(UserEntity user)
        {
            return new IdentityDto
            {
                Id = user.UserId,
                Archetype = user.ArchetypeId.HasValue ? (ArchetypeEnum)user.ArchetypeId : ArchetypeEnum.Unknown,
                CustomerId = user.CustomerId ?? 0,
                ExtId = user.ExtId,
                OwnerId = user.OwnerId
            };
        }

        protected EntityStatusDto ToEntityStatusDto(UserEntity user)
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
                ExpirationDate = user.EntityExpirationDate,
                ActiveDate = user.EntityActiveDate
            };
        }

        //Provide mapping as static method to reuse in other classes (UserService) 
        public static short MapEntityStatusEnumToStatus(EntityStatusEnum entityStatus)
        {
            switch (entityStatus)
            {
                case EntityStatusEnum.Active:
                    return 0;
                case EntityStatusEnum.Inactive:
                    return 1;
                case EntityStatusEnum.Deactive:
                    return -10;
                //Default is inactive
                default:
                    return 1;
            }
        }

        protected short ToStatus(EntityStatusEnum entityStatus)
        {
            return MapEntityStatusEnumToStatus(entityStatus);
        }

        protected EntityStatusEnum ToEntityStatusEnum(short status, int? entityStatusId)
        {
            var entityStatus = EntityStatusEnum.Unknown;
            if (entityStatusId.HasValue)
            {
                entityStatus = (EntityStatusEnum)entityStatusId;
            }
            //Return Active
            if (status == 0 && (entityStatus == EntityStatusEnum.Active || entityStatus == EntityStatusEnum.Unknown))
            {
                return EntityStatusEnum.Active;
            }

            //Return Inactive
            if (status == 1 || entityStatus == EntityStatusEnum.Inactive)
            {
                return EntityStatusEnum.Inactive;
            }

            return EntityStatusEnum.Deactive;
        }

        protected void CheckEntityVersion(byte[] clientVersion, byte[] dbVersion)
        {
            if (!StructuralComparisons.StructuralEqualityComparer.Equals(clientVersion, dbVersion))
            {
                throw new Exception(MappingErrorDefault.ERROR_ENTITY_VERSION_INCORRECTED);
            }
        }

        protected string RemoveSpecialCharactersAndSpaces(string input)
        {
            if (string.IsNullOrEmpty(input)) return input;

            StringBuilder sb = new StringBuilder();
            foreach (char c in input)
            {
                if ((c >= '0' && c <= '9') || (c >= 'A' && c <= 'Z') || (c >= 'a' && c <= 'z' || c == '.' || c == '_'))
                {
                    sb.Append(c);
                }
            }
            return sb.ToString();
        }

        public virtual UserEntity ToUserEntity(DepartmentEntity parentDepartment, UserEntity entity, UserDtoBase userDto, int? currentOwnerId = null)
        {
            throw new NotImplementedException();
        }

        protected void MapSsnToUserDto(UserDtoBase userDto, UserEntity userEntity, bool keepEncryptedSsn = false, bool keepDecryptedSsn = false)
        {
            if (keepEncryptedSsn)
            {
                userDto.SSN = userEntity.SSN;
            }
            else
            {

                if (!keepDecryptedSsn && ShouldHideSsn())
                {
                    userDto.SSN = null;
                }
                else
                {
                    userDto.SSN = _userCryptoService.DecryptSSN(userEntity.SSN);
                }
            }

        }
        public void HideOrDecryptSSN(UserDtoBase userDto)
        {
            userDto.SSN = ShouldHideSsn() ? null : _userCryptoService.DecryptSSN(userDto.SSN);
        }
        protected bool ShouldHideSsn()
        {
            //We should only hide SSN when api is authenticate by user token to keep backward compatibility for system to system integration 
            return _appSettings.HideSSN && !string.IsNullOrEmpty(_workContext.Sub);
        }
        protected bool ShouldHideDateOfBirth()
        {
            //We should only hide date of birth when api is authenticate by user token to keep backward compatibility for system to system integration 
            return _appSettings.HideDateOfBirth && !string.IsNullOrEmpty(_workContext.Sub);
        }

        protected List<LoginServiceClaimDto> ToLoginServiceClaims(ICollection<LoginServiceUserEntity> loginServiceUserEntities)
        {
            return loginServiceUserEntities == null
                ? new List<LoginServiceClaimDto>()
                : loginServiceUserEntities
                    .Select(lu => new LoginServiceClaimDto {Id = lu.LoginServiceId, Value = lu.PrimaryClaimValue})
                    .ToList();
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
    
    }
}
