using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using cxEvent.Client;
using cxOrganization.Client;
using cxOrganization.Client.UserGroups;
using cxOrganization.Domain.AdvancedWorkContext;
using cxOrganization.Domain.ApiClient;
using cxOrganization.Domain.Common;
using cxOrganization.Domain.Entities;
using cxOrganization.Domain.Enums;
using cxOrganization.Domain.Extensions;
using cxOrganization.Domain.Mappings;
using cxOrganization.Domain.Repositories;
using cxOrganization.Domain.Validators;
using cxPlatform.Client.ConexusBase;
using cxPlatform.Core;
using cxPlatform.Core.Exceptions;

namespace cxOrganization.Domain.Services
{
    public class UGMemberService : IUGMemberService
    {
        private readonly OrganizationDbContext _organizationDbContext;
        private readonly IUserGroupValidator _userGroupValidator;
        private readonly IUserValidator _userValidator;
        private readonly IUGMemberRepository _userGroupUserRepository;
        private readonly IUserGroupUserMappingService _userGroupUserMappingService;
        private readonly IUGMemberValidator _uGMemberValidator;
        private readonly IUGMemberRepository _uGMemberRepository;
        private readonly IEventLogDomainApiClient _eventClientService;
        private readonly IAdvancedWorkContext _workContext;

        private const string EventObjectName = "UGMEMBER";
        public UGMemberService(
            OrganizationDbContext organizationDbContext,
            IUserGroupValidator userGroupValidator,
            IUserValidator userValidator,
            IUGMemberRepository userGroupUserRepository,
            IUserGroupUserMappingService userGroupUserMappingService,
            IUGMemberValidator uGMemberValidator,
            IUGMemberRepository uGMemberRepository,
            IEventLogDomainApiClient eventClientService,
            IAdvancedWorkContext workContext)
        {
            _organizationDbContext = organizationDbContext;
            _userGroupValidator = userGroupValidator;
            _userValidator = userValidator;
            _userGroupUserRepository = userGroupUserRepository;
            _userGroupUserMappingService = userGroupUserMappingService;
            _uGMemberValidator = uGMemberValidator;
            _uGMemberRepository = uGMemberRepository;
            _eventClientService = eventClientService;
            _workContext = workContext;
        }
        public MemberDto InsertUserGroupUserMembership(int? userId, MemberDto memberDto)
        {
            if (userId.HasValue)
                _userValidator.ValidateMember(userId.Value);

            _userGroupValidator.ValidateMembership(memberDto);
            UGMemberEntity userGroupUserEntity = new UGMemberEntity();


            userGroupUserEntity = _userGroupUserMappingService.ToUGMemberEntity(userId, memberDto, userGroupUserEntity);
            _userGroupUserRepository.Insert(userGroupUserEntity);
            _organizationDbContext.SaveChanges();

            var insertedMemberDto = _userGroupUserMappingService.ToMemberDto(userGroupUserEntity, memberDto.Identity.Archetype);

            //Insert domain event
            //InsertEvent(new RequestContext(_workContext), (int) insertedMemberDto.Identity.Id, insertedMemberDto.Identity.Archetype,
            //    userId, ArchetypeEnum.Unknown, insertedMemberDto, EventType.CREATED);


            return memberDto;

        }

