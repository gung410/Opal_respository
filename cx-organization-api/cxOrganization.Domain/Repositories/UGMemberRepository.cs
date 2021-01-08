using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using cxOrganization.Client;
using cxOrganization.Client.UserGroups;
using cxOrganization.Domain.Entities;
using cxOrganization.Domain.Enums;
using cxOrganization.Domain.Extensions;
using cxOrganization.Domain.Repositories.QueryBuilders;
using cxPlatform.Client.ConexusBase;
using cxPlatform.Core;
using cxPlatform.Core.Extentions;
using Microsoft.EntityFrameworkCore;

namespace cxOrganization.Domain.Repositories
{
    public class UGMemberRepository : RepositoryBase<UGMemberEntity>, IUGMemberRepository
    {
        public UGMemberRepository(OrganizationDbContext dbContext)
            : base(dbContext)
        {
        }

        public List<UGMemberEntity> GetUGMembers(
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
            List<long> ugmemberIds = null,
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
            bool includeUser = false,
            bool includeUserGroup = false,
            bool nonuserMemberOnly = false,
            string userSearchKey = null,
            bool includeUserGroupUser = false,
            bool disableTracker = false)
        {
            var query = UGMemberQueryBuilder.InitQueryBuilder(disableTracker ? GetAllAsNoTracking() : GetAll())
                .FilterByOwnerId(ownerId)
                .FilterByUGMemberIds(ugmemberIds)
                .FilterByCustomerIds(customerIds)
                .FilterByUserGroupIds(userGroupIds)
                .FilterByUserGroupArchetypeIds(userGroupArchetypeIds)
                .FilterByUserGroupTypes(userGroupTypeIds)
                .FilterByUserGroupExtIds(userGroupExtIds)
                .FilterByUserGroupReferrerArchetypes(userGroupReferrerArchetypes)
                .FilterByUserGroupReferrerCxTokens(userGroupReferrerCxTokens)
                .FilterByUserGroupReferrerResources(userGroupReferrerResources)
                .FilterByUserGroupReferrerTokens(userGroupReferrerTokens)
                .FilterByUserGroupStatus(userGroupStatus)
                .FilterByUserExtIds(userExtIds)
                .FilterByUserIds(userIds).FilterByReferrerArchetypes(referrerArchetypes)
                .FilterByReferrerResources(referrerResources)
                .FilterByUserArchetypes(userArchetypes)
                .FilterByReferrerTokens(referrerTokens)
                .FilterByUGMemberStatus(ugMemberStatuses)
                .FilterByAges(memberAgeRanges)
                .FilterByGenders(memberGenders)
                .FilterByDateTime(validFromAfter: validFromAfter,
                    validFromBefore: validFromBefore,
                    validToAfter: validToAfter,
                    validToBefore: validToBefore)
                .FilterByUserSearchKey(userSearchKey)
                .Build();

            if (nonuserMemberOnly)
                query = query.Where(q => q.UserId == null);

            if (includeUser)
            {
                query = query.Include(t => t.User);
            }

            if (includeUserGroupUser)
            {
                query = query.Include(t => t.UserGroup)
                    .ThenInclude(u => u.User);

            }
            else if (includeUserGroup)
            {
                query = query.Include(t => t.UserGroup);
            }


            return query.ToList();
        }
        public Task<List<UGMemberEntity>> GetUGMembersAsync(
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
            List<long> ugmemberIds = null,
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
            bool includeUser = false,
            bool includeUserGroup = false,
            bool nonuserMemberOnly = false,
            string userSearchKey = null,
            bool includeUserGroupUser = false,
            bool disableTracker = false)
        {
            var query = UGMemberQueryBuilder.InitQueryBuilder(disableTracker ? GetAllAsNoTracking() : GetAll())
                .FilterByOwnerId(ownerId)
                .FilterByUGMemberIds(ugmemberIds)
                .FilterByCustomerIds(customerIds)
                .FilterByUserGroupIds(userGroupIds)
                .FilterByUserGroupArchetypeIds(userGroupArchetypeIds)
                .FilterByUserGroupTypes(userGroupTypeIds)
                .FilterByUserGroupExtIds(userGroupExtIds)
                .FilterByUserGroupReferrerArchetypes(userGroupReferrerArchetypes)
                .FilterByUserGroupReferrerCxTokens(userGroupReferrerCxTokens)
                .FilterByUserGroupReferrerResources(userGroupReferrerResources)
                .FilterByUserGroupReferrerTokens(userGroupReferrerTokens)
                .FilterByUserGroupStatus(userGroupStatus)
                .FilterByUserExtIds(userExtIds)
                .FilterByUserIds(userIds).FilterByReferrerArchetypes(referrerArchetypes)
                .FilterByReferrerResources(referrerResources)
                .FilterByUserArchetypes(userArchetypes)
                .FilterByReferrerTokens(referrerTokens)
                .FilterByUGMemberStatus(ugMemberStatuses)
                .FilterByAges(memberAgeRanges)
                .FilterByGenders(memberGenders)
                .FilterByDateTime(validFromAfter: validFromAfter,
                    validFromBefore: validFromBefore,
                    validToAfter: validToAfter,
                    validToBefore: validToBefore)
                .FilterByUserSearchKey(userSearchKey)
                .Build();

            if (nonuserMemberOnly)
                query = query.Where(q => q.UserId == null);

            if (includeUser)
            {
                query = query.Include(t => t.User);
            }

            if (includeUserGroupUser)
            {
                query = query.Include(t => t.UserGroup)
                    .ThenInclude(u => u.User);

            }
            else if (includeUserGroup)
            {
                query = query.Include(t => t.UserGroup);
            }


            return query.ToListAsync();
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
            var query = BuildCountMemberGroupByUserGroupQuery(ownerId: ownerId,
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
                ugMemberStatus: ugMemberStatus,
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

            return query.ToList().GroupBy(q => q.UserGroupId).ToDictionary(q => q.Key, q => q.Count());
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
            var query = BuildCountMemberGroupByUserGroupQuery(ownerId: ownerId,
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
                ugMemberStatus: ugMemberStatus,
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
            var result =  await query.ToListAsync();
            return result.GroupBy(q => q.UserGroupId).ToDictionary(q => q.Key, q => q.Count());
        }

        private IQueryable<UGMemberEntity> BuildCountMemberGroupByUserGroupQuery(int ownerId, List<int> customerIds, List<ArchetypeEnum> userGroupArchetypeIds,
            List<string> userGroupExtIds, List<string> userGroupReferrerTokens, List<string> userGroupReferrerResources,
            List<ArchetypeEnum> userGroupReferrerArchetypes, List<GrouptypeEnum> userGroupTypeIds, List<EntityStatusEnum> userGroupStatus, List<string> userGroupReferrerCxTokens,
            List<long> ugMemberIds, List<int> userIds, List<int> userGroupIds, List<EntityStatusEnum> ugMemberStatus, List<string> userExtIds, List<ArchetypeEnum> userArchetypes,
            List<string> referrerTokens, List<string> referrerResources, List<ArchetypeEnum> referrerArchetypes, List<AgeRange> memberAgeRanges, List<Gender> memberGenders,
            DateTime? validFromBefore, DateTime? validFromAfter, DateTime? validToBefore, DateTime? validToAfter,
            DateTime? createdBefore, DateTime? createdAfter, string userSearchKey)
        {
            var query = UGMemberQueryBuilder.InitQueryBuilder(GetAll())
                .FilterByOwnerId(ownerId)
                .FilterByCustomerIds(customerIds)
                .FilterByUserGroupArchetypeIds(userGroupArchetypeIds)
                .FilterByUserGroupTypes(userGroupTypeIds)
                .FilterByUserGroupExtIds(userGroupExtIds)
                .FilterByUserGroupReferrerArchetypes(userGroupReferrerArchetypes)
                .FilterByUserGroupReferrerCxTokens(userGroupReferrerCxTokens)
                .FilterByUserGroupReferrerResources(userGroupReferrerResources)
                .FilterByUserGroupReferrerTokens(userGroupReferrerTokens)
                .FilterByUserGroupStatus(userGroupStatus)
                .FilterByReferrerArchetypes(referrerArchetypes)
                .FilterByReferrerResources(referrerResources)
                .FilterByUGMemberIds(ugMemberIds)
                .FilterByUGMemberStatus(ugMemberStatus)
                .FilterByUserArchetypes(userArchetypes)
                .FilterByUserExtIds(userExtIds)
                .FilterByUserGroupIds(userGroupIds)
                .FilterByUserIds(userIds)
                .FilterByReferrerTokens(referrerTokens)
                .FilterByAges(memberAgeRanges)
                .FilterByGenders(memberGenders)
                .FilterByDateTime(validFromAfter: validFromAfter,
                    validFromBefore: validFromBefore,
                    validToAfter: validToAfter,
                    validToBefore: validToBefore,
                    createdBefore: createdBefore,
                    createdAfter: createdAfter)
                .FilterByUserSearchKey(userSearchKey)
                .Build();
            return query;
        }

        public CustomPaginatedList<UGMemberEntity> GetPaginatedUGMembers(int ownerId = 0,
            List<int> customerIds = null,
            List<long> ugMemberIds = null,
            List<string> ugMemberExtIds = null,
            List<ArchetypeEnum> userGroupArchetypeIds = null,
            List<string> userGroupReferrerTokens = null,
            List<string> userGroupReferrerResources = null,
            List<ArchetypeEnum> userGroupReferrerArchetypes = null,
            List<GrouptypeEnum> userGroupTypeIds = null,
            List<EntityStatusEnum> userGroupStatuses = null,
            List<string> userGroupReferrerCxTokens = null,
            List<int> userIds = null,
            List<int> userGroupIds = null,
            List<string> userGroupExtIds = null,
            List<EntityStatusEnum> ugMemberStatus = null,
            List<string> userExtIds = null,
            List<ArchetypeEnum> userArchetypes = null,
            List<string> referrerTokens = null,
            List<string> referrerResources = null,
            List<ArchetypeEnum> referrerArchetypes = null,
            Dictionary<GrouptypeEnum, List<int>> userGroupIdsGroupsByType = null,
            List<AgeRange> memberAgeRanges = null,
            List<Gender> memberGenders = null,
            DateTime? validFromBefore = null,
            DateTime? validFromAfter = null,
            DateTime? validToBefore = null,
            DateTime? validToAfter = null,
            DateTime? createdBefore = null,
            DateTime? createdAfter = null,
            DateTime? lastUpdatedBefore = null,
            DateTime? lastUpdatedAfter = null,
            bool includeUser = false,
            bool includeUserGroup = false,
            int pageIndex = 0,
            int pageSize = 0,
            string orderBy = "",
            bool getTotalItemCount = false,
            bool distinct = false,
            string userSearchKey = null)
        {
            var query = UGMemberQueryBuilder.InitQueryBuilder(GetAll())
                .FilterByOwnerId(ownerId)
                .FilterByCustomerIds(customerIds)
                .FilterByUGMemberIds(ugMemberIds)
                .FilterByUGMemberExtIds(ugMemberExtIds)
                .FilterByUserGroupArchetypeIds(userGroupArchetypeIds)
                .FilterByUserGroupTypes(userGroupTypeIds)
                .FilterByUserGroupExtIds(userGroupExtIds)
                .FilterByUserGroupReferrerArchetypes(userGroupReferrerArchetypes)
                .FilterByUserGroupReferrerCxTokens(userGroupReferrerCxTokens)
                .FilterByUserGroupReferrerResources(userGroupReferrerResources)
                .FilterByUserGroupReferrerTokens(userGroupReferrerTokens)
                .FilterByUserGroupStatus(userGroupStatuses)
                .FilterByReferrerArchetypes(referrerArchetypes)
                .FilterByReferrerResources(referrerResources)
                .FilterByUGMemberStatus(ugMemberStatus)
                .FilterByUserArchetypes(userArchetypes)
                .FilterByUserExtIds(userExtIds)
                .FilterByUserGroupIds(userGroupIds)
                .FilterByUserIds(userIds)
                .FilterByReferrerTokens(referrerTokens)
                .FilterByAges(memberAgeRanges)
                .FilterByGenders(memberGenders)
                .FilterByDateTime(validFromAfter: validFromAfter,
                    validFromBefore: validFromBefore,
                    validToAfter: validToAfter,
                    validToBefore: validToBefore,
                    createdBefore: createdBefore,
                    createdAfter: createdAfter,
                    lastUpdatedBefore: lastUpdatedBefore,
                    lastUpdatedAfter: lastUpdatedAfter)
                .FilterByUserSearchKey(userSearchKey)
                .Build();

            if (userGroupIdsGroupsByType != null && userGroupIdsGroupsByType.Count > 0)
            {
                foreach (var usergroupIdsGroupByType in userGroupIdsGroupsByType)
                {
                    if (usergroupIdsGroupByType.Value == null || usergroupIdsGroupByType.Value.Count == 0)
                        continue;

                    var grouptypeId = (int) usergroupIdsGroupByType.Key;

                    var subQuery = UGMemberQueryBuilder.InitQueryBuilder(GetAll())
                        .FilterByUserGroupIds(usergroupIdsGroupByType.Value)
                        .FilterByUGMemberStatus(ugMemberStatus)
                        .FilterByDateTime(validFromAfter: validFromAfter,
                            validFromBefore: validFromBefore,
                            validToAfter: validToAfter,
                            validToBefore: validToBefore)
                        .Build();
                    subQuery = subQuery.Where(sm => sm.UserGroup.UserGroupTypeId == grouptypeId);

                    query = query.Where(m => m.UserId != null && subQuery.Any(sm => sm.UserId == m.UserId));
                }
            }

            if (distinct)
            {
                query = query
                    .GroupBy(q => new { q.UserId, q.ReferrerToken, q.ReferrerArchetypeId, q.ReferrerResource })
                    .Select(g => g.OrderByDescending(o => o.Created).FirstOrDefault());
            }

            if (includeUser)
            {
                query = query.Include(t => t.User);
            }

            if (includeUserGroup)
            {
                query = query.Include(t => t.UserGroup);
            }

            query = query.ApplyOrderBy(p => p.UGMemberId, orderBy);

       
            var hasMoreData = false;
            //Build paging from IQueryable
            var totalItem = 0;
            var items = query.ToPaging(pageIndex, pageSize, out hasMoreData, out totalItem);

            int? totalItemCount = null;
            if (getTotalItemCount)
            {
                totalItemCount = query.Count();
            }

            return new CustomPaginatedList<UGMemberEntity>(items, pageIndex, pageSize, hasMoreData, totalItemCount);
        }
        public async Task<int> CountUGMembersAsync(
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
        List<long> ugmemberIds = null,
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
        bool nonuserMemberOnly = false,
        string userSearchKey = null)
        {
            var query = UGMemberQueryBuilder.InitQueryBuilder(GetAllAsNoTracking())
               .FilterByOwnerId(ownerId)
               .FilterByUGMemberIds(ugmemberIds)
               .FilterByCustomerIds(customerIds)
               .FilterByUserGroupIds(userGroupIds)
               .FilterByUserGroupArchetypeIds(userGroupArchetypeIds)
               .FilterByUserGroupTypes(userGroupTypeIds)
               .FilterByUserGroupExtIds(userGroupExtIds)
               .FilterByUserGroupReferrerArchetypes(userGroupReferrerArchetypes)
               .FilterByUserGroupReferrerCxTokens(userGroupReferrerCxTokens)
               .FilterByUserGroupReferrerResources(userGroupReferrerResources)
               .FilterByUserGroupReferrerTokens(userGroupReferrerTokens)
               .FilterByUserGroupStatus(userGroupStatus)
               .FilterByUserExtIds(userExtIds)
               .FilterByUserIds(userIds).FilterByReferrerArchetypes(referrerArchetypes)
               .FilterByReferrerResources(referrerResources)
               .FilterByUserArchetypes(userArchetypes)
               .FilterByReferrerTokens(referrerTokens)
               .FilterByUGMemberStatus(ugMemberStatuses)
               .FilterByAges(memberAgeRanges)
               .FilterByGenders(memberGenders)
               .FilterByDateTime(validFromAfter: validFromAfter,
                   validFromBefore: validFromBefore,
                   validToAfter: validToAfter,
                   validToBefore: validToBefore)
               .FilterByUserSearchKey(userSearchKey)
               .Build();

            if (nonuserMemberOnly)
                query = query.Where(q => q.UserId == null);

         

            return await query.CountAsync();
        }

        public int CountUGMembers(
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
            List<long> ugmemberIds = null,
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
            bool nonuserMemberOnly = false,
            string userSearchKey = null)
        {
            var query = UGMemberQueryBuilder.InitQueryBuilder(GetAllAsNoTracking())
                .FilterByOwnerId(ownerId)
                .FilterByUGMemberIds(ugmemberIds)
                .FilterByCustomerIds(customerIds)
                .FilterByUserGroupIds(userGroupIds)
                .FilterByUserGroupArchetypeIds(userGroupArchetypeIds)
                .FilterByUserGroupTypes(userGroupTypeIds)
                .FilterByUserGroupExtIds(userGroupExtIds)
                .FilterByUserGroupReferrerArchetypes(userGroupReferrerArchetypes)
                .FilterByUserGroupReferrerCxTokens(userGroupReferrerCxTokens)
                .FilterByUserGroupReferrerResources(userGroupReferrerResources)
                .FilterByUserGroupReferrerTokens(userGroupReferrerTokens)
                .FilterByUserGroupStatus(userGroupStatus)
                .FilterByUserExtIds(userExtIds)
                .FilterByUserIds(userIds).FilterByReferrerArchetypes(referrerArchetypes)
                .FilterByReferrerResources(referrerResources)
                .FilterByUserArchetypes(userArchetypes)
                .FilterByReferrerTokens(referrerTokens)
                .FilterByUGMemberStatus(ugMemberStatuses)
                .FilterByAges(memberAgeRanges)
                .FilterByGenders(memberGenders)
                .FilterByDateTime(validFromAfter: validFromAfter,
                    validFromBefore: validFromBefore,
                    validToAfter: validToAfter,
                    validToBefore: validToBefore)
                .FilterByUserSearchKey(userSearchKey)
                .Build();

            if (nonuserMemberOnly)
                query = query.Where(q => q.UserId == null);

            return query.Count();
        }
    }
}
