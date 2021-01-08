using cxOrganization.Client.UserGroups;
using cxOrganization.Domain.Entities;
using cxOrganization.Domain.Extensions;
using cxPlatform.Client.ConexusBase;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;

namespace cxOrganization.Domain.Repositories.QueryBuilders
{
    public class UserGroupQueryBuilder
    {
        IQueryable<UserGroupEntity> _query;
        public UserGroupQueryBuilder(IQueryable<UserGroupEntity> query)
        {
            _query = query;
        }
        public IQueryable<UserGroupEntity> Build()
        {
            return _query;

        }
        public static UserGroupQueryBuilder InitQueryBuilder(IQueryable<UserGroupEntity> query)
        {
            return new UserGroupQueryBuilder(query);
        }
        public UserGroupQueryBuilder FilterByReferrerResources(List<string> referrerResources)
        {
            if (referrerResources != null && referrerResources.Any())
            {
                if (referrerResources.Count == 1)
                {
                    var referrerResource = referrerResources.First();
                    _query = _query.Where(p => p.ReferrerResource == referrerResource);
                }
                else
                {
                    _query = _query.Where(p => referrerResources.Contains(p.ReferrerResource));
                }
            }
            return this;
        }
        public UserGroupQueryBuilder FilterByReferrerTokens(List<string> referrerTokens)
        {
            if (referrerTokens != null && referrerTokens.Any())
            {
                if (referrerTokens.Count == 1)
                {
                    var referrerToken = referrerTokens.First();
                    _query = _query.Where(p => p.ReferrerToken == referrerToken);
                }
                else
                {
                    _query = _query.Where(p => referrerTokens.Contains(p.ReferrerToken));
                }
            }
            return this;
        }
        public UserGroupQueryBuilder FilterByReferrerArchetypes(List<ArchetypeEnum> referrerArchetypes)
        {
            if (referrerArchetypes != null && referrerArchetypes.Any())
            {
                if (referrerArchetypes.Count == 1)
                {
                    var referrerArchetype = (int)referrerArchetypes.First();
                    _query = _query.Where(p => p.ReferrerArchetypeId == referrerArchetype);
                }
                else
                {
                    _query = _query.Where(p => referrerArchetypes.Contains((ArchetypeEnum)p.ReferrerArchetypeId));
                }
            }
            return this;
        }
        public UserGroupQueryBuilder FilterByUserGroupExtIds(List<string> userGroupExtIds)
        {
            if (userGroupExtIds != null && userGroupExtIds.Any())
            {
                if (userGroupExtIds.Count == 1)
                {
                    var userGroupExtId = userGroupExtIds.First();
                    _query = _query.Where(p => p.ExtId == userGroupExtId);
                }
                else
                {
                    _query = _query.Where(p => userGroupExtIds.Contains(p.ExtId));
                }
            }
            return this;
        }
        public UserGroupQueryBuilder FilterByUserGroupIds(List<int> userGroupIds)
        {
            if (userGroupIds != null && userGroupIds.Any())
            {
                if (userGroupIds.Count == 1)
                {
                    var userGroupId = userGroupIds.First();
                    _query = _query.Where(p => userGroupId == p.UserGroupId);
                }
                else
                {
                    _query = _query.Where(p => userGroupIds.Contains(p.UserGroupId));
                }
            }
            return this;
        }