        public MemberDto UpdateUserGroupUserMembership(int? userId, MemberDto memberDto)
        {
            if (userId.HasValue)
                _userValidator.ValidateMember(userId.Value);
            _userGroupValidator.ValidateMembership(memberDto);

            if (!userId.HasValue && !memberDto.UserGroupMemberId.HasValue)
            {
                throw new CXValidationException(cxExceptionCodes.ERROR_CUSTOM, "Must provide at least UserId or UserGroupMemberId");
            }

            var existingUGMember = _userGroupUserRepository
                .GetUGMembers(userIds: userId.HasValue ? new List<int> { userId.Value } : null,
                    userGroupIds: new List<int> { (int)memberDto.Identity.Id },
                    ugmemberIds: memberDto.UserGroupMemberId.HasValue
                        ? new List<long> { memberDto.UserGroupMemberId.Value }
                        : new List<long>()).FirstOrDefault();
            if (existingUGMember == null)
            {
                throw new CXValidationException(cxExceptionCodes.ERROR_IDENTITY_NOT_FOUND);
            }



            var newUGMember = _userGroupUserMappingService.ToUGMemberEntity(userId, memberDto, existingUGMember);

            _userGroupUserRepository.Update(newUGMember);
            _organizationDbContext.SaveChanges();
            var updatedMemberDto = _userGroupUserMappingService.ToMemberDto(newUGMember, memberDto.Identity.Archetype);

            var isChangedStatus = newUGMember.EntityStatusId != existingUGMember.EntityStatusId;
            var requestContext = new RequestContext(_workContext);

            if (isChangedStatus)
            {
                //If status is changed we write an event with type <name>_ENTITYSTATUS_CHANGED, 
                var statusChangedInfo = DomainHelper.BuildEntityStatusChangedEventInfo(existingUGMember.EntityStatusId, existingUGMember.EntityStatusReasonId, newUGMember.EntityStatusId, newUGMember.EntityStatusReasonId);

                InsertEvent(requestContext, (int)updatedMemberDto.Identity.Id, updatedMemberDto.Identity.Archetype, userId, ArchetypeEnum.Unknown, statusChangedInfo, EventType.ENTITYSTATUS_CHANGED);

                //Check if info are also changed, we write more an event with type <name>_UPDATED
                if (IsChangedInfo(existingUGMember, newUGMember))
                {
                    InsertEvent(requestContext, (int)updatedMemberDto.Identity.Id, updatedMemberDto.Identity.Archetype, userId, ArchetypeEnum.Unknown, updatedMemberDto, EventType.UPDATED);

                }
            }
            else
            {
                InsertEvent(requestContext, (int)updatedMemberDto.Identity.Id, updatedMemberDto.Identity.Archetype, userId, ArchetypeEnum.Unknown, updatedMemberDto, EventType.UPDATED);

            }
            return updatedMemberDto;
        }

        public MembershipDto Insert(MembershipDto membershipDto)
        {
            var memberEntityWrapper = BuildUGMemberEntityWrapper(membershipDto);

            memberEntityWrapper.NewUGMemberEntity = _userGroupUserRepository.Insert(memberEntityWrapper.NewUGMemberEntity);
            _organizationDbContext.SaveChanges();

            var insertedMembershipDto = HandleResultOfInsertingUGMember(new RequestContext(_workContext), memberEntityWrapper);

            return insertedMembershipDto;

        }



        public List<MembershipDto> Insert(List<MembershipDto> membershipDtos)
        {
            var ugMemberEntityWrappers = membershipDtos.Select(BuildUGMemberEntityWrapper).ToList();

            foreach (var ugMemberEntityWrapper in ugMemberEntityWrappers)
            {
                ugMemberEntityWrapper.NewUGMemberEntity = _userGroupUserRepository.Insert(ugMemberEntityWrapper.NewUGMemberEntity);
            }
            _organizationDbContext.SaveChanges();

            var insertedMembershipDtos = new List<MembershipDto>();
            var requestContext = new RequestContext(_workContext);

            foreach (var ugMemberEntityWrapper in ugMemberEntityWrappers)
            {

                var insertedMembershipDto = HandleResultOfInsertingUGMember(requestContext, ugMemberEntityWrapper);
                insertedMembershipDtos.Add(insertedMembershipDto);
            }
            return insertedMembershipDtos;
        }


        public List<MembershipDto> Update(List<MembershipDto> membershipDtos)
        {
            var ugMemberEntityWrappers = membershipDtos.Select(BuildUGMemberEntityWrapper).ToList();

            foreach (var ugMemberEntityWrapper in ugMemberEntityWrappers)
            {
                ugMemberEntityWrapper.NewUGMemberEntity = _userGroupUserRepository.Update(ugMemberEntityWrapper.NewUGMemberEntity);
            }
            _organizationDbContext.SaveChanges();

            var requestContext = new RequestContext(_workContext);
            var updatedMembershipDtos = new List<MembershipDto>();
            foreach (var ugMemberWrapper in ugMemberEntityWrappers)
            {
                var updatedMembershipDto = HandleResultOfUpdatingUGMember(requestContext, ugMemberWrapper);

                updatedMembershipDtos.Add(updatedMembershipDto);
            }

            return updatedMembershipDtos;
        }


