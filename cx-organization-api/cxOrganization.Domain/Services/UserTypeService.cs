using System.Collections.Generic;
using System.Linq;
using cxOrganization.Client;
using cxOrganization.Client.UserTypes;
using cxOrganization.Domain.AdvancedWorkContext;
using cxOrganization.Domain.Entities;
using cxOrganization.Domain.Mappings;
using cxOrganization.Domain.Repositories;
using cxOrganization.Domain.Validators;
using cxOrganization.Domain.Validators.UserTypes;
using cxPlatform.Client.ConexusBase;
using cxPlatform.Core;
using cxPlatform.Core.Exceptions;

namespace cxOrganization.Domain.Services
{
    public class UserTypeService : IUserTypeService
    {
        private readonly IUserTypeRepository _userTypeRepository;
        private readonly IUserRepository _userRepository;
        private readonly IAdvancedWorkContext _workContext;
        private readonly OrganizationDbContext _organizationDbContext;
        private readonly IUserTypeValidator _userTypeValidator;
        private readonly IUserValidator _userValidator;
        private readonly IUserTypeMappingService _userTypeMappingService;
        public UserTypeService(IUserTypeRepository userTypeRepository,
            IAdvancedWorkContext workContext,
            OrganizationDbContext organizationDbContext,
            IUserRepository userRepository,
            IUserTypeValidator userTypeValidator,
            IUserValidator userValidator,
            IUserTypeMappingService userTypeMappingService)
        {
            _userTypeRepository = userTypeRepository;
            _workContext = workContext;
            _organizationDbContext = organizationDbContext;
            _userRepository = userRepository;
            _userTypeValidator = userTypeValidator;
            _userValidator = userValidator;
            _userTypeMappingService = userTypeMappingService;
        }

        public List<UserTypeDto> GetUserTypes(int ownerId, List<int> userIds = null,
            List<ArchetypeEnum> archetypeIds = null,
            List<int> userTypeIds = null,
            List<string> extIds = null,
            bool includeLocalizedData = false,
            string langCode = "en-US",
            List<int> parentIds = null)
        {
            var userTypeEntities = _userTypeRepository.GetUserTypes(ownerId: ownerId,
                userIds: userIds,
                archetypeIds: archetypeIds,
                userTypeIds: userTypeIds,
                extIds: extIds,
                includeLocalizedData: includeLocalizedData,
                parentIds: parentIds);
            List<UserTypeDto> result = new List<UserTypeDto>();
            foreach (var userTypeEntity in userTypeEntities)
            {
                result.Add(_userTypeMappingService.ToUserTypeDto(userTypeEntity, langCode));
            }
            return result;
        }

