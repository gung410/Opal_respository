using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Amazon.Util.Internal;
using cxOrganization.Client.UserGroups;
using cxOrganization.Domain.AdvancedWorkContext;
using cxOrganization.Domain.Entities;
using cxOrganization.Domain.Repositories.QueryBuilders;
using cxPlatform.Client.ConexusBase;
using cxPlatform.Core;
using cxPlatform.Core.Extentions;
using Microsoft.EntityFrameworkCore;

namespace cxOrganization.Domain.Repositories
{
    /// <summary>
    /// Class UserGroupRepository
    /// </summary>
    public class UserGroupRepository : OwnerEntityBaseRepository<UserGroupEntity>, IUserGroupRepository
    {
        const int MaximumRecordsReturn = 5000;

        public UserGroupRepository(OrganizationDbContext dbContext,
            IAdvancedWorkContext workContext)
            : base(dbContext, workContext)
        {
        }

        public PaginatedList<UserGroupEntity> GetUserGroups(int ownerId = 0,
            List<int> customerIds = null,
            List<int> userGroupIds = null,
            List<int> memberUserIds = null,
            List<int> parentDepartmentIds = null,
            List<EntityStatusEnum> statusIds = null,
            List<EntityStatusEnum> userStatusIds = null,
            List<GrouptypeEnum> groupTypes = null,
            List<int> archetypeIds = null,
            List<string> extIds = null,
            DateTime? lastUpdatedBefore = null,
            DateTime? lastUpdatedAfter = null,
            List<int> parentUserIds = null,
            int pageIndex = 0,
            int pageSize = 0,
            string orderBy = "",
            List<string> referrerTokens = null,
            List<string> referrerResources = null,
            List<ArchetypeEnum> referrerArchetypes = null,
            List<ArchetypeEnum> parentDepartmentArchetypes = null,
            List<ArchetypeEnum> parentUserArchetypes = null,
            string searchKey = null,
            bool includeDepartment = false,
            bool includeUser = false)
        {

            var query = UserGroupQueryBuilder.InitQueryBuilder(GetAllAsNoTracking())
                     .FilterBySearchKey(searchKey)
                     .FilterByGroupArchetypeIds(archetypeIds)
                     .FilterByReferrerResources(referrerResources)
                     .FilterByReferrerTokens(referrerTokens)
                     .FilterByUserGroupExtIds(extIds)
                     .FilterByUserGroupIds(userGroupIds)
                     .FilterByUserGroupStatus(statusIds)
                     .FilterByUserGroupTypes(groupTypes)
                     .FilterByReferrerArchetypes(referrerArchetypes)
                     .FilterByCustomerIds(customerIds)
                     .FilterByParentDepartmentIds(parentDepartmentIds)
                     .FilterByOwnerId(ownerId)
                     .FilterByParentUserIds(parentUserIds)
                     .FilterByMemberUserIds(memberUserIds)
                     .FilterByDatetime(lastUpdatedBefore, lastUpdatedAfter)
                     .FilterByParentDepartmentArchetypes(parentDepartmentArchetypes)
                     .FilterByParentUserArchetypes(parentUserArchetypes)
                     .FilterByUserStatus(userStatusIds)
                     .Build();

            if (includeDepartment)
            {
                query = query.Include(q => q.Department);
            }

            if (includeUser)
            {
                query = query.Include(q => q.User);
            }
            //Query must be ordered before apply paging
            query = query.ApplyOrderBy(p => p.UserGroupId, orderBy);

            var hasMoreData = false;
            //Build paging from IQueryable
            var totalItem = 0;
            var items = query.ToPaging(pageIndex, pageSize, out hasMoreData, out totalItem);
            return new PaginatedList<UserGroupEntity>(items, pageIndex, pageSize, hasMoreData)
            {
                TotalItems = totalItem,

            };
        }
        public async Task<PaginatedList<UserGroupEntity>> GetUserGroupsAsync(int ownerId = 0,
         List<int> customerIds = null,
         List<int> userGroupIds = null,
         List<int> memberUserIds = null,
         List<int> parentDepartmentIds = null,
         List<EntityStatusEnum> statusIds = null,
         List<EntityStatusEnum> userStatusIds = null,
         List<GrouptypeEnum> groupTypes = null,
         List<int> archetypeIds = null,
         List<string> extIds = null,
         DateTime? lastUpdatedBefore = null,
         DateTime? lastUpdatedAfter = null,
         List<int> parentUserIds = null,
         int pageIndex = 0,
         int pageSize = 0,
         string orderBy = "",
         int? assigneeDepartmentId = null,
         List<string> referrerTokens = null,
         List<string> referrerResources = null,
         List<ArchetypeEnum> referrerArchetypes = null,
         List<ArchetypeEnum> parentDepartmentArchetypes = null,
         List<ArchetypeEnum> parentUserArchetypes = null,
         string searchKey = null,
         bool includeDepartment = false,
         bool includeUser = false)
        {

            var query = UserGroupQueryBuilder.InitQueryBuilder(GetAllAsNoTracking())
                     .FilterBySearchKey(searchKey)
                     .FilterByGroupArchetypeIds(archetypeIds)
                     .FilterByReferrerResources(referrerResources)
                     .FilterByReferrerTokens(referrerTokens)
                     .FilterByUserGroupExtIds(extIds)
                     .FilterByUserGroupIds(userGroupIds)
                     .FilterByUserGroupStatus(statusIds)
                     .FilterByUserGroupTypes(groupTypes)
                     .FilterByReferrerArchetypes(referrerArchetypes)
                     .FilterByCustomerIds(customerIds)
                     .FilterByParentDepartmentIds(parentDepartmentIds)
                     .FilterByOwnerId(ownerId)
                     .FilterByParentUserIds(parentUserIds)
                     .FilterByMemberUserIds(memberUserIds)
                     .FilterByDatetime(lastUpdatedBefore, lastUpdatedAfter)
                     .FilterByParentDepartmentArchetypes(parentDepartmentArchetypes)
                     .FilterByParentUserArchetypes(parentUserArchetypes)
                     .FilterByUserStatus(userStatusIds)
                     .Build();

            if (includeDepartment)
            {
                query = query.Include(q => q.Department);
            }

            if (includeUser)
            {
                query = query.Include(q => q.User);
            }

            if (assigneeDepartmentId is object)
            {
                //Query must be ordered before apply paging
                query = query
                    .OrderByDescending(userGroup => userGroup.Department.DepartmentId == assigneeDepartmentId)
                    .ThenBy(userGroup => userGroup.Department.Name)
                    .ThenBy(userGroup => userGroup.User.FirstName);
            }



            var hasMoreData = false;
            //Build paging from IQueryable
            var totalItem = 0;
            var paginatedEntities = await query.ToPagingAsync(pageIndex, pageSize);
            return new PaginatedList<UserGroupEntity>(paginatedEntities.Items, pageIndex, pageSize,
                paginatedEntities.HasMoreData)
            {
                TotalItems = paginatedEntities.TotalItems
            };
        }
        public UserGroupEntity GetUserGroupByExtId(string extId, int? departmentId = null, int? customerId = null)
        {
            var query = GetAll().Where(p => p.ExtId == extId && p.EntityStatusId == (short)EntityStatusEnum.Active);
            if (departmentId.HasValue)
            {
                query = query.Where(p => p.DepartmentId == departmentId.Value);
            }
            if (customerId.HasValue)
            {
                query = query.Where(p => (p.Department == null || customerId == p.Department.CustomerId.Value)
                  && (p.User == null || customerId == p.User.CustomerId.Value));
            }
            return query.FirstOrDefault();
        }

