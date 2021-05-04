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
using NPOI.SS.Formula.Functions;
using cxOrganization.Domain.AdvancedWorkContext;

namespace cxOrganization.Domain.Mappings
{
    public class ActorMappingService : UserMappingService
    {
        private readonly IUserTypeRepository _userTypeRepository;
        private readonly IAdvancedWorkContext _workContext;
        private readonly IDepartmentRepository _departmentRepository;
        private readonly IPropertyService _propertyService;
        private readonly IUserRepository _userRepository;
        private readonly IHierarchyDepartmentRepository _hierarchyDepartmentRepository;
        private readonly IOwnerRepository _ownerRepository;
        private readonly IUserCryptoService _userCryptoService;

        public ActorMappingService(IUserTypeRepository userTypeRepository,
            int userTypeId,
            IAdvancedWorkContext workContext,
            IDepartmentRepository departmentRepository,
            IPropertyService propertyService,
            IUserRepository userRepository,
            IHierarchyDepartmentRepository hierarchyDepartmentRepository,
            IOwnerRepository ownerRepository,
            IUserTypeMappingService userTypeMappingService,
            IUserCryptoService userCryptoService,
            IOptions<AppSettings> appSettingOption)
            : base(workContext, propertyService, userTypeMappingService, userCryptoService, userTypeRepository, appSettingOption)
        {
            UserTypeId = userTypeId;
            _userTypeRepository = userTypeRepository;
            _workContext = workContext;
            _departmentRepository = departmentRepository;
            _propertyService = propertyService;
            _userRepository = userRepository;
            _hierarchyDepartmentRepository = hierarchyDepartmentRepository;
            _ownerRepository = ownerRepository;
            _userCryptoService = userCryptoService;
        }

        public override ConexusBaseDto ToUserDto(UserEntity user,
            bool? getDynamicProperties = null, 
            bool keepEncryptedSsn = false,
            bool keepDecryptedSsn = false,
            List<UGMemberEntity> ugMemberEntities = null)
        {
            if (user == null || string.IsNullOrWhiteSpace(user.SSN))
            {
                return null;
            }
            var currentOwner = _ownerRepository.GetById(_workContext.CurrentOwnerId);
            var userDtoBase = new EmployeeDto
            {
                Identity = ToIdentityDto(user),
                EntityStatus = ToEntityStatusDto(user),

                //This use to solve the confuse from DB: not null column
                FirstName = string.IsNullOrEmpty(user.FirstName) ? null : user.FirstName,
                LastName = string.IsNullOrEmpty(user.LastName) ? null : user.LastName,
                DateOfBirth = ShouldHideDateOfBirth() ? null : user.DateOfBirth,
                EmailAddress = string.IsNullOrEmpty(user.Email) ? null : user.Email,
                MobileNumber = string.IsNullOrEmpty(user.Mobile) ? null : user.Mobile,
                MobileCountryCode = user.CountryCode,
                SSN =  user.SSN,
                Password = null,
                EmployerDepartmentId = user.DepartmentId,
                Gender = user.Gender,
                Tag = user.Tag,
                Created = user.Created,
                ForceLoginAgain = user.ForceUserLoginAgain
            };
         
            MapSsnToUserDto(userDtoBase, user, keepEncryptedSsn, keepDecryptedSsn);
         
            var parentDepartment = _departmentRepository.GetById(user.DepartmentId);
            if (parentDepartment.ArchetypeId == (int)ArchetypeEnum.Class)
            {
                var currentHD = _hierarchyDepartmentRepository.GetById(_workContext.CurrentHdId);
                var classHd = _hierarchyDepartmentRepository.GetHdByHierarchyIdAndDepartmentId(currentHD.HierarchyId, user.DepartmentId, true);
                if (classHd != null && classHd.ParentId != null)
                {
                    var schoolHd = _hierarchyDepartmentRepository.GetById(classHd.ParentId.Value);
                    userDtoBase.EmployerDepartmentId = schoolHd.DepartmentId;
                }
            }
            else
            {
                userDtoBase.EmployerDepartmentId = user.DepartmentId;
            }

            if (string.IsNullOrEmpty(user.Mobile))
            {
                userDtoBase.MobileNumber = null;
            }
            var userId = (int)userDtoBase.Identity.Id.Value;
            userDtoBase.DynamicAttributes = GetDynamicProperties(userId, getDynamicProperties);
            userDtoBase.LoginServiceClaims = ToLoginServiceClaims(user.LoginServiceUsers);
            userDtoBase.Roles = ExtractToUserTypeDtos(user);

            return userDtoBase;
        }