        public UserTypeEntity GetUserTypeByExtId(string extId, int? archeTypeId = null)
        {
            return _userTypeRepository.GetUserTypeByExtId(extId, archeTypeId);
        }
        public List<UserTypeDto> GetUserRoles(List<string> extIds = null,
           List<ArchetypeEnum> archetypeEnums = null,
           List<int> userTypeIds = null,
           List<int> userIds = null)
        {
            var result = new List<UserTypeDto>();
            var userTypeEntityList = _userTypeRepository.GetUserTypes(_workContext.CurrentOwnerId, userIds, archetypeEnums, userTypeIds, extIds);
            foreach (var userType in userTypeEntityList)
            {
                result.Add(_userTypeMappingService.ToUserTypeDto(userType));
            }
            return result;
        }
        public MemberDto AddUserTypeUser(int userTypeId,
            MemberDto userMemberDto,
            List<int> checkingUserArchetypeIds = null)
        {
            UserEntity userEntity = new UserEntity();
            UserTypeEntity userTypeEntity = new UserTypeEntity();
            if (userMemberDto.EntityStatus.StatusId != EntityStatusEnum.Active)
            {
                throw new CXValidationException(cxExceptionCodes.ERROR_MEMBER_NOT_FOUND);
            }
            ValidationUserTypeAndUser(userTypeId, userMemberDto, ref userEntity, ref userTypeEntity, checkingUserArchetypeIds);
            var usertypeUser = userEntity.UT_Us.FirstOrDefault(t => t.UserTypeId == userTypeEntity.UserTypeId);
            if (usertypeUser != null)
            {
                return userMemberDto;
            }

            userEntity.UT_Us.Add(usertypeUser);
            _organizationDbContext.SaveChanges();
            return userMemberDto;
        }
        private void ValidationUserTypeAndUser(int userTypeId,
            MemberDto userMemberDto,
            ref UserEntity userEntity,
            ref UserTypeEntity userType,
            List<int> checkingUserArchetypeIds = null)
        {
            if (_workContext.CurrentCustomerId != userMemberDto.Identity.CustomerId
                || _workContext.CurrentOwnerId != userMemberDto.Identity.OwnerId)
            {
                throw new CXValidationException(cxExceptionCodes.ERROR_CXTOKEN_INVALID);
            }

            userEntity = _userRepository.GetUserForUpdateInsert(userId: (int)userMemberDto.Identity.Id);

            if (userEntity == null)
            {
                throw new CXValidationException(cxExceptionCodes.ERROR_USER_NOT_FOUND, userMemberDto.Identity.Id);
            }
            if (userEntity.ArchetypeId.HasValue && userEntity.ArchetypeId.Value != (int)userMemberDto.Identity.Archetype)
            {
                throw new CXValidationException(cxExceptionCodes.ERROR_ARCHETYPE_DB_NOT_MATCH);
            }
            if (checkingUserArchetypeIds != null && checkingUserArchetypeIds.Any())
            {
                if (!userEntity.ArchetypeId.HasValue || !checkingUserArchetypeIds.Contains(userEntity.ArchetypeId.Value))
                {
                    throw new CXValidationException(cxExceptionCodes.VALIDATION_MEMBER_ARCHETYPE_INCORRECTED);
                }
            }
            userType = _userTypeRepository.GetById(userTypeId);
            if (userType == null)
            {
                throw new CXValidationException(cxExceptionCodes.ERROR_NOT_FOUND, string.Format("Id : {0}", userTypeId));
            }
        }
        public MemberDto RemoveUserTypeUser(int userTypeId, MemberDto userMemberDto, List<int> checkingUserArchetypeIds = null)
        {
            UserEntity userEntity = new UserEntity();
            UserTypeEntity userTypeEntity = new UserTypeEntity();
            if (userMemberDto.EntityStatus.StatusId != EntityStatusEnum.Inactive)
            {
                throw new CXValidationException(cxExceptionCodes.ERROR_MEMBER_NOT_FOUND);
            }
            ValidationUserTypeAndUser(userTypeId, userMemberDto, ref userEntity, ref userTypeEntity, checkingUserArchetypeIds);
            var usertypeUser = userEntity.UT_Us.FirstOrDefault(t => t.UserTypeId == userTypeEntity.UserTypeId);
            if (usertypeUser == null)
            {
                return userMemberDto;
            }

            userEntity.UT_Us.Remove(usertypeUser);
            _organizationDbContext.SaveChanges();
            return userMemberDto;
        }

        public MemberDto UpdateOrInsertUserTypeUser(int userId,
            MemberDto memberDto,
            bool isUnique = false)
        {
            if (memberDto.EntityStatus.StatusId != EntityStatusEnum.Active)
            {
                throw new CXValidationException(cxExceptionCodes.VALIDATION_STATUSID_IS_INVALID);
            }
            UserEntity userEntity = _userValidator.ValidateMember(userId);
            UserTypeEntity userTypeEntity = _userTypeValidator.ValidateMembership(memberDto);
            var usertypeUser = userEntity.UT_Us.FirstOrDefault(t => t.UserTypeId == userTypeEntity.UserTypeId);
            if (usertypeUser != null)
            {
                return memberDto;
            }

            if (isUnique)
            {
                for (int i = 0; i < userEntity.UT_Us.Count; i++)
                {
                    var element = userEntity.UT_Us.ElementAt(i);
                    if (element.UserType.ArchetypeId == (short)memberDto.Identity.Archetype)
                    {
                        userEntity.UT_Us.Remove(element);
                    }
                }
                userEntity.UT_Us.Add(new UTUEntity()
                {
                    UserTypeId = userTypeEntity.UserTypeId
                });
            }
            else
            {
                userEntity.UT_Us.Add(new UTUEntity()
                {
                    UserTypeId = userTypeEntity.UserTypeId
                });
            }
            _organizationDbContext.SaveChanges();

            return memberDto;


        }
        public MemberDto DeleteUserTypeUser(int userId,
            MemberDto levelMemberDto)
        {
            if (levelMemberDto.EntityStatus.StatusId == EntityStatusEnum.Inactive || levelMemberDto.EntityStatus.StatusId == EntityStatusEnum.Deactive)
            {
                UserEntity userEntity = _userValidator.ValidateMember(userId);
                UserTypeEntity userTypeEntity = _userTypeValidator.ValidateMembership(levelMemberDto);
                var usertypeUser = userEntity.UT_Us.FirstOrDefault(t => t.UserTypeId == userTypeEntity.UserTypeId);
                if (usertypeUser == null)
                {
                    return levelMemberDto;
                }
                userEntity.UT_Us.Remove(usertypeUser);
                _organizationDbContext.SaveChanges();
            }
            else
                throw new CXValidationException(cxExceptionCodes.VALIDATION_STATUSID_IS_INVALID);

            return levelMemberDto;
        }
    }
}