        public List<UserGroupEntity> GetUserGroupByIds(List<int> userGroupIds,
            List<int> allowArchetypeIds,
            Expression<Func<UserGroupEntity, object>>[] includeProperties,
            params EntityStatusEnum[] filters)
        {
            return GetAllIncluding(includeProperties, filters)//.Include(x => x.Department.H_D)
                    .Where(x => userGroupIds.Contains(x.UserGroupId) && x.ArchetypeId.HasValue && allowArchetypeIds.Contains(x.ArchetypeId.Value))
                    .ToList();
        }
        public List<UserGroupEntity> GetUserGroupByIds(List<int> userGroupIds,
            List<int> allowArchetypeIds,
            bool includeDepartmenttype)
        {
            var quey = GetAll();
            if (includeDepartmenttype)
                quey = quey.Include(j => j.DT_UGs)
                           .ThenInclude(t => t.DepartmentType);
            return quey.Where(x => userGroupIds.Contains(x.UserGroupId) && x.ArchetypeId.HasValue && allowArchetypeIds.Contains(x.ArchetypeId.Value))
                       .ToList();
        }

        public List<UserGroupEntity> GetUserGroupByDepartmentIds(List<int> departmentIds, int? groupType = null, params EntityStatusEnum[] filters)
        {
            if (groupType.HasValue)
            {
                return GetAll(filters).Include(x => x.Department)
                    .Where(x => x.DepartmentId.HasValue && departmentIds.Contains(x.DepartmentId.Value) && x.UserGroupTypeId == groupType)
                    .ToList();
            }
            else
            {
                return GetAll(filters).Include(x => x.Department)
                    .Where(x => x.DepartmentId.HasValue && departmentIds.Contains(x.DepartmentId.Value))
                    .ToList();
            }
        }

