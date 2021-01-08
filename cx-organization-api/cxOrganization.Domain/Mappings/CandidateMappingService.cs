using cxOrganization.Domain.Dtos.Users;
using cxOrganization.Domain.Entities;
using cxOrganization.Domain.Repositories;
using cxOrganization.Domain.Security.User;
using cxOrganization.Domain.Services;
using cxPlatform.Client.ConexusBase;
using cxPlatform.Core;
using cxPlatform.Core.Security;
using System;
using System.Collections.Generic;
using System.Linq;
using cxOrganization.Domain.Settings;
using Microsoft.Extensions.Options;

namespace cxOrganization.Domain.Mappings
{
    public class CandidateMappingService : UserMappingService
    {
        private readonly IWorkContext _workContext;
        private readonly IUserTypeRepository _userTypeRepository;
        private readonly IOwnerRepository _ownerRepository;
        private readonly IUserCryptoService _userCryptoService;

        public CandidateMappingService(
            IUserTypeRepository userTypeRepository, int userTypeId,
            IWorkContext workContext,
            IPropertyService propertyService,
            IOwnerRepository ownerRepository,
            IUserTypeMappingService userTypeMappingService,
            IUserCryptoService userCryptoService,
            IOptions<AppSettings> appSettingOption)
            : base(workContext, propertyService, userTypeMappingService, userCryptoService, userTypeRepository, appSettingOption)
        {
            //Inject services needed
            UserTypeId = userTypeId;
            _userTypeRepository = userTypeRepository;
            _workContext = workContext;
            _ownerRepository = ownerRepository;
            _userCryptoService = userCryptoService;
        }

        public override ConexusBaseDto ToUserDto(UserEntity user,
            bool? getDynamicProperties = null,
            bool keepEncryptedSsn = false,
            bool keepDecryptedSsn = false,
            List<UGMemberEntity> ugMemberEntities = null)
        {
            if (user == null || user.ArchetypeId != (short)ArchetypeEnum.Candidate)
            {
                return null;
            }

            var candidateDto = new CandidateDto
            {
                Identity = ToIdentityDto(user),
                EntityStatus = ToEntityStatusDto(user),
                FirstName = string.IsNullOrEmpty(user.FirstName) ? null : user.FirstName,
                LastName = string.IsNullOrEmpty(user.LastName) ? null : user.LastName,
                DateOfBirth = ShouldHideDateOfBirth() ? null : user.DateOfBirth,
                EmailAddress = string.IsNullOrEmpty(user.Email) ? null : user.Email,
                MobileNumber = string.IsNullOrEmpty(user.Mobile) ? null : user.Mobile,
                MobileCountryCode = user.CountryCode,
                SSN =  user.SSN ,
                Password = null,
                Gender = user.Gender,
                Tag = user.Tag,
                ParentDepartmentId = user.DepartmentId,
                Created = user.Created,
                ForceLoginAgain = user.ForceUserLoginAgain
            };

            MapSsnToUserDto(candidateDto, user, keepEncryptedSsn, keepDecryptedSsn);

            var userId = (int)candidateDto.Identity.Id.Value;
            candidateDto.DynamicAttributes = GetDynamicProperties(userId, getDynamicProperties);
            candidateDto.LoginServiceClaims = ToLoginServiceClaims(user.LoginServiceUsers);
            candidateDto.Roles = ExtractToUserTypeDtos(user).ToList();

            return candidateDto;
        }