        public MembershipDto Update(MembershipDto membershipDto)
        {
            var ugMemberEntityWrapper = BuildUGMemberEntityWrapper(membershipDto);

            ugMemberEntityWrapper.NewUGMemberEntity = _userGroupUserRepository.Update(ugMemberEntityWrapper.NewUGMemberEntity);
            _organizationDbContext.SaveChanges();
            return HandleResultOfUpdatingUGMember(new RequestContext(_workContext), ugMemberEntityWrapper);
        }

        private UGMemberEntityWrapper BuildUGMemberEntityWrapper(MembershipDto membershipDto)
        {
            var userEntity = membershipDto.MemberId.HasValue ? _userValidator.ValidateMember(membershipDto.MemberId.Value) : null;
            var userGroupEntity = _userGroupValidator.Validate(membershipDto.GroupId);

            var existingUgMemberEntity = _uGMemberValidator.Validate(membershipDto);

            var ugMemberEntityWrapper = new UGMemberEntityWrapper
            {
                UserEntity = userEntity,
                UserGroupEntity = userGroupEntity
            };

            UGMemberEntity oldEntityForCheckingChange = null;
            if (existingUgMemberEntity != null)
            {
                ugMemberEntityWrapper.IsStatusChanged = existingUgMemberEntity.EntityStatusId != (int)membershipDto.EntityStatus.StatusId;
                ugMemberEntityWrapper.OldEntityStatus = existingUgMemberEntity.EntityStatusId;
                ugMemberEntityWrapper.OldEntityStatusReason = existingUgMemberEntity.EntityStatusReasonId;

                if (ugMemberEntityWrapper.IsStatusChanged) //Only need this when status is changed
                {
                    oldEntityForCheckingChange = CloneUGMmemberEntityForCheckingChange(existingUgMemberEntity);
                }

            }
            ugMemberEntityWrapper.NewUGMemberEntity = _userGroupUserMappingService.ToUGMemberEntity(membershipDto, existingUgMemberEntity);

            ugMemberEntityWrapper.IsChangedInfo = IsChangedInfo(oldEntityForCheckingChange, ugMemberEntityWrapper.NewUGMemberEntity);

            return ugMemberEntityWrapper;
        }

        public List<MembershipDto> GetMemberships(
            int ownerId = 0,
            List<int> customerIds = null,
            List<ArchetypeEnum> userGroupArchetypeIds = null,
            List<string> userGroupExtIds = null,
            List<string> userGroupReferrerTokens = null,
            List<string> userGroupReferrerResources = null,
            List<ArchetypeEnum> userGroupReferrerArchetypes = null,
            List<GrouptypeEnum> userGroupTypeIds = null,
            List<EntityStatusEnum> userGroupStatus = null,
            List<string> userGroupReferrerCxTokens = null,
            List<long> userGroupUserIds = null,
            List<int> userIds = null,
            List<int> userGroupIds = null,
            List<EntityStatusEnum> ugMemberStatuses = null,
            List<string> userExtIds = null,
            List<ArchetypeEnum> userArchetypes = null,
            List<string> referrerTokens = null,
            List<string> referrerResources = null,
            List<ArchetypeEnum> referrerArchetypes = null,
            List<AgeRange> memberAgeRanges = null,
            List<Gender> memberGenders = null,
            DateTime? validFromBefore = null,
            DateTime? validFromAfter = null,
            DateTime? validToBefore = null,
            DateTime? validToAfter = null,
            bool includeUser = false)
        {
            var ugMembers = _uGMemberRepository.GetUGMembers(ownerId: ownerId,
                        customerIds: customerIds,
                        userGroupIds: userGroupIds,
                        ugMemberStatuses: ugMemberStatuses,
                        userIds: userIds,
                        userExtIds: userExtIds,
                        userArchetypes: userArchetypes,
                        referrerResources: referrerResources,
                        referrerTokens: referrerTokens,
                        referrerArchetypes: referrerArchetypes,
                        memberGenders: memberGenders,
                        memberAgeRanges: memberAgeRanges,
                        validFromAfter: validFromAfter,
                        validFromBefore: validFromBefore,
                        validToAfter: validToAfter,
                        validToBefore: validToBefore,
                        userGroupArchetypeIds: userGroupArchetypeIds,
                        userGroupReferrerArchetypes: userGroupReferrerArchetypes,
                        userGroupReferrerResources: userGroupReferrerResources,
                        userGroupReferrerCxTokens: userGroupReferrerCxTokens,
                        userGroupReferrerTokens: userGroupReferrerTokens,
                        userGroupTypeIds: userGroupTypeIds,
                        userGroupExtIds: userGroupExtIds,
                        userGroupStatus: userGroupStatus,
                        ugmemberIds: userGroupUserIds,
                        includeUser: includeUser);
            var results = new List<MembershipDto>();
            foreach (var ugMember in ugMembers)
            {
                results.Add(_userGroupUserMappingService.ToMembershipDto(ugMember));
            }
            return results;
        }

