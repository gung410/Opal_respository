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
    public class LearnerMappingService : UserMappingService
    {
        private readonly IWorkContext _workContext;
        private readonly IDepartmentService _departmentService;
        private readonly IHierarchyDepartmentRepository _hierarchyDepartmentRepository;
        private readonly IUserTypeRepository _userTypeRepository;
        private readonly IOwnerRepository _ownerRepository;
        private readonly IUserCryptoService _userCryptoService;

        public LearnerMappingService(
            IUserTypeRepository userTypeRepository, int userTypeId,
            IWorkContext workContext, IDepartmentService departmentService,
            IPropertyService propertyService,
            IHierarchyDepartmentRepository hierarchyDepartmentRepository,
            IOwnerRepository ownerRepository,
            IUserTypeMappingService userTypeMappingService,
            IUserCryptoService userCryptoService,
            IOptions<AppSettings> appSettingOption)
            : base(workContext, propertyService, userTypeMappingService, userCryptoService, userTypeRepository, appSettingOption )
        {
            //Inject services needed
            UserTypeId = userTypeId;
            _userTypeRepository = userTypeRepository;
            _workContext = workContext;
            _departmentService = departmentService;
            _hierarchyDepartmentRepository = hierarchyDepartmentRepository;
            _ownerRepository = ownerRepository;
            _userCryptoService = userCryptoService;
        }

        public override ConexusBaseDto ToUserDto(UserEntity user, 
            bool? getDynamicProperties = null, 
            bool keepEncryptedSsn = false, 
            bool  keepDecryptedSsn = false,
            List<UGMemberEntity> ugMemberEntities = null)
        {
            if (user == null || user.ArchetypeId != (short)ArchetypeEnum.Learner)
            {
                return null;
            }

            var learner = new LearnerDto
            {
                Identity = ToIdentityDto(user),
                EntityStatus = ToEntityStatusDto(user),
                FirstName = string.IsNullOrEmpty(user.FirstName) ? null : user.FirstName,
                LastName = string.IsNullOrEmpty(user.LastName) ? null : user.LastName,
                DateOfBirth = ShouldHideDateOfBirth() ? null : user.DateOfBirth,
                EmailAddress = string.IsNullOrEmpty(user.Email) ? null : user.Email,
                MobileNumber = string.IsNullOrEmpty(user.Mobile) ? null : user.Mobile,
                MobileCountryCode = user.CountryCode,
                SSN = user.SSN,
                Password = null,
                Gender = user.Gender,
                Tag = user.Tag,
                Created = user.Created,
                ForceLoginAgain = user.ForceUserLoginAgain
            };

            MapSsnToUserDto(learner, user, keepEncryptedSsn, keepDecryptedSsn);

            var parentDepartment = _departmentService.GetDepartmentById(user.DepartmentId);
            if (parentDepartment.ArchetypeId == (int)ArchetypeEnum.Class)
            {
                var currentHD = _hierarchyDepartmentRepository.GetById(_workContext.CurrentHdId);
                if (currentHD != null)
                {
                    var classHd = _hierarchyDepartmentRepository.GetHdByHierarchyIdAndDepartmentId(currentHD.HierarchyId, user.DepartmentId, true);
                    if (classHd != null && classHd.ParentId != null)
                    {
                        var schoolHd = _hierarchyDepartmentRepository.GetById(classHd.ParentId.Value);
                        learner.ParentDepartmentId = schoolHd.DepartmentId;
                    }
                }
                else
                {
                    learner.ParentDepartmentId = user.DepartmentId;
                }
            }
            else
            {
                learner.ParentDepartmentId = user.DepartmentId;
            }

            var userId = (int)learner.Identity.Id.Value;
            learner.DynamicAttributes = GetDynamicProperties(userId, getDynamicProperties);
            learner.LoginServiceClaims = ToLoginServiceClaims(user.LoginServiceUsers);
            learner.Roles = ExtractToUserTypeDtos(user);

            return learner;
        }

        public override UserEntity ToUserEntity(DepartmentEntity parentDepartment,
            HierarchyDepartmentEntity parentHd,
            UserEntity userEntity,
            UserDtoBase userDto,
            bool skipCheckingEntityVersion = false,
            int? currentOwnerId = null)
        {
            var learnerDto = (LearnerDto)userDto;

            //Initial  entity
            if (userDto.Identity.Id == 0)
            {
                return NewUserEntity(parentDepartment, learnerDto);
            }

            if (userEntity == null)
                return null;
            if (!skipCheckingEntityVersion)
            {
                CheckEntityVersion(learnerDto.EntityStatus.EntityVersion, userEntity.EntityVersion);
            }
            //Entity is existed
            userEntity.FirstName = learnerDto.FirstName ?? string.Empty;
            userEntity.LastName = learnerDto.LastName ?? string.Empty;
            userEntity.DateOfBirth = learnerDto.DateOfBirth ?? userEntity.DateOfBirth;
            userEntity.Email = learnerDto.EmailAddress ?? string.Empty;
            userEntity.Mobile = learnerDto.MobileNumber ?? string.Empty;
            userEntity.CountryCode = learnerDto.MobileCountryCode;
            userEntity.ExtId = learnerDto.Identity.ExtId ?? string.Empty;
          
            if (learnerDto.SSN != null)
            {
                userEntity.SSN = _userCryptoService.EncryptSSN(learnerDto.SSN);
            }

            userEntity.Gender = learnerDto.Gender;
            userEntity.EntityStatusId = (int)learnerDto.EntityStatus.StatusId;
            userEntity.EntityStatusReasonId = Enum.IsDefined(typeof(EntityStatusReasonEnum), learnerDto.EntityStatus.StatusReasonId) ? (int)learnerDto.EntityStatus.StatusReasonId : userEntity.EntityStatusReasonId;
            userEntity.LastUpdated = DateTime.Now;
            userEntity.LastUpdatedBy = learnerDto.EntityStatus.LastUpdatedBy;
            userEntity.Tag = learnerDto.Tag ?? string.Empty;

            //ALlow change department when it's not in class level
            var parentDepartmentInDb = _departmentService.GetDepartmentById(userEntity.DepartmentId);
            if (parentDepartmentInDb != null && parentDepartmentInDb.ArchetypeId != (int)ArchetypeEnum.Class)
            {
                userEntity.DepartmentId = learnerDto.ParentDepartmentId;
            }

            //Set default country code
            if (parentDepartment != null && (!learnerDto.MobileCountryCode.HasValue || learnerDto.MobileCountryCode <= 0))
            {
                userEntity.CountryCode = parentDepartment.CountryCode;
            }
            if (learnerDto.EntityStatus.LastExternallySynchronized.HasValue)
            {
                userEntity.LastSynchronized = learnerDto.EntityStatus.LastExternallySynchronized.Value;
            }
            userEntity.Locked = (short)(learnerDto.EntityStatus.ExternallyMastered ? 1 : 0);

            if (learnerDto.ForceLoginAgain.HasValue)
            {
                userEntity.ForceUserLoginAgain = learnerDto.ForceLoginAgain.Value;
            }

            return userEntity;
        }
        public override UserEntity ToUserEntity(DepartmentEntity parentDepartment,
            UserEntity userEntity,
            UserDtoBase userDto,
            int? currentOwnerId = null)
        {
            var learnerDto = (LearnerDto)userDto;

            //Initial  entity
            if (userDto.Identity.Id == 0)
            {
                return NewUserEntity(parentDepartment, learnerDto);
            }

            if (userEntity == null)
                return null;
            CheckEntityVersion(learnerDto.EntityStatus.EntityVersion, userEntity.EntityVersion);
            //Entity is existed
            userEntity.FirstName = learnerDto.FirstName ?? string.Empty;
            userEntity.LastName = learnerDto.LastName ?? string.Empty;
            userEntity.DateOfBirth = learnerDto.DateOfBirth ?? userEntity.DateOfBirth;
            userEntity.Email = learnerDto.EmailAddress ?? string.Empty;
            userEntity.Mobile = learnerDto.MobileNumber ?? string.Empty;
            userEntity.CountryCode = learnerDto.MobileCountryCode;
            userEntity.ExtId = learnerDto.Identity.ExtId ?? string.Empty;
            if (learnerDto.SSN != null)
            {
                userEntity.SSN = _userCryptoService.EncryptSSN(learnerDto.SSN);
            }
            userEntity.Gender = learnerDto.Gender;
            userEntity.EntityStatusId = (int)learnerDto.EntityStatus.StatusId;
            userEntity.EntityStatusReasonId = Enum.IsDefined(typeof(EntityStatusReasonEnum), learnerDto.EntityStatus.StatusReasonId) ? (int)learnerDto.EntityStatus.StatusReasonId : userEntity.EntityStatusReasonId;
            userEntity.LastUpdated = DateTime.Now;
            userEntity.LastUpdatedBy = learnerDto.EntityStatus.LastUpdatedBy;
            userEntity.Tag = learnerDto.Tag ?? string.Empty;

            //ALlow change department when it's not in class level
            var parentDepartmentInDb = _departmentService.GetDepartmentById(userEntity.DepartmentId);
            if (parentDepartmentInDb != null && parentDepartmentInDb.ArchetypeId != (int)ArchetypeEnum.Class)
            {
                userEntity.DepartmentId = learnerDto.ParentDepartmentId;
            }

            //Set default country code
            if (parentDepartment != null && (!learnerDto.MobileCountryCode.HasValue || learnerDto.MobileCountryCode <= 0))
            {
                userEntity.CountryCode = parentDepartment.CountryCode;
            }
            if (learnerDto.EntityStatus.LastExternallySynchronized.HasValue)
            {
                userEntity.LastSynchronized = learnerDto.EntityStatus.LastExternallySynchronized.Value;
            }
            userEntity.Locked = (short)(learnerDto.EntityStatus.ExternallyMastered ? 1 : 0);

            if (learnerDto.ForceLoginAgain.HasValue)
            {
                userEntity.ForceUserLoginAgain = learnerDto.ForceLoginAgain.Value;
            }
            return userEntity;
        }

        private UserEntity NewUserEntity(DepartmentEntity parent, LearnerDto learnerDto)
        {
            var userNew = new UserEntity
            {
                UserName = Guid.NewGuid().ToString(),
                FirstName = learnerDto.FirstName ?? string.Empty,
                LastName = learnerDto.LastName ?? string.Empty,
                Email = learnerDto.EmailAddress ?? string.Empty,
                Mobile = learnerDto.MobileNumber ?? string.Empty,
                CountryCode = learnerDto.MobileCountryCode,
                ExtId = learnerDto.Identity.ExtId ?? string.Empty,
                DepartmentId = learnerDto.ParentDepartmentId,
                SSN = _userCryptoService.EncryptSSN(learnerDto.SSN),
                Tag = learnerDto.Tag ?? string.Empty,
                SaltPassword = string.Empty,
                HashPassword = string.Empty,
                Created = DateTime.Now,
                LastUpdated = DateTime.Now,
                LastUpdatedBy = learnerDto.EntityStatus.LastUpdatedBy,
                EntityStatusId = (int)learnerDto.EntityStatus.StatusId,
                EntityStatusReasonId = (int)learnerDto.EntityStatus.StatusReasonId,
                OwnerId = learnerDto.Identity.OwnerId,
                CustomerId = learnerDto.Identity.CustomerId,
                ArchetypeId = (int)ArchetypeEnum.Learner,
                DateOfBirth = learnerDto.DateOfBirth,
                Gender = learnerDto.Gender,
                ForceUserLoginAgain = learnerDto.ForceLoginAgain ?? false
            };


            var userType = _userTypeRepository.GetById(UserTypeId);
            if (userType != null)
            {
                userNew.UT_Us.Add(new UTUEntity { UserTypeId = UserTypeId.Value });
            }
            if (learnerDto.EntityStatus.LastExternallySynchronized.HasValue)
            {
                userNew.LastSynchronized = learnerDto.EntityStatus.LastExternallySynchronized.Value;
            }
            userNew.Locked = (short)(learnerDto.EntityStatus.ExternallyMastered ? 1 : 0);

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
                userNew.OneTimePassword = string.IsNullOrEmpty(learnerDto.Password) ? Encryption.Encrypt(Generator.GenerateRandomPassword(8, "ABCDEFGHJKLMNPQRSTWXZ23456789", false, true, bIncludeNumbers: false)) : Encryption.Encrypt(learnerDto.Password);
            }
            else
            {
                userNew.Password = string.IsNullOrEmpty(learnerDto.Password) ? Encryption.Encrypt(Generator.GenerateRandomPassword(8, "ABCDEFGHJKLMNPQRSTWXZ23456789", false, true, bIncludeNumbers: false)) : Encryption.Encrypt(learnerDto.Password);
                userNew.OneTimePassword = Encryption.Encrypt(string.Empty);
            }
            return userNew;
        }
    }
}