        public override UserEntity ToUserEntity(DepartmentEntity parentDepartment,
            HierarchyDepartmentEntity parentHd,
            UserEntity userEntity,
            UserDtoBase userDto,
            bool skipCheckingEntityVersion = false,
            int? currentOwnerId = null)
        {
            var candidateDto = (CandidateDto)userDto;

            //Initial  entity
            if (userDto.Identity.Id == 0)
            {
                return NewUserEntity(parentHd.Department, candidateDto);
            }

            if (userEntity == null)
                return null;
            if (!skipCheckingEntityVersion)
            {
                CheckEntityVersion(candidateDto.EntityStatus.EntityVersion, userEntity.EntityVersion);
            }
            //Entity is existed
            userEntity.FirstName = candidateDto.FirstName ?? string.Empty;
            userEntity.LastName = candidateDto.LastName ?? string.Empty;
            userEntity.DateOfBirth = candidateDto.DateOfBirth ?? userEntity.DateOfBirth;
            userEntity.Email = candidateDto.EmailAddress ?? string.Empty;
            userEntity.Mobile = candidateDto.MobileNumber ?? string.Empty;
            userEntity.CountryCode = candidateDto.MobileCountryCode;
            userEntity.ExtId = candidateDto.Identity.ExtId ?? string.Empty;
          
            if (candidateDto.SSN != null)
            {
                userEntity.SSN = _userCryptoService.EncryptSSN(candidateDto.SSN);
            }

            userEntity.Gender = candidateDto.Gender;
            userEntity.EntityStatusId = (int)candidateDto.EntityStatus.StatusId;
            userEntity.EntityStatusReasonId = Enum.IsDefined(typeof(EntityStatusReasonEnum), candidateDto.EntityStatus.StatusReasonId) ? (int)candidateDto.EntityStatus.StatusReasonId : userEntity.EntityStatusReasonId;
            userEntity.LastUpdated = DateTime.Now;
            userEntity.LastUpdatedBy = userDto.EntityStatus.LastUpdatedBy > 0
                                            ? userDto.EntityStatus.LastUpdatedBy
                                            : _workContext.CurrentUserId;
            userEntity.Tag = candidateDto.Tag ?? string.Empty;
            userEntity.DepartmentId = candidateDto.ParentDepartmentId;



            //Set default country code
            if (parentDepartment != null && (!candidateDto.MobileCountryCode.HasValue || candidateDto.MobileCountryCode <= 0))
            {
                userEntity.CountryCode = parentDepartment.CountryCode;
            }
            if (candidateDto.EntityStatus.LastExternallySynchronized.HasValue)
            {
                userEntity.LastSynchronized = candidateDto.EntityStatus.LastExternallySynchronized.Value;
            }
            userEntity.Locked = (short)(candidateDto.EntityStatus.ExternallyMastered ? 1 : 0);

            if (candidateDto.ForceLoginAgain.HasValue)
            {
                userEntity.ForceUserLoginAgain = candidateDto.ForceLoginAgain.Value;
            }

            return userEntity;
        }