        private bool IsChangedInfo(UGMemberEntity oldUGMemberEntity, UGMemberEntity newUGMemberEntity)
        {
            if (oldUGMemberEntity == null || newUGMemberEntity == null) return false;
            return oldUGMemberEntity.DisplayName != newUGMemberEntity.DisplayName
                   || oldUGMemberEntity.ReferrerResource != newUGMemberEntity.ReferrerResource
                   || oldUGMemberEntity.ReferrerToken != newUGMemberEntity.ReferrerToken
                   || oldUGMemberEntity.ReferrerArchetypeId != newUGMemberEntity.ReferrerArchetypeId
                   || oldUGMemberEntity.PeriodId != newUGMemberEntity.PeriodId
                   || oldUGMemberEntity.ExtId != newUGMemberEntity.ExtId
                   || oldUGMemberEntity.UserId != newUGMemberEntity.UserId
                   || oldUGMemberEntity.MemberRoleId != newUGMemberEntity.MemberRoleId;


        }
        private UGMemberEntity CloneUGMmemberEntityForCheckingChange(UGMemberEntity entity)
        {
            return new UGMemberEntity
            {
                UserId = entity.UserId,
                ReferrerResource = entity.ReferrerResource,
                ReferrerArchetypeId = entity.ReferrerArchetypeId,
                ReferrerToken = entity.ReferrerToken,
                DisplayName = entity.DisplayName,
                ExtId = entity.ExtId,
                EntityStatusReasonId = entity.EntityStatusReasonId,
                EntityStatusId = entity.EntityStatusId,
                MemberRoleId = entity.MemberRoleId,
                PeriodId = entity.PeriodId
            };
        }