        public override UserEntity ToUserEntity(DepartmentEntity parentDepartment,
            HierarchyDepartmentEntity parentHd,
            UserEntity userEntity,
            UserDtoBase userDto,
            bool skipCheckingEntityVersion = false,
            int? currentOwnerId = null)
        {
            var empDto = (EmployeeDto)userDto;

            //Initial  entity
            empDto.EntityStatus.LastUpdatedBy = empDto.EntityStatus.LastUpdatedBy > 0
                                                     ? empDto.EntityStatus.LastUpdatedBy
                                                     : _workContext.CurrentUserId;

            if (userDto.Identity.Id == 0)
            {
                return NewUserEntity(parentHd, empDto);
            }

            //Entity is existed
            if (userEntity == null)
                return null;
            if (!skipCheckingEntityVersion)
            {
                CheckEntityVersion(empDto.EntityStatus.EntityVersion, userEntity.EntityVersion);
            }
            userEntity.FirstName = empDto.FirstName ?? string.Empty;
            userEntity.LastName = empDto.LastName ?? string.Empty;
            userEntity.DateOfBirth = empDto.DateOfBirth ?? userEntity.DateOfBirth;
            userEntity.Email = empDto.EmailAddress ?? string.Empty;
            userEntity.Mobile = empDto.MobileNumber ?? string.Empty;
            userEntity.CountryCode = empDto.MobileCountryCode;
            userEntity.ExtId = empDto.Identity.ExtId ?? string.Empty;
            
            if (empDto.SSN != null)
            {
                userEntity.SSN = _userCryptoService.EncryptSSN(empDto.SSN);
            }

            userEntity.Gender = empDto.Gender;
            userEntity.EntityStatusId = (short)empDto.EntityStatus.StatusId;
            userEntity.EntityStatusReasonId = Enum.IsDefined(typeof(EntityStatusReasonEnum), userDto.EntityStatus.StatusReasonId) ? (int)userDto.EntityStatus.StatusReasonId : userEntity.EntityStatusReasonId;
            userEntity.LastUpdated = DateTime.Now;
            userEntity.Tag = empDto.Tag ?? string.Empty;
            userEntity.LastUpdatedBy = userDto.EntityStatus.LastUpdatedBy > 0
                                            ? userDto.EntityStatus.LastUpdatedBy
                                            : _workContext.CurrentUserId;

            //ALlow change department when it's not in class level
            var parentDepartmentInDb = _departmentRepository.GetById(userEntity.DepartmentId);
            if (parentDepartmentInDb != null && parentDepartmentInDb.ArchetypeId != (int)ArchetypeEnum.Class)
            {
                userEntity.DepartmentId = empDto.EmployerDepartmentId;
            }

            //Set default country code
            if (!empDto.MobileCountryCode.HasValue || empDto.MobileCountryCode <= 0)
            {
                var department = _departmentRepository.GetById(userEntity.DepartmentId);
                userEntity.CountryCode = department.CountryCode;
            }
            if (empDto.EntityStatus.LastExternallySynchronized.HasValue)
            {
                userEntity.LastSynchronized = empDto.EntityStatus.LastExternallySynchronized.Value;
            }
            userEntity.Locked = (short)(empDto.EntityStatus.ExternallyMastered ? 1 : 0);

            if (empDto.ForceLoginAgain.HasValue)
            {
                userEntity.ForceUserLoginAgain = empDto.ForceLoginAgain.Value;
            }
            return userEntity;
        }

