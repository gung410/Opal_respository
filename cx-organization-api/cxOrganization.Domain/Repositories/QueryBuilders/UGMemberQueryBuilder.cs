using System;
using System.Collections.Generic;
using System.Linq;
using cxOrganization.Client.UserGroups;
using cxOrganization.Domain.Entities;
using cxOrganization.Domain.Enums;
using cxOrganization.Domain.Extensions;
using cxPlatform.Client.ConexusBase;
using LinqKit;
using Microsoft.EntityFrameworkCore;

namespace cxOrganization.Domain.Repositories.QueryBuilders
{
    public class UGMemberQueryBuilder
    {
        private IQueryable<UGMemberEntity> _query;
        public UGMemberQueryBuilder(IQueryable<UGMemberEntity> query)
        {
            _query = query;
        }
        public IQueryable<UGMemberEntity> Build()
        {
            return _query;
        }
        public static UGMemberQueryBuilder InitQueryBuilder(IQueryable<UGMemberEntity> query)
        {
            return new UGMemberQueryBuilder(query);
        }
        public UGMemberQueryBuilder FilterByUserGroupTypes(List<GrouptypeEnum> userGroupTypes)
        {
            if (userGroupTypes != null && userGroupTypes.Any())
            {
                if (userGroupTypes.Count == 1)
                {
                    var typeId = (int)userGroupTypes.First();
                    _query = _query.Where(p => p.UserGroup.UserGroupTypeId == typeId);
                }
                else
                {
                    _query = _query.Where(p => userGroupTypes.Contains((GrouptypeEnum)p.UserGroup.UserGroupTypeId.Value));
                }
            }
            return this;
        }