        private void InsertEvent(RequestContext requestContext, int userGroupId, ArchetypeEnum userGroupArchetype, int? userId, ArchetypeEnum userArchetype, object additionalInformation, EventType eventType)
        {
            DomainEventDto eventDto = new DomainEventBuilder()
              .CreatedDate(DateTime.Now)
              .InGroup(userGroupId, userGroupArchetype)
              .WithAdditionalInformation(additionalInformation)
              .WithEventTypeName(eventType.ToEventTypeName(EventObjectName))
              .CreateWithRequestContext(requestContext);

            if (userId.HasValue)
            {
                eventDto.UserIdentity = new IdentityBaseDto()
                {
                    Id = userId,
                    Archetype = userArchetype
                };
            }

            _eventClientService.WriteDomainEvent(eventDto, requestContext);
        }
        private MembershipDto HandleResultOfInsertingUGMember(RequestContext RequestContext, UGMemberEntityWrapper memberEntityWrapper)
        {
            var insertedMembershipDto = _userGroupUserMappingService.ToMembershipDto(memberEntityWrapper.NewUGMemberEntity);

            var userGroupArchetype = (ArchetypeEnum)(memberEntityWrapper.UserGroupEntity.ArchetypeId ?? 0);
            var userArchetype = (ArchetypeEnum)(memberEntityWrapper.UserEntity == null ? 0 : memberEntityWrapper.UserEntity.ArchetypeId ?? 0);

            InsertEvent(RequestContext, memberEntityWrapper.NewUGMemberEntity.UserGroupId, userGroupArchetype,
                memberEntityWrapper.NewUGMemberEntity.UserId, userArchetype, insertedMembershipDto, EventType.CREATED);
            return insertedMembershipDto;
        }
        private MembershipDto HandleResultOfUpdatingUGMember(RequestContext requestContext, UGMemberEntityWrapper ugMemberWrapper)
        {
            var userGroupArchetype = (ArchetypeEnum)(ugMemberWrapper.UserGroupEntity.ArchetypeId ?? 0);
            var userArchetype = (ArchetypeEnum)(ugMemberWrapper.UserEntity == null ? 0 : ugMemberWrapper.UserEntity.ArchetypeId ?? 0);

            var updatedMembershipDto = _userGroupUserMappingService.ToMembershipDto(ugMemberWrapper.NewUGMemberEntity);

            if (ugMemberWrapper.IsStatusChanged)
            {
                var statusChangedInfo = DomainHelper.BuildEntityStatusChangedEventInfo(ugMemberWrapper.OldEntityStatus, ugMemberWrapper.OldEntityStatusReason,
                    ugMemberWrapper.NewUGMemberEntity.EntityStatusId, ugMemberWrapper.NewUGMemberEntity.EntityStatusReasonId);

                InsertEvent(requestContext, ugMemberWrapper.NewUGMemberEntity.UserGroupId, userGroupArchetype, ugMemberWrapper.NewUGMemberEntity.UserId, userArchetype, statusChangedInfo,
                    EventType.ENTITYSTATUS_CHANGED);

                if (ugMemberWrapper.IsChangedInfo)
                {
                    InsertEvent(requestContext, ugMemberWrapper.NewUGMemberEntity.UserGroupId, userGroupArchetype, ugMemberWrapper.NewUGMemberEntity.UserId, userArchetype, updatedMembershipDto,
                        EventType.UPDATED);
                }
            }
            else
            {
                InsertEvent(requestContext, ugMemberWrapper.NewUGMemberEntity.UserGroupId, userGroupArchetype, ugMemberWrapper.NewUGMemberEntity.UserId, userArchetype, updatedMembershipDto,
                    EventType.UPDATED);
            }
            return updatedMembershipDto;
        }