        private UserEntity NewUserEntity(HierarchyDepartmentEntity parentHd, EmployeeDto empDto)
        {
            var userNew = new UserEntity
            {
                UserName = GenerateUserName(empDto),
                FirstName = empDto.FirstName ?? string.Empty,
                LastName = empDto.LastName ?? string.Empty,
                Email = empDto.EmailAddress ?? string.Empty,
                Mobile = empDto.MobileNumber ?? string.Empty,
                CountryCode = empDto.MobileCountryCode,
                ExtId = empDto.Identity.ExtId ?? string.Empty,
                DepartmentId = empDto.EmployerDepartmentId,
                SSN = empDto.SSN ?? string.Empty,
                Tag = empDto.Tag ?? string.Empty,
                SaltPassword = string.Empty,
                HashPassword = string.Empty,
                Created = DateTime.Now,
                LastUpdated = DateTime.Now,
                LastUpdatedBy = empDto.EntityStatus.LastUpdatedBy,
                EntityStatusId = (int)empDto.EntityStatus.StatusId,
                EntityStatusReasonId = (int)empDto.EntityStatus.StatusReasonId,
                OwnerId = empDto.Identity.OwnerId,
                CustomerId = empDto.Identity.CustomerId,
                ArchetypeId = (int)ArchetypeEnum.Employee,
                UT_Us = new List<UTUEntity>(),
                DateOfBirth = empDto.DateOfBirth,
                Gender = empDto.Gender,
                ForceUserLoginAgain = empDto.ForceLoginAgain ?? false
            };
            var defaultUserType = _userTypeRepository.GetById(UserTypeId);
            if (defaultUserType != null)
            {
                userNew.UT_Us.Add(new UTUEntity()
                {
                    UserId = userNew.UserId,
                    UserTypeId = defaultUserType.UserTypeId
                });
            }

            if (empDto.EntityStatus.LastExternallySynchronized.HasValue)
            {
                userNew.LastSynchronized = empDto.EntityStatus.LastExternallySynchronized.Value;
            }
            userNew.Locked = (short)(empDto.EntityStatus.ExternallyMastered ? 1 : 0);

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

            var currentOwner = _ownerRepository.GetById(_workContext.CurrentOwnerId);
            //Handle password
            if (currentOwner.UseOTP)
            {
                userNew.Password = string.IsNullOrEmpty(empDto.Password)
                    ? Encryption.Encrypt(Generator.GenerateRandomPassword(currentOwner.OTPLength, "ABCDEFGHJKLMNPQRSTWXZ23456789", false, true, bIncludeNumbers: false))
                    : Encryption.Encrypt(empDto.Password);
                userNew.OneTimePassword = Encryption.Encrypt(empDto.Password);
            }
            else
            {
                userNew.Password = string.IsNullOrEmpty(empDto.Password)
                    ? Encryption.Encrypt(Generator.GenerateRandomPassword(currentOwner.OTPLength, "ABCDEFGHJKLMNPQRSTWXZ23456789", false, true, bIncludeNumbers: false))
                    : Encryption.Encrypt(empDto.Password);
                userNew.OneTimePassword = Encryption.Encrypt(string.Empty);
            }
            //TODO
            //var userType = GetDefaultRegistrationUserType(parentHd);
            //if (userType != null)
            //    userNew.UT_Us.Add(new UTUEntity(){ UserId = userNew.UserId, UserTypeId= userType.UserTypeId });
            return userNew;
        }

        private string GenerateUserName(EmployeeDto empDto)
        {
            //Take from email
            if (!string.IsNullOrEmpty(empDto.EmailAddress))
            {
                var usersByUserName = _userRepository.GetUsersByUsername(_workContext.CurrentOwnerId, empDto.EmailAddress, EntityStatusEnum.All);
                if (!usersByUserName.Any())
                {
                    return empDto.EmailAddress;
                }
            }

            //Using 4 first letters in firstname and 4 first letters in lastname
            try
            {
                var fourFirstLettersOfFirstName = empDto.FirstName;
                if (!string.IsNullOrEmpty(empDto.FirstName) && empDto.FirstName.Length >= 4)
                {
                    fourFirstLettersOfFirstName = empDto.FirstName.Substring(0, 4);
                }
                var fourFirstLettersOfLastName = empDto.LastName;
                if (!string.IsNullOrEmpty(empDto.FirstName) && empDto.FirstName.Length >= 4)
                {
                    fourFirstLettersOfLastName = empDto.LastName.Substring(0, 4);
                }
                var username = RemoveSpecialCharactersAndSpaces(string.Format("{0}{1}", fourFirstLettersOfFirstName, fourFirstLettersOfLastName));
                if (!string.IsNullOrEmpty(username))
                {
                    var usersByUserName = _userRepository.GetUsersByUsername(_workContext.CurrentOwnerId, username, EntityStatusEnum.All);
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

            //Generate a GUID as username
            return Guid.NewGuid().ToString();
        }

    }
}