        public List<UserGroupEntity> GetUserGroupsByDepartmentId(int departmentId, bool includeDeppartmentType = false, params EntityStatusEnum[] filters)
        {
            var query = GetAll(filters).Where(ug => ug.DepartmentId == departmentId);

            return query.ToList();
        }

        public List<UserGroupEntity> GetuserGroupsByDepartmentIdAndUserGroupType(int departmentId,
            int userGroupTypeId,
            bool isIncludeUsers = false,
            int departmentTypeId = 0,
            params EntityStatusEnum[] filters)
        {
            var query = GetAllAsNoTracking(filters).Where(t => t.DepartmentId == departmentId &&
                                                         (userGroupTypeId == 0 || t.UserGroupTypeId == userGroupTypeId));


            if (isIncludeUsers)
                query = query.Include(j => j.UGMembers)
                             .ThenInclude(t => t.User)
                             .ThenInclude(k => k.UT_Us)
                             .ThenInclude(l => l.UserType);
            return query.ToList();
        }

        public UserGroupEntity GetUserGroupIncludeDepartmentType(int userGroupId, params EntityStatusEnum[] filters)
        {
            return GetAll(filters).Include(x => x.DT_UGs)
                .ThenInclude(j => j.DepartmentType)
                .FirstOrDefault(x => x.UserGroupId == userGroupId);
        }

        public List<UserGroupEntity> GetUserGroups(int? userGroupId, string extId, params EntityStatusEnum[] filters)
        {
            var filterById = userGroupId.HasValue && userGroupId > 0;
            var filterByExtId = !string.IsNullOrEmpty(extId);
            if (!(filterById || filterByExtId))
            {
                return new List<UserGroupEntity>();
            }
            return GetAll(filters)
                .Include(x => x.Department)
                .Include(x => x.User)
                .Where(userGroupEntity => (userGroupEntity.UserGroupId == userGroupId || (filterByExtId && userGroupEntity.ExtId == extId))).ToList();
        }

        public List<UserGroupEntity> GetUserGroupByIdsIncludeProperties(List<int> userGroupIds, Expression<Func<UserGroupEntity, object>>[] includeProperties, params EntityStatusEnum[] filters)
        {
            return GetAllIncluding(includeProperties, filters)
                    .Where(x => userGroupIds.Contains(x.UserGroupId) && x.ArchetypeId.HasValue)
                    .ToList();
        }

        public UserGroupEntity GetUserGroupById(int Id, bool includeUsers = true, bool includeUserType = false, params EntityStatusEnum[] filters)
        {
            var query = GetAll(filters).Where(x => x.UserGroupId == Id);
            if (includeUsers)
                query = query.Include(x => x.UGMembers).ThenInclude(t => t.User);
            if (includeUserType)
                query = query.Include(x => x.UGMembers)
                             .ThenInclude(j => j.User)
                             .ThenInclude(j => j.UT_Us)
                             .ThenInclude(j => j.UserType);

            var userGroup = query.FirstOrDefault();
            return userGroup;
        }