        public UserGroupQueryBuilder FilterByParentDepartmentIds(List<int> departmentIds)
        {
            if (departmentIds.IsNotNullOrEmpty())
            {
                departmentIds = departmentIds.Distinct().ToList();
                if (departmentIds.Count == 1)
                {
                    var departmentId = departmentIds.First();
                    _query = _query.Where(p => departmentId == p.DepartmentId);
                }
                else
                {
                    _query = _query.Where(p => departmentIds.Contains(p.DepartmentId.Value));
                }
            }
            return this;
        }
        public UserGroupQueryBuilder FilterByUserGroupStatus(List<EntityStatusEnum> userGroupStatus)
        {
            if(!userGroupStatus.IsNotNullOrEmpty())
            {
                _query = _query.Where(p => p.EntityStatusId == (int)EntityStatusEnum.Active);
            }
            if (userGroupStatus != null && userGroupStatus.Any() && !userGroupStatus.Contains(EntityStatusEnum.All))
            {
                if (userGroupStatus.Count == 1)
                {
                    var userGroupStatusId = userGroupStatus.First();
                    _query = _query.Where(p => (short)userGroupStatusId == p.EntityStatusId);
                }
                else
                {
                    _query = _query.Where(p => userGroupStatus.Contains((EntityStatusEnum)p.EntityStatusId));
                }
            }
            return this;
        }
        public UserGroupQueryBuilder FilterByGroupArchetypeIds(List<ArchetypeEnum> userGroupArchetypeIds)
        {
            if (userGroupArchetypeIds != null && userGroupArchetypeIds.Any())
            {
                if (userGroupArchetypeIds.Count == 1)
                {
                    var userGroupArchetypeId = userGroupArchetypeIds.First();
                    _query = _query.Where(p => userGroupArchetypeId == (ArchetypeEnum)p.ArchetypeId.Value);
                }
                else
                {
                    _query = _query.Where(p => userGroupArchetypeIds.Contains((ArchetypeEnum)p.ArchetypeId.Value));
                }
            }
            return this;
        }
        public UserGroupQueryBuilder FilterByGroupArchetypeIds(List<int> userGroupArchetypeIds)
        {
            if (userGroupArchetypeIds != null && userGroupArchetypeIds.Any())
            {
                if (userGroupArchetypeIds.Count == 1)
                {
                    var userGroupArchetypeId = userGroupArchetypeIds.First();
                    _query = _query.Where(p => userGroupArchetypeId == p.ArchetypeId.Value);
                }
                else
                {
                    _query = _query.Where(p => userGroupArchetypeIds.Contains(p.ArchetypeId.Value));
                }
            }
            return this;
        }
        public UserGroupQueryBuilder FilterByCustomerIds(List<int> customerIds)
        {
            if (customerIds != null && customerIds.Any())
            {
                if (customerIds.Count == 1)
                {
                    var customerId = customerIds.First();
                    _query = _query.Where(p => (p.Department == null || customerId == p.Department.CustomerId.Value)
                    && (p.User == null || customerId == p.User.CustomerId.Value));
                }
                else
                {
                    _query = _query.Where(p => (p.Department == null || customerIds.Contains(p.Department.CustomerId.Value)) 
                    && (p.User == null || customerIds.Contains(p.User.CustomerId.Value)));
                }
            }
            return this;
        }
        public UserGroupQueryBuilder FilterByUserGroupTypes(List<GrouptypeEnum> userGroupTypes)
        {
            if (userGroupTypes != null && userGroupTypes.Any())
            {
                if (userGroupTypes.Count == 1)
                {
                    var typeId = (int)userGroupTypes.First();
                    _query = _query.Where(p => p.UserGroupTypeId == typeId);
                }
                else
                {
                    _query = _query.Where(p => userGroupTypes.Contains((GrouptypeEnum)p.UserGroupTypeId.Value));
                }
            }
            return this;
        }
        public UserGroupQueryBuilder FilterByOwnerId(int ownerId)
        {
            if(ownerId > 0)
            {
                _query = _query.Where(t => t.OwnerId == ownerId);
            }
            return this;
        }
        public UserGroupQueryBuilder FilterByCxTokens(List<string> referercxTokens)
        {
            if (referercxTokens != null && referercxTokens.Any())
            {
                if (referercxTokens.Count == 1)
                {
                    var refererCxToken = string.Format("cxtoken={0}", referercxTokens.First());
                    _query = _query.Where(p => p.ExtId.StartsWith(refererCxToken));
                }
                else
                {
                    List<string> referercxTokensTemp = new List<string>();
                    foreach (var item in referercxTokens)
                    {
                        referercxTokensTemp.Add(string.Format("cxtoken={0}", item));
                    }
                    _query = _query.Where(p => referercxTokensTemp.Any(ug => p.ExtId.StartsWith(ug)));
                }
            }
            return this;
        }
        public UserGroupQueryBuilder FilterByParentUserIds(List<int> parentUserIds)
        {
            if (parentUserIds.IsNotNullOrEmpty())
            {
                if (parentUserIds.Count == 1)
                {
                    var userId = parentUserIds.First();
                    _query = _query.Where(p => userId == p.UserId);
                }
                else
                {
                    _query = _query.Where(p => parentUserIds.Contains(p.UserId.Value));
                }
            }
            return this;
        }
        public UserGroupQueryBuilder FilterByMemberUserIds(List<int> memberUserIds)
        {
            if (memberUserIds.IsNotNullOrEmpty())
            {
                if (memberUserIds.Count == 1)
                {
                    var userId = memberUserIds.First();
                    _query = _query.Where(p => p.UGMembers.Any(t => t.UserId.HasValue && userId == t.UserId.Value));
                }
                else
                {
                    _query = _query.Where(p => p.UGMembers.Any(t => t.UserId.HasValue && memberUserIds.Contains(t.UserId.Value)));
                }
            }
            return this;
        }