        public UGMemberQueryBuilder FilterByUGMemberIds(List<long> ugMemberIds)
        {
            if (ugMemberIds != null && ugMemberIds.Any())
            {
                if (ugMemberIds.Count == 1)
                {
                    var uguId = ugMemberIds[0];
                    _query = _query.Where(t => t.UGMemberId == uguId);
                }
                else
                {
                    _query = _query.Where(t => ugMemberIds.Contains(t.UGMemberId));
                }

            }
            return this;
        }
        public UGMemberQueryBuilder FilterByUGMemberExtIds(List<string> ugMemberExtIds)
        {
            if (ugMemberExtIds != null && ugMemberExtIds.Any())
            {
                if (ugMemberExtIds.Count == 1)
                {
                    var extId = ugMemberExtIds[0];
                    _query = _query.Where(t => t.ExtId == extId);
                }
                else
                {
                    _query = _query.Where(t => ugMemberExtIds.Contains(t.ExtId));
                }

            }
            return this;
        }
        public UGMemberQueryBuilder FilterByUserIds(List<int> userIds)
        {
            if (userIds != null && userIds.Any())
            {
                if (userIds.Count == 1)
                {
                    var userId = userIds[0];
                    _query = _query.Where(t => t.UserId == userId);
                }
                else
                {
                    _query = _query.Where(t => userIds.Contains(t.UserId.Value));
                }

            }
            return this;
        }
        public UGMemberQueryBuilder FilterByUserGroupIds(List<int> userGroupIds)
        {
            if (userGroupIds != null && userGroupIds.Any())
            {
                if (userGroupIds.Count == 1)
                {
                    var userGroupId = userGroupIds[0];
                    _query = _query.Where(t => t.UserGroupId == userGroupId);
                }
                else
                {
                    _query = _query.Where(t => userGroupIds.Contains(t.UserGroupId));
                }

            }
            return this;
        }
        public UGMemberQueryBuilder FilterByUGMemberStatus(List<EntityStatusEnum> ugMemberStatus)
        {
            if (!ugMemberStatus.IsNotNullOrEmpty())
            {
                _query = _query.Where(p => p.EntityStatusId.Value == (int)EntityStatusEnum.Active);
            }
            else if (!ugMemberStatus.Contains(EntityStatusEnum.All))
            {
                if (ugMemberStatus.Count == 1)
                {
                    var firstValue = (int)ugMemberStatus[0];
                    _query = _query.Where(p => p.EntityStatusId == firstValue);
                }
                else
                {
                    _query = _query.Where(p => ugMemberStatus.Contains((EntityStatusEnum)p.EntityStatusId.Value));
                }
            }
            return this;
        }
        public UGMemberQueryBuilder FilterByUserExtIds(List<string> userExtIds)
        {
            if (userExtIds != null && userExtIds.Any())
            {
                if (userExtIds.Count == 1)
                {
                    var extId = userExtIds[0];
                    _query = _query.Where(t => t.User.ExtId == extId);
                }
                else
                {
                    _query = _query.Where(t => userExtIds.Contains(t.User.ExtId));
                }

            }
            return this;
        }
        public UGMemberQueryBuilder FilterByUserArchetypes(List<ArchetypeEnum> userArchetypes)
        {
            if (userArchetypes != null && userArchetypes.Any())
            {
                if (userArchetypes.Count == 1)
                {
                    var archetype = (int)userArchetypes[0];
                    _query = _query.Where(t => t.User.ArchetypeId == archetype);
                }
                else
                {
                    _query = _query.Where(t => userArchetypes.Contains((ArchetypeEnum)t.User.ArchetypeId));
                }
            }
            return this;
        }
        public UGMemberQueryBuilder FilterByReferrerTokens(List<string> referrerTokens)
        {
            if (referrerTokens != null && referrerTokens.Any())
            {
                if (referrerTokens.Count == 1)
                {
                    var token = referrerTokens[0];
                    _query = _query.Where(t => t.ReferrerToken == token);
                }
                else
                {
                    _query = _query.Where(t => referrerTokens.Contains(t.ReferrerToken));
                }
            }
            return this;
        }
        public UGMemberQueryBuilder FilterByDateTime(DateTime? validFromBefore = null,
            DateTime? validFromAfter = null,
            DateTime? validToBefore = null,
            DateTime? validToAfter = null,
            DateTime? createdBefore = null,
            DateTime? createdAfter = null,
            DateTime? lastUpdatedBefore = null,
            DateTime? lastUpdatedAfter = null)
        {
            if (validFromBefore.HasValue)
                _query = _query.Where(t => t.validFrom == null || t.validFrom <= validFromBefore );
            if (validToBefore.HasValue)
                _query = _query.Where(t => t.ValidTo == null || t.ValidTo <= validToBefore );
            if (validFromAfter.HasValue)
                _query = _query.Where(t => t.validFrom == null || t.validFrom >= validFromAfter);
            if (validToAfter.HasValue)
                _query = _query.Where(t => t.ValidTo == null || t.ValidTo >= validToAfter);
            if (createdBefore.HasValue)
                _query = _query.Where(t => t.Created <= createdBefore);
            if (createdAfter.HasValue)
                _query = _query.Where(t => t.Created >= createdAfter);
            if (lastUpdatedBefore.HasValue)
                _query = _query.Where(t => t.LastUpdated <= lastUpdatedBefore);
            if (lastUpdatedAfter.HasValue)
                _query = _query.Where(t => t.LastUpdated >= lastUpdatedAfter);
            return this;
        }
        public UGMemberQueryBuilder FilterByReferrerResources(List<string> referrerResources)
        {
            if (referrerResources != null && referrerResources.Any())
            {
                if (referrerResources.Count == 1)
                {
                    var referrerResource = referrerResources[0];
                    _query = _query.Where(t => t.ReferrerResource == referrerResource);
                }
                else
                {
                    _query = _query.Where(t => referrerResources.Contains(t.ReferrerResource));
                }
            }
            return this;
        }
        public UGMemberQueryBuilder FilterByReferrerArchetypes(List<ArchetypeEnum> referrerArchetypes)
        {
            if (referrerArchetypes != null && referrerArchetypes.Any())
            {
                if (referrerArchetypes.Count == 1)
                {
                    var referrerArchetype = (int)referrerArchetypes[0];
                    _query = _query.Where(t => t.ReferrerArchetypeId == referrerArchetype);
                }
                else
                {
                    _query = _query.Where(t => referrerArchetypes.Contains((ArchetypeEnum)t.ReferrerArchetypeId));
                }
            }
            return this;
        }
        public UGMemberQueryBuilder FilterByUserGroupReferrerResources(List<string> userGroupReferrerResources)
        {
            if (userGroupReferrerResources != null && userGroupReferrerResources.Any())
            {
                if (userGroupReferrerResources.Count == 1)
                {
                    var referrerResource = userGroupReferrerResources.First();
                    _query = _query.Where(p => p.UserGroup.ReferrerResource == referrerResource);
                }
                else
                {
                    _query = _query.Where(p => userGroupReferrerResources.Contains(p.UserGroup.ReferrerResource));
                }
            }
            return this;
        }
        public UGMemberQueryBuilder FilterByUserGroupReferrerTokens(List<string> userGroupReferrerTokens)
        {
            if (userGroupReferrerTokens != null && userGroupReferrerTokens.Any())
            {
                if (userGroupReferrerTokens.Count == 1)
                {
                    var referrerToken = userGroupReferrerTokens.First();
                    _query = _query.Where(p => p.UserGroup.ReferrerToken == referrerToken);
                }
                else
                {
                    _query = _query.Where(p => userGroupReferrerTokens.Contains(p.UserGroup.ReferrerToken));
                }
            }
            return this;
        }
        public UGMemberQueryBuilder FilterByUserGroupReferrerArchetypes(List<ArchetypeEnum> userGroupReferrerArchetypes)
        {
            if (userGroupReferrerArchetypes != null && userGroupReferrerArchetypes.Any())
            {
                if (userGroupReferrerArchetypes.Count == 1)
                {
                    var referrerArchetype = (int)userGroupReferrerArchetypes.First();
                    _query = _query.Where(p => p.UserGroup.ReferrerArchetypeId == referrerArchetype);
                }
                else
                {
                    _query = _query.Where(p => userGroupReferrerArchetypes.Contains((ArchetypeEnum)p.UserGroup.ReferrerArchetypeId));
                }
            }
            return this;
        }
        public UGMemberQueryBuilder FilterByUserGroupExtIds(List<string> userGroupExtIds)
        {
            if (userGroupExtIds != null && userGroupExtIds.Any())
            {
                if (userGroupExtIds.Count == 1)
                {
                    var userGroupExtId = userGroupExtIds.First();
                    _query = _query.Where(p => p.UserGroup.ExtId == userGroupExtId);
                }
                else
                {
                    _query = _query.Where(p => userGroupExtIds.Contains(p.UserGroup.ExtId));
                }
            }
            return this;
        }
        public UGMemberQueryBuilder FilterByUserGroupStatus(List<EntityStatusEnum> userGroupStatus)
        {
            if(!userGroupStatus.IsNotNullOrEmpty())
            {
                _query = _query.Where(p => p.UserGroup.EntityStatusId == (int)EntityStatusEnum.Active);
            }
            if (userGroupStatus != null && userGroupStatus.Any() && !userGroupStatus.Contains(EntityStatusEnum.All))
            {
                if (userGroupStatus.Count == 1)
                {
                    var userGroupStatusId = userGroupStatus.First();
                    _query = _query.Where(p => (short)userGroupStatusId == p.UserGroup.EntityStatusId);
                }
                else
                {
                    _query = _query.Where(p => userGroupStatus.Contains((EntityStatusEnum)p.UserGroup.EntityStatusId));
                }
            }
            return this;
        }
        public UGMemberQueryBuilder FilterByUserGroupArchetypeIds(List<ArchetypeEnum> userGroupArchetypeIds)
        {
            if (userGroupArchetypeIds != null && userGroupArchetypeIds.Any())
            {
                if (userGroupArchetypeIds.Count == 1)
                {
                    var userGroupArchetypeId = userGroupArchetypeIds.First();
                    _query = _query.Where(p => userGroupArchetypeId == (ArchetypeEnum)p.UserGroup.ArchetypeId.Value);
                }
                else
                {
                    _query = _query.Where(p => userGroupArchetypeIds.Contains((ArchetypeEnum)p.UserGroup.ArchetypeId.Value));
                }
            }
            return this;
        }
        public UGMemberQueryBuilder FilterByCustomerIds(List<int> customerIds)
        {
            if (customerIds != null && customerIds.Any())
            {
                if (customerIds.Count == 1)
                {
                    var customerId = customerIds.First();
                    _query = _query.Where(p => p.CustomerId == customerId || p.CustomerId == null);
                }
                else
                {
                    _query = _query.Where(p => customerIds.Contains(p.CustomerId.Value));
                }
            }
            return this;
        }
        public UGMemberQueryBuilder FilterByOwnerId(int ownerId)
        {
            if (ownerId > 0)
            {
                _query = _query.Where(t => t.UserGroup.OwnerId == ownerId);
            }
            return this;
        }
        public UGMemberQueryBuilder FilterByUserGroupReferrerCxTokens(List<string> referercxTokens)
        {
            if (referercxTokens != null && referercxTokens.Any())
            {
                if (referercxTokens.Count == 1)
                {
                    var refererCxToken = string.Format("cxtoken={0}", referercxTokens.First());
                    _query = _query.Where(p => p.UserGroup.ExtId.StartsWith(refererCxToken));
                }
                else
                {
                    List<string> referercxTokensTemp = new List<string>();
                    foreach (var item in referercxTokens)
                    {
                        referercxTokensTemp.Add(string.Format("cxtoken={0}", item));
                    }
                    _query = _query.Where(p => referercxTokensTemp.Any(ug => p.UserGroup.ExtId.StartsWith(ug)));
                }
            }
            return this;
        }
        public UGMemberQueryBuilder FilterByGenders(List<Gender> genders)
        {
            genders = genders == null ? new List<Gender>() : genders.Distinct().ToList();
            if (genders.IsNotNullOrEmpty())
            {
                if (genders.Count == 1)
                {
                    Gender value = genders[0];
                    if (value != Gender.Unknown)
                    {
                        _query = _query.Where(t => t.User.Gender == (int)value);
                    }
                    else
                    {
                        _query = _query.Where(t => t.User.Gender == null || t.User.Gender == (int)value);
                    }

                }
                else
                {
                    if (genders.Contains(Gender.Unknown))
                    {
                        _query = _query.Where(t => t.User.Gender == null || genders.Contains((Gender)t.User.Gender.Value));
                    }
                    else
                    {
                        _query = _query.Where(t => genders.Contains((Gender)t.User.Gender.Value));
                    }
                }
            }
            return this;
        }
        public UGMemberQueryBuilder FilterByAges(List<AgeRange> ageRanges)
        {
            if (ageRanges.IsNotNullOrEmpty())
            {
                List<AgeRangePoints> agePointList = GetAgePointFromAgeRange(ageRanges);

                var searchPredicate = PredicateBuilder.New<UGMemberEntity>();
                foreach (var item in agePointList)
                {
                    var currentYear = DateTime.Now.Year;
                    var to = item.End;
                    var from = item.Start;
                    var datetimeToYear = new DateTime(currentYear - to, 1, 1);
                    var datetimeFromYear = new DateTime(currentYear - from, 12, 31);
                    searchPredicate =
                    searchPredicate.Or(x =>
                    ((x.User.DateOfBirth.Value <= datetimeFromYear) && (x.User.DateOfBirth.Value >= datetimeToYear))
                    || (from == 0 && to == 0 && x.User.DateOfBirth == null));
                }

                _query = _query.AsExpandable().Where(searchPredicate);
            }
            return this;
        }
        private List<AgeRangePoints> GetAgePointFromAgeRange(List<AgeRange> ageRanges)
        {
            const int minAge = 0;
            const int maxAge = 200;
            const int ageStep = 9;
            const int underTwentyMaxAge = 19;
            List<AgeRangePoints> agePointList = new List<AgeRangePoints>();

            foreach (var age in ageRanges.Distinct())
            {
                switch (age)
                {
                    case AgeRange.UnderTwenty:
                        agePointList.Add(new AgeRangePoints { Start = minAge, End = underTwentyMaxAge });
                        break;
                    case AgeRange.Twenties:
                    case AgeRange.Thirties:
                    case AgeRange.Forties:
                        agePointList.Add(new AgeRangePoints { Start = (int)age, End = (int)age + ageStep });
                        break;
                    case AgeRange.FiftyAndGreater:
                        agePointList.Add(new AgeRangePoints { Start = (int)age, End = maxAge });
                        break;
                }
            }
            return agePointList;
        }

        public UGMemberQueryBuilder FilterByUserSearchKey(string searchKey)
        {
            if (searchKey == null) return this;

            searchKey = searchKey.Trim();

            if (searchKey.Length == 0 ) return this;

            char space = ' ';

            var isSearchingByName = searchKey.IndexOf(space) > 0;
            if (isSearchingByName)
            {
                string term = string.Format("%{0}%", searchKey.Replace(' ', '%'));
                _query = _query.Where(x => EF.Functions.Like(term, x.User.FirstName + x.User.LastName));
            }
            else
            {
                _query = _query.Where(x => x.User.FirstName.Contains(searchKey)
                                    || x.User.LastName.Contains(searchKey)
                                    || x.User.Email.Contains(searchKey)
                                    || x.User.Mobile.Contains(searchKey)
                                    || x.User.SSN.Contains(searchKey));
            }
            return this;
        }
    }
}