        public override UserEntity ToUserEntity(DepartmentEntity parentDepartment, UserEntity userEntity, UserDtoBase userDto, int? currentOwnerId = null)
        {
            var candidateDto = (CandidateDto)userDto;

            //Initial  entity
            if (userDto.Identity.Id == 0)
            {
                return NewUserEntity(parentDepartment, candidateDto);
            }

            if (userEntity == null)
                return null;
            CheckEntityVersion(candidateDto.EntityStatus.EntityVersion, userEntity.EntityVersion);
            //Entity is existed
            userEntity.FirstName = candidateDto.FirstName ?? string.Empty;
            userEntity.LastName = candidateDto.LastName ?? string.Empty;
            userEntity.DateOfBirth = candidateDto.DateOfBirth ?? userEntity.DateOfBirth;
            userEntity.Email = candidateDto.EmailAddress ?? string.Empty;
            userEntity.Mobile = candidateDto.MobileNumber ?? string.Empty;
            userEntity.CountryCode = candidateDto.MobileCountryCode;
            userEntity.ExtId = candidateDto.Identity.ExtId ?? string.Empty;

            if (candidateDto.SSN != null)
            {
                userEntity.SSN = _userCryptoService.EncryptSSN(candidateDto.SSN);
            }

            userEntity.Gender = candidateDto.Gender;
            userEntity.EntityStatusId = (int)candidateDto.EntityStatus.StatusId;
            userEntity.EntityStatusReasonId = Enum.IsDefined(typeof(EntityStatusReasonEnum), candidateDto.EntityStatus.StatusReasonId) ? (int)candidateDto.EntityStatus.StatusReasonId : userEntity.EntityStatusReasonId;
            userEntity.LastUpdated = DateTime.Now;
            userEntity.LastUpdatedBy = candidateDto.EntityStatus.LastUpdatedBy;
            userEntity.Tag = candidateDto.Tag ?? string.Empty;
            userEntity.DepartmentId = candidateDto.ParentDepartmentId;


            //Set default country code
            if (parentDepartment != null && (!candidateDto.MobileCountryCode.HasValue || candidateDto.MobileCountryCode <= 0))
            {
                userEntity.CountryCode = parentDepartment.CountryCode;
            }
            if (candidateDto.EntityStatus.LastExternallySynchronized.HasValue)
            {
                userEntity.LastSynchronized = candidateDto.EntityStatus.LastExternallySynchronized.Value;
            }
            userEntity.Locked = (short)(candidateDto.EntityStatus.ExternallyMastered ? 1 : 0);

            if (candidateDto.ForceLoginAgain.HasValue)
            {
                userEntity.ForceUserLoginAgain = candidateDto.ForceLoginAgain.Value;
            }
            

            return userEntity;
        }
        private UserEntity NewUserEntity(DepartmentEntity parent, CandidateDto candidateDto)
        {
            var userNew = new UserEntity
            {
                UserName = Guid.NewGuid().ToString(),
                FirstName = candidateDto.FirstName ?? string.Empty,
                LastName = candidateDto.LastName ?? string.Empty,
                Email = candidateDto.EmailAddress ?? string.Empty,
                Mobile = candidateDto.MobileNumber ?? string.Empty,
                CountryCode = candidateDto.MobileCountryCode,
                ExtId = candidateDto.Identity.ExtId ?? string.Empty,
                DepartmentId = candidateDto.ParentDepartmentId,
                SSN = _userCryptoService.EncryptSSN(candidateDto.SSN),
                Tag = candidateDto.Tag ?? string.Empty,
                SaltPassword = string.Empty,
                HashPassword = string.Empty,
                Created = DateTime.Now,
                LastUpdated = DateTime.Now,
                LastUpdatedBy = candidateDto.EntityStatus.LastUpdatedBy > 0
                    ? candidateDto.EntityStatus.LastUpdatedBy
                    : _workContext.CurrentUserId,
                EntityStatusId = (int)candidateDto.EntityStatus.StatusId,
                EntityStatusReasonId = (int)candidateDto.EntityStatus.StatusReasonId,
                OwnerId = candidateDto.Identity.OwnerId,
                CustomerId = candidateDto.Identity.CustomerId,
                ArchetypeId = (int)ArchetypeEnum.Candidate,
                DateOfBirth = candidateDto.DateOfBirth,
                Gender = candidateDto.Gender,
                ForceUserLoginAgain = candidateDto.ForceLoginAgain ?? false
            };
            if (this.UserTypeId.HasValue)
            {
                userNew.UT_Us = new List<UTUEntity> { new UTUEntity { UserTypeId = this.UserTypeId.Value } };
            }
            if (candidateDto.EntityStatus.LastExternallySynchronized.HasValue)
            {
                userNew.LastSynchronized = candidateDto.EntityStatus.LastExternallySynchronized.Value;
            }
            userNew.Locked = (short)(candidateDto.EntityStatus.ExternallyMastered ? 1 : 0);

            if (parent != null)
            {
                //Set default customer
                if (!userNew.CustomerId.HasValue && parent.CustomerId > 0)
                {
                    userNew.CustomerId = parent.CustomerId;
                }
                //Set default language
                if (userNew.LanguageId <= 0)
                {
                    userNew.LanguageId = parent.LanguageId;
                }
                //Set default country code
                if (!userNew.CountryCode.HasValue || userNew.CountryCode <= 0)
                {
                    userNew.CountryCode = parent.CountryCode;
                }
            }
            var currentOwner = _ownerRepository.GetById(_workContext.CurrentOwnerId);
            if (currentOwner.UseOTP)
            {
                userNew.Password = string.Empty;
                userNew.OneTimePassword = string.IsNullOrEmpty(candidateDto.Password) ? Encryption.Encrypt(Generator.GenerateRandomPassword(8, "ABCDEFGHJKLMNPQRSTWXZ23456789", false, true, bIncludeNumbers: false)) : Encryption.Encrypt(candidateDto.Password);
            }
            else
            {
                userNew.Password = string.IsNullOrEmpty(candidateDto.Password) ? Encryption.Encrypt(Generator.GenerateRandomPassword(8, "ABCDEFGHJKLMNPQRSTWXZ23456789", false, true, bIncludeNumbers: false)) : Encryption.Encrypt(candidateDto.Password);
                userNew.OneTimePassword = Encryption.Encrypt(string.Empty);
            }
            return userNew;
        }


    }
}
