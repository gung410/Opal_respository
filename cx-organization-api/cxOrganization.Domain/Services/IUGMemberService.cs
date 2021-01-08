using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using cxOrganization.Client;
using cxOrganization.Client.UserGroups;
using cxOrganization.Domain.Enums;
using cxPlatform.Client.ConexusBase;

namespace cxOrganization.Domain.Services
{
    public interface IUGMemberService
    {
        /// <summary>
        /// Insert membership
        /// </summary>
        /// <param name="userid"></param>
        /// <param name="memberDto"></param>
        /// <returns></returns>
        MemberDto InsertUserGroupUserMembership(int? userId, MemberDto memberDto);
        /// <summary>
        /// Update membership
        /// </summary>
        /// <param name="userid"></param>
        /// <param name="memberDto"></param>
        /// <returns></returns>
        MemberDto UpdateUserGroupUserMembership(int? userId, MemberDto memberDto);
        MembershipDto Insert(MembershipDto membershipDto);
        MembershipDto Update(MembershipDto membershipDto);
        List<MembershipDto> GetMemberships(
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
            bool includeUser = false);
        List<MembershipDto> Insert(List<MembershipDto> membershipDtos);
        List<MembershipDto> Update(List<MembershipDto> membershipDtos);


        Dictionary<int, int> CountMemberGroupByUserGroup(
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
            string userSearchKey = null);

          Task<Dictionary<int, int>> CountMemberGroupByUserGroupAsync(
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
            string userSearchKey = null);


    }
}