        public Dictionary<int, int> CountMemberGroupByUserGroup(
            int ownerId = 0,
            List<int> customerIds = null,
            List<ArchetypeEnum> userGroupArchetypeIds = null,
            List<string> userGroupExtIds = null,
            List<string> userGroupReferrerTokens = null,
            List<string> userGroupReferrerResources = null,
            List<ArchetypeEnum> userGroupReferrerArchetypes = null,
            List<GrouptypeEnum> userGroupTypeIds = null,
            List<EntityStatusEnum> userGroupStatus = null,
            List<string> userGroupReferrerCxTokens = null,
            List<long> ugMemberIds = null,
            List<int> userIds = null,
            List<int> userGroupIds = null,
            List<EntityStatusEnum> ugMemberStatus = null,
            List<string> userExtIds = null,
            List<ArchetypeEnum> userArchetypes = null,
            List<string> referrerTokens = null,
            List<string> referrerResources = null,
            List<ArchetypeEnum> referrerArchetypes = null,
            List<AgeRange> memberAgeRanges = null,
            List<Gender> memberGenders = null,
            DateTime? validFromBefore = null,
            DateTime? validFromAfter = null,
            DateTime? validToBefore = null,
            DateTime? validToAfter = null,
            DateTime? createdBefore = null,
            DateTime? createdAfter = null,
           string userSearchKey = null)
        {
            return _uGMemberRepository.CountMemberGroupByUserGroup(ownerId: ownerId,
                customerIds: customerIds,
                userGroupArchetypeIds: userGroupArchetypeIds,
                userGroupExtIds: userGroupExtIds,
                userGroupReferrerTokens: userGroupReferrerTokens,
                userGroupReferrerResources: userGroupReferrerResources,
                userGroupReferrerArchetypes: userGroupReferrerArchetypes,
                userGroupTypeIds: userGroupTypeIds,
                userGroupStatus: userGroupStatus,
                userGroupReferrerCxTokens: userGroupReferrerCxTokens,
                ugMemberIds: ugMemberIds,
                userIds: userIds,
                userGroupIds: userGroupIds,
                ugMemberStatus: userGroupStatus,
                userExtIds: userExtIds,
                userArchetypes: userArchetypes,
                referrerTokens: referrerTokens,
                referrerResources: referrerResources,
                referrerArchetypes: referrerArchetypes,
                memberAgeRanges: memberAgeRanges,
                memberGenders: memberGenders,
                validFromBefore: validFromBefore,
                validFromAfter: validFromAfter,
                validToBefore: validToBefore,
                validToAfter: validToAfter,
                createdBefore: createdBefore,
                createdAfter: createdAfter,
                userSearchKey: userSearchKey);
        }
        public async Task<Dictionary<int, int>> CountMemberGroupByUserGroupAsync(
            int ownerId = 0,
            List<int> customerIds = null,
            List<ArchetypeEnum> userGroupArchetypeIds = null,
            List<string> userGroupExtIds = null,
            List<string> userGroupReferrerTokens = null,
            List<string> userGroupReferrerResources = null,
            List<ArchetypeEnum> userGroupReferrerArchetypes = null,
            List<GrouptypeEnum> userGroupTypeIds = null,
            List<EntityStatusEnum> userGroupStatus = null,
            List<string> userGroupReferrerCxTokens = null,
            List<long> ugMemberIds = null,
            List<int> userIds = null,
            List<int> userGroupIds = null,
            List<EntityStatusEnum> ugMemberStatus = null,
            List<string> userExtIds = null,
            List<ArchetypeEnum> userArchetypes = null,
            List<string> referrerTokens = null,
            List<string> referrerResources = null,
            List<ArchetypeEnum> referrerArchetypes = null,
            List<AgeRange> memberAgeRanges = null,
            List<Gender> memberGenders = null,
            DateTime? validFromBefore = null,
            DateTime? validFromAfter = null,
            DateTime? validToBefore = null,
            DateTime? validToAfter = null,
            DateTime? createdBefore = null,
            DateTime? createdAfter = null,
           string userSearchKey = null)
        {
            return await _uGMemberRepository.CountMemberGroupByUserGroupAsync(ownerId: ownerId,
                customerIds: customerIds,
                userGroupArchetypeIds: userGroupArchetypeIds,
                userGroupExtIds: userGroupExtIds,
                userGroupReferrerTokens: userGroupReferrerTokens,
                userGroupReferrerResources: userGroupReferrerResources,
                userGroupReferrerArchetypes: userGroupReferrerArchetypes,
                userGroupTypeIds: userGroupTypeIds,
                userGroupStatus: userGroupStatus,
                userGroupReferrerCxTokens: userGroupReferrerCxTokens,
                ugMemberIds: ugMemberIds,
                userIds: userIds,
                userGroupIds: userGroupIds,
                ugMemberStatus: userGroupStatus,
                userExtIds: userExtIds,
                userArchetypes: userArchetypes,
                referrerTokens: referrerTokens,
                referrerResources: referrerResources,
                referrerArchetypes: referrerArchetypes,
                memberAgeRanges: memberAgeRanges,
                memberGenders: memberGenders,
                validFromBefore: validFromBefore,
                validFromAfter: validFromAfter,
                validToBefore: validToBefore,
                validToAfter: validToAfter,
                createdBefore: createdBefore,
                createdAfter: createdAfter,
                userSearchKey: userSearchKey);
        }


        /// <summary>
        /// A wrapper that is for processing insert/update UgMemberEntity
        /// </summary>
        private class UGMemberEntityWrapper
        {
            public UserGroupEntity UserGroupEntity { get; set; }
            public UserEntity UserEntity { get; set; }

            public UGMemberEntity NewUGMemberEntity { get; set; }

            public int? OldEntityStatus { get; set; }
            public int? OldEntityStatusReason { get; set; }
            public bool IsStatusChanged { get; set; }

            public bool IsChangedInfo { get; set; }
        }
    }
}
