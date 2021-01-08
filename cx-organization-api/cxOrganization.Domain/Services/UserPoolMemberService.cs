using cxOrganization.Client;
using cxOrganization.Domain.Entities;
using cxOrganization.Domain.Mappings;
using cxOrganization.Domain.Repositories;
using cxOrganization.Domain.Validators;
using cxPlatform.Client.ConexusBase;
using cxPlatform.Core.Exceptions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;

namespace cxOrganization.Domain.Services
{
    public class UserPoolMemberService : IUserPoolMemberService
    {
        private readonly IUserGroupRepository _userGroupRepository;
        private readonly OrganizationDbContext _organizationDbContext;
        private readonly IUGMemberRepository _uGMemberRepository;
        private readonly IUserValidator _userValidator;
        private readonly IUserPoolValidator _userPoolValidator;
        private readonly IUserGroupUserMappingService _userGroupUserMappingService;
        private readonly ILogger<UserPoolMemberService> _logger;

        public UserPoolMemberService(IUserGroupRepository userGroupRepository,
            OrganizationDbContext organizationDbContext,
            IUGMemberRepository uGMemberRepository,
            IUserGroupUserMappingService userGroupUserMappingService,
            IServiceProvider serviceProvider,
            ILogger<UserPoolMemberService> logger)
        {
            _userGroupRepository = userGroupRepository;
            _organizationDbContext = organizationDbContext;
            _uGMemberRepository = uGMemberRepository;
            _userValidator = serviceProvider.GetService<UserValidator>();
            _userPoolValidator = serviceProvider.GetService<UserPoolValidator>();
            _userGroupUserMappingService = userGroupUserMappingService;
            _logger = logger;
        }
        
        public List<MembershipDto> GetMembers(int userPoolId)
        {
            var result = new List<MembershipDto>();
            var userGroupEntity = _userGroupRepository.GetUserGroupById(userPoolId, false, false);
            if (userGroupEntity == null)
            {
                throw new CXValidationException(cxExceptionCodes.ERROR_IDENTITY_NOT_FOUND);
            }
            
            var activeMembers = GetActiveMembers(userGroupEntity.UserGroupId, true);

            foreach (var member in activeMembers)
            {
                var user = member.User;
                if (user != null)
                {
                    var memberDto = _userGroupUserMappingService
                        .ToMembershipDto(member);
                    result.Add(memberDto);
                }
            }
            return result;
        }

        public Dictionary<int, int> CountMemberGroupByUserPools(List<int> userPoolIds)
        {
            if (userPoolIds == null || !userPoolIds.Any()) return new Dictionary<int, int>();

            return _uGMemberRepository
                    .CountMemberGroupByUserGroup(
                        userGroupIds: userPoolIds,
                        userGroupArchetypeIds: new List<ArchetypeEnum>() { ArchetypeEnum.UserPool },
                        validFromBefore: DateTime.Now,
                        validToAfter: DateTime.Now);
        }

        private IEnumerable<UGMemberEntity> GetActiveMembers(int userGroupId, bool includeUser, List<int> userIds = null)
        {
            return _uGMemberRepository
                .GetUGMembers(
                    userGroupIds: new List<int>() { userGroupId },
                    userGroupArchetypeIds: new List<ArchetypeEnum>() { ArchetypeEnum.UserPool },
                    userIds: userIds,
                    includeUser: includeUser,
                    validFromBefore: DateTime.Now,
                    validToAfter: DateTime.Now);
        }

        public List<MembershipDto> AddMembers(int userPoolId, List<MembershipDto> membershipDtos)
        {
            var userGroup = _userPoolValidator.ValidateMemberships(userPoolId, membershipDtos);
            var userIds = membershipDtos.Where(m => m.MemberId > 0).Select(m => m.MemberId.Value).ToList();

            var existingUGMembers = GetActiveMembers(userGroup.UserGroupId, true, userIds);
            var userIdsExistingMembers = existingUGMembers.Where(ugm => ugm.UserId.HasValue).Select(ugm => ugm.UserId.Value).ToList();
            var userIdsForAddingMembers = userIds.Except(userIdsExistingMembers);
            var membershipDtosForAddingMembers = membershipDtos
                .Where(m => m.MemberId.HasValue && userIdsForAddingMembers.Contains(m.MemberId.Value));

            var insertedUGMemberEntities = new List<UGMemberEntity>();

            // Insert membership
            foreach (var membershipDto in membershipDtosForAddingMembers)
            {
                var userEntity = _userValidator.ValidateMember(membershipDto.MemberId.Value);
                var ugMemberEntity = new UGMemberEntity();
                ugMemberEntity = _userGroupUserMappingService.ToUGMemberEntity(membershipDto, ugMemberEntity);
                ugMemberEntity = _uGMemberRepository.Insert(ugMemberEntity);
                if (ugMemberEntity != null)
                {
                    ugMemberEntity.User = userEntity;
                    insertedUGMemberEntities.Add(ugMemberEntity);
                }
                else
                {
                    _logger.LogError(
                        $"Adding new membership failed with unknown reason: {JsonConvert.SerializeObject(ugMemberEntity)}");
                }
            }
            if (insertedUGMemberEntities.Any())
            {
                _organizationDbContext.SaveChanges();
            }

            var result = new List<MembershipDto>();
            foreach (var ugm in insertedUGMemberEntities)
            {
                result.Add(_userGroupUserMappingService.ToMembershipDto(ugm));
            }
            foreach (var ugm in existingUGMembers)
            {
                result.Add(_userGroupUserMappingService.ToMembershipDto(ugm));
            }

            return result;
        }

        public List<MembershipDto> RemoveMembers(int userPoolId, List<MembershipDto> membershipDtos)
        {
            var userGroup = _userPoolValidator.ValidateMemberships(userPoolId, membershipDtos);
            var userIds = membershipDtos.Where(m => m.MemberId > 0).Select(m => m.MemberId.Value).ToList();
            var existingUGMembers = GetActiveMembers(userGroup.UserGroupId, true, userIds);

            foreach (var membershipDto in membershipDtos)
            {
                var ugm = existingUGMembers.FirstOrDefault(m => m.UserId == membershipDto.MemberId);
                if(ugm != null)
                {
                    var userGroupUserEntity = _userGroupUserMappingService.ToUGMemberEntity(membershipDto, ugm);
                    userGroupUserEntity.EntityStatusId = (int)EntityStatusEnum.Deactive;    // Force to set Deactivated.
                    userGroupUserEntity.EntityStatusReasonId = (int)EntityStatusReasonEnum.Deactive_None;
                    _uGMemberRepository.Update(userGroupUserEntity);
                }
                else
                {
                    _logger.LogWarning(
                        $"Deactivating membership failed because of Not found membership: {JsonConvert.SerializeObject(membershipDto)}");
                }
            }
            if(existingUGMembers.Any()) { _organizationDbContext.SaveChanges(); }

            var result = new List<MembershipDto>();
            foreach (var ugm in existingUGMembers)
            {
                result.Add(_userGroupUserMappingService.ToMembershipDto(ugm));
            }
            return result;
        }
    }
}