        public UserGroupQueryBuilder FilterByMemberUserIds(List<int> memberUserIds, List<EntityStatusEnum> memberStatuses)
        {
            if (memberUserIds.IsNullOrEmpty()) return this;

            if (memberStatuses == null) memberStatuses = new List<EntityStatusEnum>();
            if (memberStatuses.Count == 0) memberStatuses.Add(EntityStatusEnum.Active);

            if (memberStatuses.Contains(EntityStatusEnum.All)) //There is no filter on member entity status
            {

                if (memberUserIds.Count == 1)
                {
                    var userId = memberUserIds.First();
                    _query = _query.Where(p => p.UGMembers.Any(t => userId == t.UserId));
                }
                else
                {
                    _query = _query.Where(p =>
                        p.UGMembers.Any(t => t.UserId.HasValue && memberUserIds.Contains(t.UserId.Value)));
                }

                return this;
            }

            //Filter on both entity status and user id


            if (memberUserIds.Count == 1 && memberStatuses.Count == 1)
            {
                var userId = memberUserIds.First();
                var entityStatusId = (int) memberStatuses.First();
                _query = _query.Where(p =>
                    p.UGMembers.Any(t => t.EntityStatusId == entityStatusId && userId == t.UserId));

            }
            else if (memberUserIds.Count == 1)
            {
                var userId = memberUserIds.First();
                var entityStatusIds = memberStatuses.Select(m => (int?) m).ToList();
                _query = _query.Where(p =>
                    p.UGMembers.Any(t => t.UserId == userId && entityStatusIds.Contains(t.EntityStatusId)));
            }
            else if (memberStatuses.Count == 1)
            {
                var entityStatusId = (int) memberStatuses.First();
                _query = _query.Where(p => p.UGMembers.Any(t =>
                    t.EntityStatusId == entityStatusId && t.UserId.HasValue && memberUserIds.Contains(t.UserId.Value)));
            }
            else
            {
                var entityStatusIds = memberStatuses.Select(m => (int?) m).ToList();
                _query = _query.Where(p => p.UGMembers.Any(t =>
                    entityStatusIds.Contains(t.EntityStatusId) && t.UserId.HasValue &&
                    memberUserIds.Contains(t.UserId.Value)));
            }


            return this;
        }

        public UserGroupQueryBuilder FilterByDatetime(DateTime? lastUpdatedBefore = null, DateTime? lastUpdatedAfter = null)
        {
            if (lastUpdatedBefore.HasValue)
            {
                _query = _query.Where(p => p.LastUpdated <= lastUpdatedBefore);
            }
            if (lastUpdatedAfter.HasValue)
            {
                _query = _query.Where(p => p.LastUpdated >= lastUpdatedAfter);
            }
            return this;
        }

        public UserGroupQueryBuilder FilterByParentDepartmentArchetypes(List<ArchetypeEnum> parentDepartmentArchetypes)
        {
            if (parentDepartmentArchetypes != null && parentDepartmentArchetypes.Any())
            {
                if (parentDepartmentArchetypes.Count == 1)
                {
                    var parentDepartmentArchetype = (int)parentDepartmentArchetypes.First();
                    _query = _query.Where(p => p.Department.ArchetypeId == parentDepartmentArchetype);
                }
                else
                {
                    _query = _query.Where(p => parentDepartmentArchetypes.Contains((ArchetypeEnum)p.Department.ArchetypeId));
                }
            }
            return this;
        }
        public UserGroupQueryBuilder FilterByParentUserArchetypes(List<ArchetypeEnum> parentUserArchetypes)
        {
            if (parentUserArchetypes != null && parentUserArchetypes.Any())
            {
                if (parentUserArchetypes.Count == 1)
                {
                    var parentUserArchetype = (int)parentUserArchetypes.First();
                    _query = _query.Where(p => p.User.ArchetypeId == parentUserArchetype);
                }
                else
                {
                    _query = _query.Where(p => parentUserArchetypes.Contains((ArchetypeEnum)p.User.ArchetypeId));
                }
            }
            return this;
        }

        public UserGroupQueryBuilder FilterBySearchKey(string searchKey)
        {
            if (!string.IsNullOrEmpty(searchKey))
            {
                searchKey = searchKey.Trim();
                _query = _query.Where(x => x.User.FirstName.Contains(searchKey)
                                || x.User.LastName.Contains(searchKey)
                                || x.User.Email.Contains(searchKey)
                                || x.User.UserName.Contains(searchKey)
                                || x.User.Department.Name.Contains(searchKey));
            }
            return this;
        }

        public UserGroupQueryBuilder FilterByUserStatus(List<EntityStatusEnum> userStatus)
        {
            if (userStatus.IsNotNullOrEmpty())
            {
                if (userStatus.Contains(EntityStatusEnum.All))
                    return this;
                else
                    _query = _query.Where(p => userStatus.Contains((EntityStatusEnum)p.User.EntityStatusId));
            }
            else
            {
                _query = _query.Where(p => p.User.EntityStatusId == (int)EntityStatusEnum.Active);
            }
            return this;
        }
    }
}