        //public bool UpdateUG_U(string userGroupIds, string userIds, List<int> userGroupIdsToRemove)
        //{
        //    var result = 1;

        //    var aParams = new List<SqlParameter>
        //        {
        //            new SqlParameter("@Result", SqlDbType.Int),
        //            new SqlParameter("@UserGroupIds", userGroupIds),
        //            new SqlParameter("@UserIds", userIds),
        //            new SqlParameter("@RemoveFromGroupIDList", string.Join(",", userGroupIdsToRemove))
        //        };

        //    try
        //    {
        //        aParams[0].Direction = ParameterDirection.Output;
        //        ExecuteStoredProcedure("[org].[prc_AddTo_UserGroup] @Result output, @UserGroupIds , @UserIds, @RemoveFromGroupIDList", aParams.ToArray());
        //        result = (int)aParams[0].Value;
        //    }
        //    catch (Exception)
        //    {
        //        result = -1;
        //    }

        //    return result == 0;
        //}

        public List<UserGroupEntity> GetUserGroupsWithoutPaging(int ownerId = 0,
            List<int> customerIds = null,
            List<ArchetypeEnum> userGroupArchetypeIds = null,
            List<int> userGroupIds = null,
            List<int> parentDepartmentIds = null,
            List<string> userGroupExtIds = null,
            List<string> referrerTokens = null,
            List<string> referrerResources = null,
            List<ArchetypeEnum> referrerArchetypes = null,
            List<GrouptypeEnum> userGroupTypeIds = null,
            List<EntityStatusEnum> userGroupStatuses = null,
            List<string> referercxTokens = null,
            bool includeMemberUsers = false,
            List<int> parentUserIds = null,
            DateTime? lastUpdatedBefore = null,
            DateTime? lastUpdatedAfter = null,
            List<ArchetypeEnum> parentDepartmentArchetypes = null,
            List<ArchetypeEnum> parentUserArchetypes = null,
            List<int> memberUserIds = null,
            List<EntityStatusEnum> memberStatuses = null,
            bool includeUser = true,
            bool includeDepartment = true)
        {
            var query = UserGroupQueryBuilder.InitQueryBuilder(GetAllAsNoTracking())
                .FilterByGroupArchetypeIds(userGroupArchetypeIds)
                .FilterByReferrerResources(referrerResources)
                .FilterByReferrerTokens(referrerTokens)
                .FilterByUserGroupExtIds(userGroupExtIds)
                .FilterByUserGroupIds(userGroupIds)
                .FilterByUserGroupStatus(userGroupStatuses)
                .FilterByUserGroupTypes(userGroupTypeIds)
                .FilterByReferrerArchetypes(referrerArchetypes)
                .FilterByCxTokens(referercxTokens)
                .FilterByCustomerIds(customerIds)
                .FilterByParentDepartmentIds(parentDepartmentIds)
                .FilterByOwnerId(ownerId)
                .FilterByParentUserIds(parentUserIds)
                .FilterByDatetime(lastUpdatedBefore, lastUpdatedAfter)
                .FilterByMemberUserIds(memberUserIds, memberStatuses)
                .FilterByParentDepartmentArchetypes(parentDepartmentArchetypes)
                .FilterByParentUserArchetypes(parentUserArchetypes)
                .Build();

            if (includeDepartment)
                query = query.Include(x => x.Department);

            if (includeUser)
                query = query.Include(x => x.User);

            if (includeMemberUsers)
                query = query.Include(x => x.UGMembers.Select(t => t.User));

            return query.ToList();
        }

        public List<UserGroupEntity> GetUserGroupsByArchetypeIds(List<ArchetypeEnum> archetypeIds)
        {
            var query = GetAllAsNoTracking(null).Where(t => archetypeIds.Contains((ArchetypeEnum)t.ArchetypeId));

            return query.ToList();
        }
        public List<string> GetModifiedProperties(UserGroupEntity entity)
        {
            var modifiedProperties = new List<string>();
            var properties = _dbContext.Entry(entity).Properties;
            foreach (var propertyEntry in properties)
            {
                if (propertyEntry.IsModified) modifiedProperties.Add(propertyEntry.Metadata.Name);
            }

            return modifiedProperties;
        }
    }
}
