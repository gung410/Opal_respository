using cxOrganization.Domain.Entities;
using cxOrganization.Domain.Enums;
using cxOrganization.Domain.Extensions;
using cxOrganization.Domain.Repositories.QueryBuilders;
using cxOrganization.Domain.Security.User;
using cxOrganization.Domain.Settings;
using cxPlatform.Client.ConexusBase;
using cxPlatform.Core;
using cxPlatform.Core.Common;
using cxPlatform.Core.Exceptions;
using cxPlatform.Core.Extentions;
using cxPlatform.Core.Security;
using LinqKit;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using cxOrganization.Domain.DomainEnums;
using cxOrganization.Domain.Services.Reports;
using Microsoft.Data.SqlClient;
using Gender = cxOrganization.Domain.Enums.Gender;

namespace cxOrganization.Domain.Repositories
{
    /// <summary>
    /// Class UserRepository
    /// </summary>
    public class UserRepository : EntityBaseRepository<UserEntity>, IUserRepository
    {
        const int MaximumRecordsReturn = 100000;
        private readonly IUserTypeRepository _userTypeRepository;
        private readonly IHierarchyDepartmentRepository _hierarchyDepartmentRepository;
        private readonly Func<string, ICryptographyService> _cryptographyService;
        private IOptions<AppSettings> _options;
        private readonly IUserCryptoService _userCryptoService;
        
        /// <summary>
        /// Initializes a new instance of the <see cref="UserRepository" /> class.
        /// </summary>
        /// <param name="dbContext">The unit of work</param>
        /// <param name="cryptographyService"></param>
        /// <param name="userTypeRepository"></param>
        /// <param name="workContext"></param>
        /// <param name="hierarchyDepartmetRepository"></param>
        /// <param name="options"></param>
        /// <param name="userCryptoService"></param>
        public UserRepository(OrganizationDbContext dbContext,
            Func<string, ICryptographyService> cryptographyService,
            IUserTypeRepository userTypeRepository,
            IWorkContext workContext,
            IHierarchyDepartmentRepository hierarchyDepartmetRepository,
            IOptions<AppSettings> options,
            IUserCryptoService userCryptoService)
            : base(dbContext, workContext)
        {
            _cryptographyService = cryptographyService;
            _userTypeRepository = userTypeRepository;
            _hierarchyDepartmentRepository = hierarchyDepartmetRepository;
            _options = options;
            _userCryptoService = userCryptoService;
        }
        public PaginatedList<UserEntity> GetUsers(int ownerId = 0,
            List<int> customerIds = null,
            List<int> userIds = null,
            List<int> userGroupIds = null,
            List<EntityStatusEnum> statusIds = null,
            List<ArchetypeEnum> archetypeIds = null,
            List<int> userTypeIds = null,
            List<string> userTypeExtIds = null,
            List<int> parentDepartmentIds = null,
            List<string> extIds = null,
            List<string> ssnList = null,
            List<string> userNames = null,
            DateTime? lastUpdatedBefore = null,
            DateTime? lastUpdatedAfter = null,
            List<AgeRange> ageRanges = null,
            List<Gender> genders = null,
            List<EntityStatusEnum> memberStatuses = null,
            DateTime? memberValidFromBefore = null,
            DateTime? memberValidFromAfter = null,
            DateTime? memberValidToBefore = null,
            DateTime? memberValidToAfter = null,
            string searchKey = null,
            List<string> loginServiceClaims = null,
            List<string> loginServiceClaimTypes = null,
            List<int> loginServiceIds = null,
            int pageIndex = 0,
            int pageSize = 0,
            string orderBy = "",
            IncludeDepartmentOption includeDepartment = IncludeDepartmentOption.None,
            bool includeLoginServiceUsers = false, 
            IncludeUserTypeOption  includeUserTypes = IncludeUserTypeOption.None,
            bool filterOnParentHd = true,
            IncludeUgMemberOption includeUGMembers = IncludeUgMemberOption.None,
            List<string> jsonDynamicData = null,
            bool? externallyMastered = null,
            bool skipPaging = false,
            DateTime? createdAfter = null,
            DateTime? createdBefore = null,
            DateTime? expirationDateAfter = null,
            DateTime? expirationDateBefore = null,
            List<int> orgUnittypeIds = null,
            List<List<int>> multiUserTypeFilters = null,
            List<List<int>> multiUserGroupFilters = null,
            List<List<string>> multiUserTypeExtIdFilters = null,
            List<string> emails = null,
            bool forUpdating = false,
            bool includeOwnUserGroups = false,
            DateTime? entityActiveDateBefore = null,
            DateTime? entityActiveDateAfter = null,
            bool filterOnUd = false,
            List<int> exceptUserIds = null)
        {
            //temporary to query user json array data
            if (jsonDynamicData != null && jsonDynamicData.Any(x => x.Contains("hrmsLeaveFrom")))
            {
                var jsonFilterData = jsonDynamicData.FirstOrDefault().Split("=");
                var jsonQuery = _dbSet.FromSqlRaw(@"select u.* from org.[User] u CROSS APPLY OPENJSON(u.DynamicAttributes ,'$.hrmsLeaveInfo')
                    WITH(
                    hrmsLeaveBeginDate nvarchar(100),
                    hrmsLeaveCanceled nvarchar(10)
                ) jsonData WHERE CAST(jsonData.hrmsLeaveBeginDate as DATE) <= {0} AND jsonData.hrmsLeaveCanceled <> 'Y' AND  u.EntityStatusId <> 2", jsonFilterData[1]);
                var jsonHasMoreData = false;
                //Build paging from IQueryable
                var jsonTotalItem = 0;
                var jsonItems =  jsonQuery.ToPaging(pageIndex, pageSize, skipPaging, out jsonHasMoreData, out jsonTotalItem);
                return new PaginatedList<UserEntity>(jsonItems, pageIndex, pageSize, jsonHasMoreData)
                { TotalItems = jsonTotalItem };
            }
            if (jsonDynamicData != null && jsonDynamicData.Any(x => x.Contains("hrmsLeaveEndDate")))
            {
                var jsonFilterData = jsonDynamicData.FirstOrDefault().Split("=");
                var jsonQuery = _dbSet.FromSqlRaw(@"select u.* from org.[User] u CROSS APPLY OPENJSON(u.DynamicAttributes ,'$.hrmsLeaveInfo')
                    WITH(
                    hrmsLeaveEndDate nvarchar(100),
                    hrmsLeaveCanceled nvarchar(10)
                ) jsonData WHERE CAST(jsonData.hrmsLeaveEndDate as DATE) <= {0} AND jsonData.hrmsLeaveCanceled <> 'Y' AND u.EntityStatusId = 2", jsonFilterData[1]);
                var jsonHasMoreData = false;
                //Build paging from IQueryable
                var jsonTotalItem = 0;
                var jsonItems =  jsonQuery.ToPaging(pageIndex, pageSize, skipPaging, out jsonHasMoreData, out jsonTotalItem);
                return new PaginatedList<UserEntity>(jsonItems, pageIndex, pageSize, jsonHasMoreData)
                { TotalItems = jsonTotalItem };
            }
            if (jsonDynamicData != null && jsonDynamicData.Any(x => x.Contains("hrmsExitFrom")))
            {
                var jsonFilterData = jsonDynamicData.FirstOrDefault().Split("=");
                var jsonQuery = _dbSet.FromSqlRaw("select * from org.[User] u where CAST(JSON_VALUE(u.DynamicAttributes, '$.hrmsExitDate') as DATE) <= {0} AND u.EntityStatusId <> 2", jsonFilterData[1]);
                var jsonHasMoreData = false;
                var jsonTotalItem = 0;
                var jsonItems = jsonQuery.ToPaging(pageIndex, pageSize, skipPaging, out jsonHasMoreData, out jsonTotalItem);
                return new PaginatedList<UserEntity>(jsonItems, pageIndex, pageSize, jsonHasMoreData) { TotalItems = jsonTotalItem };
            }


            if (!HandleUserTypeArguments(userTypeExtIds, multiUserTypeExtIdFilters, ref multiUserTypeFilters))
                return new PaginatedList<UserEntity>();

            var queryable = forUpdating ? GetAll() : GetAllAsNoTracking();
            var query = UserQueryBuilder.InitQueryBuilder(_options, _userCryptoService, queryable)
                .FilterByAges(ageRanges)
                .FilterByArchetypeIds(archetypeIds)
                .FilterByCustomerIds(customerIds)
                .FilterByDate(createdAfter: createdAfter, createdBefore: createdBefore,
                    lastUpdatedAfter: lastUpdatedAfter, lastUpdatedBefore: lastUpdatedBefore,
                    expirationDateAfter: expirationDateAfter, expirationDateBefore: expirationDateBefore,
                    entityActiveDateAfter: entityActiveDateAfter, entityActiveDateBefore: entityActiveDateBefore)
                .FilterByDepartmentIds(parentDepartmentIds, filterOnParentHd, filterOnUd)
                .FilterByGenders(genders)
                .FilterBySearchKey(searchKey, _options.Value.EnableSearchingSSN)
                .FilterBySsnList(ssnList)
                .FilterByStatusIds(statusIds)
                .FilterByUserExtIds(extIds)
                .FilterByUserGroupIds(userGroupIds: userGroupIds,
                    memberValidFromAfter: memberValidFromAfter,
                    memberValidFromBefore: memberValidFromBefore,
                    memberValidToAfter: memberValidToAfter,
                    memberValidToBefore: memberValidToBefore,
                    memberStatuses: memberStatuses)
                .FilterByMultiUserGroupFilters(multiUserGroupFilters: multiUserGroupFilters,
                    memberValidFromAfter: memberValidFromAfter,
                    memberValidFromBefore: memberValidFromBefore,
                    memberValidToAfter: memberValidToAfter,
                    memberValidToBefore: memberValidToBefore,
                    memberStatuses: memberStatuses)
                .FilterByUserIds(userIds, exceptUserIds)
                .FilterByOwnerId(ownerId)
                .FilterByUserTypeIds(userTypeIds)

                .FilterByMultiUserTypeFilters(multiUserTypeFilters)
                .FilterByUserNames(userNames)
                .FilterByEmails(emails)
                .FilterByLoginServiceInfo(loginServiceIds, loginServiceClaims, loginServiceClaimTypes)
                .FilterByJsonValue(jsonDynamicData)
                .IncludeDepartment(includeDepartment)
                .IncludeUserTypes(includeUserTypes)
                .IncludeUGMembers(includeUGMembers)
                .Build();

            if (orgUnittypeIds != null && orgUnittypeIds.Any())
                query = query.Where(t => t.Department.DT_Ds.Any(d => orgUnittypeIds.Contains(d.DepartmentTypeId)));

            if (externallyMastered.HasValue)
            {
                var locked = externallyMastered.Value ? 1 : 0;
                query = query.Where(x => x.Locked == locked);
            }
            if (includeLoginServiceUsers)
            {
                query = query.Include(q => q.LoginServiceUsers);}
         

            if (includeOwnUserGroups)
            {
                query = query.Include(q => q.UserGroups);
            }
            if (pageIndex == 0 && pageSize == 0 && !skipPaging)
            {
                pageIndex = 1;
                var itemsTemp = query.Take(MaximumRecordsReturn).ToList();
                return new PaginatedList<UserEntity>(itemsTemp, pageIndex, MaximumRecordsReturn, false) { TotalItems = itemsTemp.Count };
            }
            //Query must be ordered before apply paging
            if (searchKey != null)
                query = query.OrderBy(x => x.FirstName);
            else
                query = query.ApplyOrderBy(p => p.UserId, orderBy);
            var hasMoreData = false;
            //Build paging from IQueryable
            var totalItem = 0;
            var items = query.ToPaging(pageIndex, pageSize, skipPaging, out hasMoreData, out totalItem);
            return new PaginatedList<UserEntity>(items, pageIndex, pageSize, hasMoreData) { TotalItems = totalItem };
        }
        public async Task<PaginatedList<UserEntity>> GetUsersAsync(int ownerId = 0,
            List<int> customerIds = null,
            List<int> userIds = null,
            List<int> userGroupIds = null,
            List<EntityStatusEnum> statusIds = null,
            List<ArchetypeEnum> archetypeIds = null,
            List<int> userTypeIds = null,
            List<string> userTypeExtIds = null,
            List<int> parentDepartmentIds = null,
            List<string> departmentExtIds = null,
            List<string> extIds = null,
            List<string> ssnList = null,
            List<string> userNames = null,
            DateTime? lastUpdatedBefore = null,
            DateTime? lastUpdatedAfter = null,
            List<AgeRange> ageRanges = null,
            List<Gender> genders = null,
            List<EntityStatusEnum> memberStatuses = null,
            DateTime? memberValidFromBefore = null,
            DateTime? memberValidFromAfter = null,
            DateTime? memberValidToBefore = null,
            DateTime? memberValidToAfter = null,
            string searchKey = null,
            List<string> loginServiceClaims = null,
            List<string> loginServiceClaimTypes = null,
            List<int> loginServiceIds = null,
            int pageIndex = 0,
            int pageSize = 0,
            string orderBy = "",
            IncludeDepartmentOption includeDepartment = IncludeDepartmentOption.None,
            bool includeLoginServiceUsers = false,
            IncludeUserTypeOption includeUserTypes = IncludeUserTypeOption.None,
            bool filterOnParentHd = true,
            IncludeUgMemberOption includeUGMembers = IncludeUgMemberOption.None,
            List<string> jsonDynamicData = null,
            bool? externallyMastered = null,
            bool skipPaging = false,
            DateTime? createdAfter = null,
            DateTime? createdBefore = null,
            DateTime? expirationDateAfter = null,
            DateTime? expirationDateBefore = null,
            List<int> orgUnittypeIds = null,
            List<List<int>> multiUserTypeFilters = null,
            List<List<int>> multiUserGroupFilters = null,
            List<List<string>> multiUserTypeExtIdFilters = null,
            List<string> emails = null,
            bool forUpdating = false,
            bool includeOwnUserGroups = false,
            DateTime? entityActiveDateBefore = null,
            DateTime? entityActiveDateAfter = null,
            bool filterOnUd = false,
            List<int> exceptUserIds = null)
        {
            //temporary to query user json array data
            if (jsonDynamicData != null && jsonDynamicData.Any(x => x.Contains("hrmsLeaveFrom")))
            {
                var jsonFilterData = jsonDynamicData.FirstOrDefault().Split("=");
                var jsonQuery = _dbSet.FromSqlRaw(@"select u.* from org.[User] u CROSS APPLY OPENJSON(u.DynamicAttributes ,'$.hrmsLeaveInfo')
                    WITH(
                    hrmsLeaveBeginDate nvarchar(100),
                    hrmsLeaveCanceled nvarchar(10),
                    hrmsLeaveProcessed bit
                ) jsonData WHERE CAST(jsonData.hrmsLeaveBeginDate as DATE) <= {0} AND jsonData.hrmsLeaveCanceled = 'false' AND (jsonData.hrmsLeaveProcessed IS NULL or jsonData.hrmsLeaveProcessed = 0) AND u.EntityStatusId <> 2", jsonFilterData[1]);
                var jsonHasMoreData = false;
                var paginatedJsonItems = await jsonQuery.ToPagingAsync(pageIndex, pageSize, skipPaging);
                return new PaginatedList<UserEntity>(paginatedJsonItems.Items, pageIndex, pageSize, jsonHasMoreData)
                    {TotalItems = paginatedJsonItems.TotalItems};
            }
            if (jsonDynamicData != null && jsonDynamicData.Any(x => x.Contains("hrmsLeaveEndDate")))
            {
                var jsonFilterData = jsonDynamicData.FirstOrDefault().Split("=");
                var jsonQuery = _dbSet.FromSqlRaw(@"select u.* from org.[User] u CROSS APPLY OPENJSON(u.DynamicAttributes ,'$.hrmsLeaveInfo')
                    WITH(
                    hrmsLeaveEndDate nvarchar(100),
                    hrmsLeaveCanceled nvarchar(10),
                    hrmsLeaveProcessed bit
                ) jsonData WHERE CAST(jsonData.hrmsLeaveEndDate as DATE) <= {0} AND jsonData.hrmsLeaveCanceled = 'false' AND (jsonData.hrmsLeaveProcessed IS NULL or jsonData.hrmsLeaveProcessed = 0) AND u.EntityStatusId = 2", jsonFilterData[1]);
                var jsonHasMoreData = false;
                var paginatedJsonItems = await jsonQuery.ToPagingAsync(pageIndex, pageSize, skipPaging);
                return new PaginatedList<UserEntity>(paginatedJsonItems.Items, pageIndex, pageSize, jsonHasMoreData)
                { TotalItems = paginatedJsonItems.TotalItems };
            }
            if (jsonDynamicData != null && jsonDynamicData.Any(x => x.Contains("hrmsExitFrom")))
            {
                var jsonFilterData = jsonDynamicData.FirstOrDefault().Split("=");
                var jsonQuery = _dbSet.FromSqlRaw("select * from org.[User] u where CAST(JSON_VALUE(u.DynamicAttributes, '$.hrmsExitDate') as DATE) <= {0} AND u.EntityStatusId <> 2", jsonFilterData[1]);
                var jsonHasMoreData = false;
                var jsonTotalItem = 0;
                var jsonItems = jsonQuery.ToPaging(pageIndex, pageSize, skipPaging, out jsonHasMoreData, out jsonTotalItem);
                return new PaginatedList<UserEntity>(jsonItems, pageIndex, pageSize, jsonHasMoreData) { TotalItems = jsonTotalItem };
            }

            if (!HandleUserTypeArguments(userTypeExtIds, multiUserTypeExtIdFilters, ref multiUserTypeFilters))
                return new PaginatedList<UserEntity>();

            var queryable = forUpdating ? GetAll() : GetAllAsNoTracking();
            var query = UserQueryBuilder.InitQueryBuilder(_options, _userCryptoService, queryable)
                .FilterByAges(ageRanges)
                .FilterByArchetypeIds(archetypeIds)
                .FilterByCustomerIds(customerIds)
                .FilterByDate(createdAfter: createdAfter, createdBefore: createdBefore,
                    lastUpdatedAfter: lastUpdatedAfter, lastUpdatedBefore: lastUpdatedBefore,
                    expirationDateAfter: expirationDateAfter, expirationDateBefore: expirationDateBefore,
                    entityActiveDateAfter: entityActiveDateAfter, entityActiveDateBefore: entityActiveDateBefore)
                .FilterByDepartmentIds(parentDepartmentIds, filterOnParentHd, filterOnUd)
                .FilterByGenders(genders)
                .FilterBySearchKey(searchKey, _options.Value.EnableSearchingSSN)
                .FilterBySsnList(ssnList)
                .FilterByStatusIds(statusIds)
                .FilterByUserExtIds(extIds)
                .FilterByUserGroupIds(userGroupIds: userGroupIds,
                    memberValidFromAfter: memberValidFromAfter,
                    memberValidFromBefore: memberValidFromBefore,
                    memberValidToAfter: memberValidToAfter,
                    memberValidToBefore: memberValidToBefore,
                    memberStatuses: memberStatuses)
                .FilterByMultiUserGroupFilters(multiUserGroupFilters: multiUserGroupFilters,
                    memberValidFromAfter: memberValidFromAfter,
                    memberValidFromBefore: memberValidFromBefore,
                    memberValidToAfter: memberValidToAfter,
                    memberValidToBefore: memberValidToBefore,
                    memberStatuses: memberStatuses)
                .FilterByUserIds(userIds, exceptUserIds)
                .FilterByOwnerId(ownerId)
                .FilterByUserTypeIds(userTypeIds)
                //SPT perf issue -> use userTypeIds instead
                //.FilterByUserTypeExtIds(userTypeExtIds)
                .FilterByMultiUserTypeFilters(multiUserTypeFilters)
                .FilterByUserNames(userNames)
                .FilterByEmails(emails)
                .FilterByLoginServiceInfo(loginServiceIds, loginServiceClaims, loginServiceClaimTypes)
                .FilterByJsonValue(jsonDynamicData)
                .IncludeDepartment(includeDepartment)
                .FilterByDepartmentExtIds(departmentExtIds)
                .IncludeUserTypes(includeUserTypes)
                .IncludeUGMembers(includeUGMembers)
                .Build();

            if (orgUnittypeIds != null && orgUnittypeIds.Any())
                query = query.Where(t => t.Department.DT_Ds.Any(d => orgUnittypeIds.Contains(d.DepartmentTypeId)));

            if (externallyMastered.HasValue)
            {
                var locked = externallyMastered.Value ? 1 : 0;
                query = query.Where(x => x.Locked == locked);
            }
            if (includeLoginServiceUsers)
            {
                query = query.Include(q => q.LoginServiceUsers);
            }
            if (includeOwnUserGroups)
            {
                query = query.Include(q => q.UserGroups);
            }
            if (pageIndex == 0 && pageSize == 0 && !skipPaging)
            {
                pageIndex = 1;
                var itemsTemp = await query.Take(MaximumRecordsReturn).ToListAsync();
                return new PaginatedList<UserEntity>(itemsTemp, pageIndex, MaximumRecordsReturn, false) { TotalItems = itemsTemp.Count };
            }
            //Query must be ordered before apply paging
            if (searchKey != null)
                query = query.OrderBy(x => x.FirstName);
            else
                query = query.ApplyOrderBy(p => p.UserId, orderBy);
            var pagingResult = await query.ToPagingAsync(pageIndex, pageSize, skipPaging);
            return new PaginatedList<UserEntity>(pagingResult.Items, pageIndex, pageSize, pagingResult.HasMoreData) { TotalItems = pagingResult.TotalItems };
        }

        private bool HandleUserTypeArguments(List<string> userTypeExtIds, List<List<string>> multiUserTypeExtIdFilters, ref List<List<int>> multiUserTypeFilters)
        {
            ILookup<string, UserTypeEntity> usertypesLookup = null;
            // SPT perf issue -> use userTypeIds instead
            if (!userTypeExtIds.IsNullOrEmpty())
            {
                usertypesLookup = _userTypeRepository.GetAllUserTypesLookupByExtIdInCache();

                multiUserTypeFilters = multiUserTypeFilters ?? new List<List<int>>();

                var filteringUserTypeIds = FindFilteringUserTypeIds(userTypeExtIds, usertypesLookup);

                if (filteringUserTypeIds.IsNullOrEmpty())
                {
                    return false;
                }

                multiUserTypeFilters.Add(filteringUserTypeIds);
            }

            if (!multiUserTypeExtIdFilters.IsNullOrEmpty())
            {
                usertypesLookup = usertypesLookup ?? _userTypeRepository.GetAllUserTypesLookupByExtIdInCache();

                multiUserTypeFilters = multiUserTypeFilters ?? new List<List<int>>();
            
                foreach (var filteringUserTypeExtIds in multiUserTypeExtIdFilters)
                {
                    if (!filteringUserTypeExtIds.IsNullOrEmpty())
                    {
                        var filteringUserTypeIds = FindFilteringUserTypeIds(filteringUserTypeExtIds, usertypesLookup);
                        if (filteringUserTypeIds.IsNullOrEmpty())
                        {
                            return false;
                        }

                        multiUserTypeFilters.Add(filteringUserTypeIds);
                    }
                }
            }

            return true;
        }

        private static List<int> FindFilteringUserTypeIds(List<string> filteringUserTypeExtIds, ILookup<string, UserTypeEntity> usertypesLookup)
        {
            return (from ext in filteringUserTypeExtIds
                select usertypesLookup[ext].FirstOrDefault()
                into userType
                where userType != null
                select userType.UserTypeId).ToList();
        }

        public PaginatedList<UserEntity> SearchActors(int ownerId = 0,
            List<int> customerIds = null,
            List<int> userIds = null,
            List<int> userGroupIds = null,
            List<EntityStatusEnum> statusIds = null,
            List<ArchetypeEnum> archetypeIds = null,
            List<int> userTypeIds = null,
            List<string> userTypeExtIds = null,
            List<int> parentDepartmentIds = null,
            List<string> extIds = null,
            List<string> ssnList = null,
            List<string> userNames = null,
            DateTime? lastUpdatedBefore = null,
            DateTime? lastUpdatedAfter = null,
            List<AgeRange> ageRanges = null,
            List<Gender> genders = null,
            List<EntityStatusEnum> memberStatuses = null,
            DateTime? memberValidFromBefore = null,
            DateTime? memberValidFromAfter = null,
            DateTime? memberValidToBefore = null,
            DateTime? memberValidToAfter = null,
            string searchKey = null,
            List<string> loginServiceClaims = null,
            List<string> loginServiceClaimTypes = null,
            List<int> loginServiceIds = null,
            int pageIndex = 0,
            int pageSize = 0,
            string orderBy = "",
            bool includeDepartment = false,
            bool includeLoginServiceUsers = false,
            bool includeUserTypes = false,
            List<int> exceptUserIds = null)

        {
            var query = UserQueryBuilder.InitQueryBuilder(_options, _userCryptoService, GetAllAsNoTracking())
                            .FilterByAges(ageRanges)
                            .FilterByArchetypeIds(archetypeIds)
                            .FilterByCustomerIds(customerIds)
                            .FilterByDate(lastUpdatedBefore, lastUpdatedAfter)
                            .FilterByDepartmentIds(parentDepartmentIds)
                            .FilterByGenders(genders)
                            .FilterBySearchKey(searchKey, _options.Value.EnableSearchingSSN)
                            .FilterBySsnList(ssnList)
                            .FilterByStatusIds(statusIds)
                            .FilterByUserExtIds(extIds)
                            .FilterByUserGroupIds(userGroupIds: userGroupIds,
                                                  memberValidFromAfter: memberValidFromAfter,
                                                  memberValidFromBefore: memberValidFromBefore,
                                                  memberValidToAfter: memberValidToAfter,
                                                  memberValidToBefore: memberValidToBefore,
                                                  memberStatuses: memberStatuses)
                            .FilterByUserIds(userIds, exceptUserIds)
                            .FilterByOwnerId(ownerId)
                            .FilterByUserTypeIds(userTypeIds)
                            .FilterByUserTypeExtIds(userTypeExtIds)
                            .FilterByUserNames(userNames)
                            .FilterByLoginServiceInfo(loginServiceIds, loginServiceClaims, loginServiceClaimTypes)
                            .Build();

            if (includeDepartment)
            {
                query = query.Include(q => q.Department);
            }
            if (includeLoginServiceUsers)
            {
                query = query.Include(q => q.LoginServiceUsers);
            }
            if (includeUserTypes)
            {
                query = query.Include(q => q.UT_Us)
                             .ThenInclude(j => j.UserType)
                             .ThenInclude(t => t.LT_UserType);
            }

            query = query.ApplyOrderBy(p => p.UserId, orderBy);
            if (pageIndex == 0 && pageSize == 0)
            {
                pageIndex = 1;
                var itemsTemp = query.Take(MaximumRecordsReturn).ToList();
                return new PaginatedList<UserEntity>(itemsTemp, pageIndex, MaximumRecordsReturn, false);
            }
            var hasMoreData = false;
            //Build paging from IQueryable
            var totalItem = 0;
            var items = query.ToPaging(pageIndex, pageSize, out hasMoreData, out totalItem);
            return new PaginatedList<UserEntity>(items, pageIndex, pageSize, hasMoreData);
        }

        public List<UserEntity> GetUserByIds(List<int> userIds, Expression<Func<UserEntity, object>>[] includeProperties)
        {
            var query = GetAllIncluding(includeProperties);
            if (userIds != null && userIds.Any())
            {
                query = query.Where(t => userIds.Contains(t.UserId));
            }
            return query.ToList();
        }
        public List<UserEntity> GetUserByIds(List<int> userIds,
            bool includeUserType = false,
            bool includeUserGroup = false,
            bool includeDepartment = false)
        {
            IQueryable<UserEntity> query = BuildIncludeQuery(includeUserType, includeUserGroup, includeDepartment);

            if (!userIds.IsNullOrEmpty())
            {
                if (userIds.Count == 1)
                {
                    int value = userIds[0];
                    query = query.Where(t => t.UserId == value);
                }
                else
                {
                    query = query.Where(t => userIds.Contains(t.UserId));
                }
            }

            return query.ToList();
        }

        private IQueryable<UserEntity> BuildIncludeQuery(bool includeUserType = false,
            bool includeUserGroup = false,
            bool includeDepartment = false,
            bool includeUgMember = false,
            bool includeUserGroupFromUgMember = false)
        {
            var query = GetAll();
            if (includeUserType)
                query = query.Include(j => j.UT_Us).ThenInclude(t => t.UserType);
            if (includeUserGroup)
                query = query.Include(j => j.UserGroups);
            if (includeDepartment)
                query = query.Include(j => j.Department);
            if (includeUgMember)
                query = includeUserGroupFromUgMember ? query.Include(j => j.UGMembers).ThenInclude(t => t.UserGroup).AsQueryable() : query.Include(j => j.UGMembers);
            return query;
        }

        public UserEntity Insert(UserEntity entity, int useHashPassword, bool useOTP, int defaultHashMethod)
        {
            //Apply hash password
            var useHashPasswordStatus = (UseHashPasswordStatus)useHashPassword;
            //entity.ChangePassword = true;
            if (useOTP)
            {
                var result = false;
                if (!string.IsNullOrEmpty(Encryption.Decrypt(entity.OneTimePassword, ref result)))
                {
                    entity.OneTimePassword = entity.OneTimePassword;
                    entity.Password = Encryption.Encrypt(Generator.GenerateRandomPassword(10));
                }
            }
            else
            {
                if (useHashPasswordStatus == UseHashPasswordStatus.Use)
                {
                    if (!string.IsNullOrEmpty(entity.Password) && (string.IsNullOrEmpty(entity.HashPassword) || string.IsNullOrEmpty(entity.SaltPassword)))
                    {
                        var passwordSalt = _cryptographyService(defaultHashMethod.ToString(CultureInfo.InvariantCulture)).CreatePasswordSalt();
                        var hashedPassword = _cryptographyService(defaultHashMethod.ToString(CultureInfo.InvariantCulture)).CreatePasswordHash(entity.Password, passwordSalt);
                        entity.SaltPassword = passwordSalt;
                        entity.HashPassword = hashedPassword;
                        entity.Password = Encryption.Encrypt(string.Empty);
                    }
                }
                else if (useHashPasswordStatus == UseHashPasswordStatus.None)
                {
                    if (!string.IsNullOrEmpty(entity.Password) && (string.IsNullOrEmpty(entity.HashPassword) || string.IsNullOrEmpty(entity.SaltPassword)))
                    {
                        entity.SaltPassword = string.Empty;
                        entity.HashPassword = string.Empty;
                        entity.Password = Encryption.Encrypt(entity.Password);
                    }
                }
                else if (useHashPasswordStatus == UseHashPasswordStatus.Both)
                {
                    if (string.IsNullOrEmpty(entity.HashPassword) || string.IsNullOrEmpty(entity.SaltPassword))
                    {
                        var passwordSalt = _cryptographyService(defaultHashMethod.ToString(CultureInfo.InvariantCulture)).CreatePasswordSalt();
                        var hashedPassword = _cryptographyService(defaultHashMethod.ToString(CultureInfo.InvariantCulture)).CreatePasswordHash(entity.Password, passwordSalt);
                        entity.SaltPassword = passwordSalt;
                        entity.HashPassword = hashedPassword;

                        entity.Password = Encryption.Encrypt(Generator.GenerateRandomPassword(10));
                    }
                }
            }
            return base.Insert(entity);
        }
        public UserEntity Update(UserEntity entity,
            int useHashPassword,
            bool useOTP,
            int defaultHashMethod,
            bool changePassword = false,
            bool generateRandomPassword = true,
            string updatedPassword = "")
        {
            if (changePassword)
            {
                //Apply hash password
                var useHashPasswordStatus = (UseHashPasswordStatus)useHashPassword;
                if (useOTP)
                {
                    var result = false;
                    if (!string.IsNullOrEmpty(Encryption.Decrypt(entity.OneTimePassword, ref result)))
                    {
                        entity.OneTimePassword = entity.OneTimePassword;
                        entity.Password = Encryption.Encrypt(entity.Password);
                        return base.Update(entity);
                    }
                }
                if (!string.IsNullOrEmpty(entity.Password) || !string.IsNullOrEmpty(updatedPassword))
                {
                    updatedPassword = !string.IsNullOrEmpty(updatedPassword) ? updatedPassword : entity.Password;
                    if (useHashPasswordStatus == UseHashPasswordStatus.Use)
                    {
                        var passwordSalt = _cryptographyService(defaultHashMethod.ToString(CultureInfo.InvariantCulture)).CreatePasswordSalt();
                        var hashedPassword = _cryptographyService(defaultHashMethod.ToString(CultureInfo.InvariantCulture)).CreatePasswordHash(updatedPassword, passwordSalt);
                        entity.SaltPassword = passwordSalt;
                        entity.HashPassword = hashedPassword;
                        entity.Password = Encryption.Encrypt(string.Empty);
                    }
                    else if (useHashPasswordStatus == UseHashPasswordStatus.None)
                    {
                        entity.SaltPassword = string.Empty;
                        entity.HashPassword = string.Empty;
                        entity.Password = Encryption.Encrypt(entity.Password);
                    }
                    else if (useHashPasswordStatus == UseHashPasswordStatus.Both)
                    {
                        var passwordSalt = _cryptographyService(defaultHashMethod.ToString(CultureInfo.InvariantCulture)).CreatePasswordSalt();
                        var hashedPassword = _cryptographyService(defaultHashMethod.ToString(CultureInfo.InvariantCulture)).CreatePasswordHash(updatedPassword, passwordSalt);
                        entity.SaltPassword = passwordSalt;
                        entity.HashPassword = hashedPassword;
                        entity.Password = Encryption.Encrypt(generateRandomPassword ? Generator.GenerateRandomPassword(10) : entity.Password);
                    }
                }
            }
            return base.Update(entity);
        }

        public UserEntity GetUserById(int userId)
        {
            return GetAllAsNoTracking().Include(user => user.UT_Us)
                .ThenInclude(j => j.UserType)
                .Include(user => user.UGMembers.Select(t => t.UserGroup))
                .FirstOrDefault(user => user.UserId == userId);
        }
        public async Task<UserEntity> GetUserAsync(int ownerId, int? userId, string extId)
        {

            if ((!userId.HasValue || userId.Value <= 0) && string.IsNullOrEmpty(extId))
            {
                return null;
            }

            var query = GetAllAsNoTracking().Where(user => user.OwnerId == ownerId);
            if (userId > 0)
            {
                query = query.Where(q => q.UserId == userId.Value);
            }

            if (!string.IsNullOrEmpty(extId))
            {
                query = query.Where(q => q.ExtId == extId);
            }

            return await query.FirstOrDefaultAsync();
        }

        public List<UserEntity> GetUsersByDepartmentIds(List<int> departmentIds, params EntityStatusEnum[] filters)
        {
            return GetAllAsNoTracking(filters).Where(x => departmentIds.Contains(x.DepartmentId)).ToList();
        }

        public List<UserEntity> GetUsersByDepartmentIdIncludeUserTypesUserGroup(int departmentId, int ownerId, params EntityStatusEnum[] filters)
        {
            var result = GetAllAsNoTracking(filters).Include(user => user.UT_Us)
                .ThenInclude(j => j.UserType)
                .Include(p => p.UGMembers.Select(t => t.UserGroup))
                .Where(a => a.DepartmentId == departmentId && a.OwnerId == ownerId).ToList();
            var users = (from m in result select m).ToList();
            return users;
        }

        public List<UserEntity> GetUsersByUserGroupIds(List<int> userGroupIds,
            List<int> usertypeIds,
            List<int> gender = null,
            List<string> ages = null,
            string courtry = "")
        {
            if (gender == null)
                gender = new List<int>();
            var query = GetAll().Where(x => x.UGMembers.Any(u => userGroupIds.Contains(u.UserGroupId))
            && x.UT_Us.Any(t => usertypeIds.Contains(t.UserTypeId)) && (!gender.Any() || (gender.Contains(x.Gender.Value) && x.Gender.HasValue) || (gender.Contains(2) && (x.Gender == null || x.Gender == 2)))
            && (string.IsNullOrEmpty(courtry)) /*|| x.CountryCode == int.Parse(courtry)*/);
            var searchPredicate = PredicateBuilder.New<UserEntity>();
            if (ages != null && ages.Any())
            {
                foreach (var item in ages)
                {
                    var currentYear = DateTime.Now.Year;
                    var dataAges = item.Split('-');
                    var to = int.Parse(dataAges[1]);
                    var datetimeToYear = new DateTime(currentYear - to, 1, 1);
                    var from = int.Parse(dataAges[0]);
                    var datetimeFromYear = new DateTime(currentYear - from, 12, 31);
                    searchPredicate =
                    searchPredicate.Or(x =>
                    ((x.DateOfBirth.Value <= datetimeFromYear) && (x.DateOfBirth.Value >= datetimeToYear))
                    || (from == 0 && to == 0 && x.DateOfBirth == null));
                }

                query = query.AsExpandable().Where(searchPredicate);
            }
            return query.ToList();
        }
        public List<UserEntity> GetUsersByUserIdsAndArchetypeIds(List<long?> userIds, List<int> allowArchetypeIds)
        {
            return GetAll().Include(x => x.Department.H_D)
                     .Where(x => userIds.Contains(x.UserId) && x.ArchetypeId.HasValue && allowArchetypeIds.Contains(x.ArchetypeId.Value))
                     .ToList();
        }

        public UserEntity GetUserByUserExtId(string userExtId, int customerId = 0)
        {
            var query = GetAll(EntityStatusEnum.Active).Where(p => p.ExtId == userExtId);
            if (customerId > 0)
            {
                query = query.Where(p => p.CustomerId == customerId);
            }
            return query.FirstOrDefault();
        }
        public UserEntity GetUserBySSN(string ssn, bool includeDepartment = false, int customerId = 0)
        {
            var query = GetAllAsNoTracking().Where(t => !string.IsNullOrEmpty(t.SSN) && t.SSN.Equals(ssn));
            //Filter if customer id greater than 0
            if (customerId > 0)
            {
                query = query.Where(t => t.CustomerId == customerId);
            }
            if (includeDepartment)
            {
                query = query.Include(t => t.Department);
            }

            return query.FirstOrDefault();
        }
        public List<UserEntity> GetUsersByUserExtId(string extId)
        {
            return GetAll().Where(p => p.ExtId.ToLower() == extId.ToLower()).ToList();
        }
        public UserEntity GetDefaultUserByCustomer(int customerId)
        {
            return GetAll().FirstOrDefault(x => x.CustomerId == customerId);
        }

        public List<UserEntity> SearchUser(string firstSearchKey, string secondSearchKey, int departmentId, int hdId = 0, bool deepSearch = false, int maxTake = 0)
        {
            var query = GetAllAsNoTracking(EntityStatusEnum.Active).Where(x => x.FirstName.Contains(firstSearchKey)
                                                                  && x.LastName.Contains(secondSearchKey));
            if (!deepSearch)
            {
                query = query.Where(x => x.DepartmentId == departmentId);
            }
            else
            {
                var allDepartment = _hierarchyDepartmentRepository.GetAllDepartmentIdsFromAHierachyDepartmentToBelow(hdId);
                query = query.Where(x => allDepartment.Contains(x.DepartmentId));
            }
            query = maxTake > 0 ? query.OrderBy(x => x.FirstName).Take(maxTake) : query.OrderBy(x => x.FirstName);
            return query.ToList();
        }

        public List<UserEntity> SearchUser(string searchKey, int departmentId, int hdId = 0, bool deepSearch = false, int maxTake = 0)
        {
            var query = GetAllAsNoTracking(EntityStatusEnum.Active).Where(x => x.FirstName.Contains(searchKey)
            || x.LastName.Contains(searchKey)
            || x.Email.Contains(searchKey)
            || x.Mobile.Contains(searchKey)
            || x.SSN.Contains(searchKey));

            if (!deepSearch)
            {
                query = query.Where(x => x.DepartmentId == departmentId);
            }
            else
            {
                var allDepartment = _hierarchyDepartmentRepository.GetAllDepartmentIdsFromAHierachyDepartmentToBelow(hdId);
                query = query.Where(x => allDepartment.Contains(x.DepartmentId));
            }
            query = maxTake > 0 ? query.OrderBy(x => x.FirstName).Take(maxTake) : query.OrderBy(x => x.FirstName);
            return query.ToList();
        }
        public List<UserEntity> GetUsersIncludeUserTypes(List<int> userIds, bool includeDepartment = false)
        {
            var users = GetAll().Include(x => x.UT_Us)
                .ThenInclude(j => j.UserType)
                .ThenInclude(t => t.LT_UserType)
                .Where(u => userIds.Contains(u.UserId));
            if (includeDepartment)
                users = users.Include(x => x.Department);
            return users.ToList();
        }

        public void AddRemoveUserTypes(UserEntity user, List<int> userTypeIdsToAdd, List<int> userTypeIdsToRemove)
        {
            if (user != null)
            {
                UTUEntity userType;
                if (userTypeIdsToRemove != null)
                {
                    foreach (int userTypeId in userTypeIdsToRemove)
                    {
                        userType = user.UT_Us.FirstOrDefault(ut => ut.UserTypeId == userTypeId);
                        if (userType != null) // Remove from user if exist!
                        {
                            user.UT_Us.Remove(userType);
                        }
                    }
                }
                foreach (int userTypeId in userTypeIdsToAdd)
                {
                    userType = user.UT_Us.FirstOrDefault(ut => ut.UserTypeId == userTypeId);
                    if (userType == null) // Add to user if not exist!
                    {
                        user.UT_Us.Add(new UTUEntity()
                        {
                            UserId = user.UserId,
                            UserTypeId = userTypeId
                        });
                    }
                }
            }
        }

        public List<UserEntity> GetUsersByDepartmentIdsAndUserTypeIds(List<int> departmentIds, List<int> userTypeIds)
        {
            return GetAll().Include(user => user.UT_Us)
                .ThenInclude(j => j.UserType)
                .Where(user => departmentIds.Any(departmentId => departmentId == user.DepartmentId)
                && user.UT_Us.Any(usertype => userTypeIds.Any(usertypeId => usertypeId == usertype.UserTypeId)) && user.EntityStatusId == (short)EntityStatusEnum.Active).ToList();
        }

        public List<UserEntity> GetUsers(int ownerId, int? userId, string username, string extId)
        {
            var filterById = userId.HasValue && userId > 0;
            var filterByUsername = !string.IsNullOrEmpty(username);
            var filterByExtId = !string.IsNullOrEmpty(extId);
            if (!(filterById || filterByUsername || filterByExtId))
            {
                return new List<UserEntity>();
            }

            var users = GetAll().Include(x => x.UT_Us).ThenInclude(x => x.UserType).ThenInclude(x => x.LT_UserType).Include(x => x.UGMembers).ThenInclude(x => x.UserGroup).Where(user => user.OwnerId == ownerId
                   && (user.UserId == userId || (filterByUsername && user.UserName == username) || (filterByExtId && user.ExtId == extId))).ToList();

            return users;
        }
        public async Task<List<UserEntity>> GetUsersAsync(int ownerId, int? userId, string username, string extId)
        {
            var filterById = userId.HasValue && userId > 0;
            var filterByUsername = !string.IsNullOrEmpty(username);
            var filterByExtId = !string.IsNullOrEmpty(extId);
            if (!(filterById || filterByUsername || filterByExtId))
            {
                return new List<UserEntity>();
            }

            var users = await GetAll().Include(x => x.UT_Us).ThenInclude(x => x.UserType).ThenInclude(x => x.LT_UserType)
                .Include(x => x.UGMembers).ThenInclude(x => x.UserGroup).Where(user => user.OwnerId == ownerId
                                                                                       && (user.UserId == userId ||
                                                                                           (filterByUsername &&
                                                                                            user.UserName == username
                                                                                           ) || (filterByExtId &&
                                                                                                 user.ExtId == extId)))
                .ToListAsync();

            return users;
        }

        public List<UserEntity> GetUsersByUsername(int ownerId, string username, params EntityStatusEnum[] filters)
        {
            var users = base._dbSet.AsQueryable().Where(user => user.OwnerId == ownerId && user.UserName == username).ToList();

            return users;
        }

        public IEnumerable<UserEntity> GetUsersByDepartment(int departmentId, bool includeDepartment = true, bool includeUserType = true, bool includeUserGroup = false, bool putToCache = false, bool allowGetUserDeleted = false)
        {
            var query = GetAllAsNoTracking().Where(user => user.DepartmentId == departmentId);
            if (!allowGetUserDeleted)
            {
                query = query.Where(user => user.EntityStatusId == (short)EntityStatusEnum.Active);
            }
            if (includeDepartment)
                query = query.Include(user => user.Department);
            if (includeUserType)
                query = query.Include(user => user.UT_Us).ThenInclude(j => j.UserType).ThenInclude(j => j.LT_UserType);
            if (includeUserType)
                query = query.Include(user => user.UGMembers.Select(t => t.UserGroup));
            //return query.ToList();

            if (!putToCache)
                return query.ToList();
            IEnumerable<UserEntity> users = query.ToList();
            return users;


        }

        public List<UserEntity> GetListUserByDepartmentIds(IEnumerable<int> departmentIds, bool includeLinkedUsers)
        {
            if (includeLinkedUsers)
            {
                return GetAll().Where(p => departmentIds.Contains(p.DepartmentId) || p.U_D.Any(u => departmentIds.Contains(u.DepartmentId))).ToList();
            }
            return GetAll().Where(p => departmentIds.Contains(p.DepartmentId)).ToList();
        }
        public bool CheckUsername(int ownerId, int userId, string userName)
        {
            return GetAll().Count(p => p.UserId != userId && p.OwnerId == ownerId && p.UserName == userName) > 0;
        }

        public UserEntity GetUserForUpdateInsert(int userId)
        {
            var user = GetAll().Include(t => t.UGMembers.Select(p => p.UserGroup))
                .Include(j => j.UT_Us)
                .ThenInclude(j => j.UserType).Include(p => p.Department)
                .FirstOrDefault(u => u.UserId == userId);
            return user;
        }
        public List<UserEntity> GetUsersByUsernameForUpdate(int ownerId, string username, params EntityStatusEnum[] filters)
        {
            if (filters == null)
            {
                filters = new EntityStatusEnum[] { EntityStatusEnum.Active };
            }
            return GetAll(filters).Include(p => p.UGMembers.Select(t => t.UserGroup))
                .Include(p => p.UT_Us)
                .ThenInclude(j => j.UserType)
                .Where(user => user.OwnerId == ownerId && user.UserName == username).ToList();
        }
        public UserEntity GetUserIncludeDepartmentIncludeUserTypesIncludeUserGroups(int userId, bool putToCache = true)
        {
            if (!putToCache)
            {
                return GetAll().Include(p => p.Department)
                    .Include(p => p.UT_Us)
                    .ThenInclude(j => j.UserType)
                    .Include(p => p.UGMembers.Select(t => t.UserGroup)).
                    FirstOrDefault(p => p.UserId == userId);
            }
            var user = GetAll().Include(p => p.Department)
                .Include(p => p.UT_Us)
                .ThenInclude(j => j.UserType)
                .Include(p => p.UGMembers.Select(t => t.UserGroup)).
                FirstOrDefault(p => p.UserId == userId);
            return user;
        }

        public List<UserEntity> GetUsersByExtIds(int customerId,
            int ownerId,
            List<string> userExtIds,
            bool includeUserType = false,
            bool includeUserGroup = false,
            bool includeDepartment = false,
            bool includeUgMember = false,
            bool includeUserGroupFromUgMember = false)
        {
            return BuildIncludeQuery(includeUserType: includeUserType,
                                     includeDepartment: includeDepartment,
                                     includeUserGroup: includeUserGroup,
                                     includeUgMember: includeUgMember,
                                     includeUserGroupFromUgMember: includeUserGroupFromUgMember)
                                     .Where(x => userExtIds.Contains(x.ExtId)
                                                 && x.CustomerId == customerId
                                                 && x.OwnerId == ownerId).ToList();
        }

        public int CountUsers(int ownerId = 0,
          List<int> customerIds = null,
          List<int> userIds = null,
          List<int> userGroupIds = null,
          List<EntityStatusEnum> statusIds = null,
          List<ArchetypeEnum> archetypeIds = null,
          List<int> userTypeIds = null,
          List<string> userTypeExtIds = null,
          List<int> parentDepartmentIds = null,
          List<string> extIds = null,
          List<string> ssnList = null,
          List<string> userNames = null,
          DateTime? lastUpdatedBefore = null,
          DateTime? lastUpdatedAfter = null,
          List<AgeRange> ageRanges = null,
          List<Gender> genders = null,
          bool filterOnParentHd = true,
          List<string> jsonDynamicData = null,
          DateTime? createdAfter = null,
          DateTime? createdBefore = null,
          bool? externallyMastered = null,
          bool filterOnUd = false,
          List<int> exceptUserIds = null)

        {
            var query = BuildCountUsersQuery(ownerId: ownerId,
                customerIds: customerIds,
                userIds: userIds,
                userGroupIds: userGroupIds,
                statusIds: statusIds,
                archetypeIds: archetypeIds,
                userTypeIds: userTypeIds,
                userTypeExtIds: userTypeExtIds,
                parentDepartmentIds: parentDepartmentIds,
                extIds: extIds,
                ssnList: ssnList,
                userNames: userNames,
                lastUpdatedBefore: lastUpdatedBefore,
                lastUpdatedAfter: lastUpdatedAfter,
                ageRanges: ageRanges,
                genders: genders,
                filterOnParentHd: filterOnParentHd,
                jsonDynamicData: jsonDynamicData,
                createdAfter: createdAfter,
                createdBefore: createdBefore,
                externallyMastered: externallyMastered,
                filterOnUd: filterOnUd,
                exceptUserIds: exceptUserIds);
            return query.Count();
        }
        public async Task<int> CountUsersAsync(int ownerId = 0,
         List<int> customerIds = null,
         List<int> userIds = null,
         List<int> userGroupIds = null,
         List<EntityStatusEnum> statusIds = null,
         List<ArchetypeEnum> archetypeIds = null,
         List<int> userTypeIds = null,
         List<string> userTypeExtIds = null,
         List<int> parentDepartmentIds = null,
         List<string> extIds = null,
         List<string> ssnList = null,
         List<string> userNames = null,
         DateTime? lastUpdatedBefore = null,
         DateTime? lastUpdatedAfter = null,
         List<AgeRange> ageRanges = null,
         List<Gender> genders = null,
         bool filterOnParentHd = true,
         List<string> jsonDynamicData = null,
         DateTime? createdAfter = null,
         DateTime? createdBefore = null,
         bool? externallyMastered = null,
         bool filterOnUd = false,
         List<int> exceptUserIds = null)

        {
            var query = BuildCountUsersQuery(ownerId: ownerId,
                customerIds: customerIds,
                userIds: userIds,
                userGroupIds: userGroupIds,
                statusIds: statusIds,
                archetypeIds: archetypeIds,
                userTypeIds: userTypeIds,
                userTypeExtIds: userTypeExtIds,
                parentDepartmentIds: parentDepartmentIds,
                extIds: extIds,
                ssnList: ssnList,
                userNames: userNames,
                lastUpdatedBefore: lastUpdatedBefore,
                lastUpdatedAfter: lastUpdatedAfter,
                ageRanges: ageRanges,
                genders: genders,
                filterOnParentHd: filterOnParentHd,
                jsonDynamicData: jsonDynamicData,
                createdAfter: createdAfter,
                createdBefore: createdBefore,
                externallyMastered: externallyMastered,
                filterOnUd: filterOnUd,
                exceptUserIds: exceptUserIds);
            return  await  query.CountAsync();
        }
        private IQueryable<UserEntity> BuildCountUsersQuery(int ownerId, List<int> customerIds, List<int> userIds, List<int> userGroupIds, List<EntityStatusEnum> statusIds,
            List<ArchetypeEnum> archetypeIds, List<int> userTypeIds, List<string> userTypeExtIds, List<int> parentDepartmentIds, List<string> extIds, List<string> ssnList,
            List<string> userNames, DateTime? lastUpdatedBefore, DateTime? lastUpdatedAfter, List<AgeRange> ageRanges, List<Gender> genders,
            bool filterOnParentHd, List<string> jsonDynamicData, DateTime? createdAfter, DateTime? createdBefore,
            bool? externallyMastered, bool filterOnUd,
            List<int> exceptUserIds)
        {
            var query = UserQueryBuilder.InitQueryBuilder(_options, _userCryptoService, GetAllAsNoTracking())
                .FilterByAges(ageRanges)
                .FilterByArchetypeIds(archetypeIds)
                .FilterByCustomerIds(customerIds)
                .FilterByDate(lastUpdatedBefore, lastUpdatedAfter)
                .FilterByDepartmentIds(parentDepartmentIds, filterOnParentHd, filterOnUd)
                .FilterByUserGroupIds(userGroupIds, null, null, null, null, null)
                .FilterByGenders(genders)
                .FilterBySsnList(ssnList)
                .FilterByStatusIds(statusIds)
                .FilterByUserExtIds(extIds)
                .FilterByUserIds(userIds, exceptUserIds)
                .FilterByOwnerId(ownerId)
                .FilterByUserTypeExtIds(userTypeExtIds)
                .FilterByUserTypeIds(userTypeIds)
                .FilterByUserNames(userNames)
                .FilterByJsonValue(jsonDynamicData)
                .Build();

            if (createdAfter.HasValue)
                query = query.Where(t => t.Created >= createdAfter);
            if (createdBefore.HasValue)
                query = query.Where(t => t.Created <= createdBefore);

            if (externallyMastered.HasValue)
            {
                var locked = externallyMastered.Value ? 1 : 0;
                query = query.Where(x => x.Locked == locked);
            }

            return query;
        }

        public IDictionary<int, int> CountUsersGroupByDepartment(int ownerId = 0,
         List<int> customerIds = null,
         List<int> userIds = null,
         List<int> userGroupIds = null,
         List<EntityStatusEnum> statusIds = null,
         List<ArchetypeEnum> archetypeIds = null,
         List<int> userTypeIds = null,
         List<string> userTypeExtIds = null,
         List<int> parentDepartmentIds = null,
         List<string> userExtIds = null,
         List<string> ssnList = null,
         List<string> userNames = null,
         DateTime? lastUpdatedBefore = null,
         DateTime? lastUpdatedAfter = null,
         List<AgeRange> ageRanges = null,
         List<Gender> genders = null,
         bool filterOnParentHd = true,
         bool filterOnUd = false,
         List<int> exceptUserIds = null)
        {
            IQueryable<UserEntity> query = GetUserGroupByDepartmentQuery(ownerId: ownerId, customerIds: customerIds,
                userIds: userIds, userGroupIds: userGroupIds,
                statusIds: statusIds, archetypeIds: archetypeIds, userTypeIds: userTypeIds,
                userTypeExtIds: userTypeExtIds, parentDepartmentIds: parentDepartmentIds, userExtIds: userExtIds,
                ssnList: ssnList, userNames: userNames, lastUpdatedBefore: lastUpdatedBefore,
                lastUpdatedAfter: lastUpdatedAfter,
                ageRanges: ageRanges, genders: genders, filterOnParentHd: filterOnParentHd, filterOnUd: filterOnUd,
                exceptUserIds: exceptUserIds);
            return query.ToList().GroupBy(g => g.DepartmentId).ToDictionary(g => g.Key, g => g.Count());
        }
        public async Task<Dictionary<int, int>> CountUsersGroupByDepartmentAsync(int ownerId = 0,
         List<int> customerIds = null,
         List<int> userIds = null,
         List<int> userGroupIds = null,
         List<EntityStatusEnum> statusIds = null,
         List<ArchetypeEnum> archetypeIds = null,
         List<int> userTypeIds = null,
         List<string> userTypeExtIds = null,
         List<int> parentDepartmentIds = null,
         List<string> userExtIds = null,
         List<string> ssnList = null,
         List<string> userNames = null,
         DateTime? lastUpdatedBefore = null,
         DateTime? lastUpdatedAfter = null,
         List<AgeRange> ageRanges = null,
         List<Gender> genders = null,
         bool filterOnParentHd = true,
         bool filterOnUd = false,
         List<int> exceptUserIds= null)
        {
            IQueryable<UserEntity> query = GetUserGroupByDepartmentQuery(ownerId: ownerId, customerIds: customerIds,
                userIds: userIds, userGroupIds: userGroupIds,
                statusIds: statusIds, archetypeIds: archetypeIds, userTypeIds: userTypeIds,
                userTypeExtIds: userTypeExtIds, parentDepartmentIds: parentDepartmentIds, userExtIds: userExtIds,
                ssnList: ssnList, userNames: userNames, lastUpdatedBefore: lastUpdatedBefore,
                lastUpdatedAfter: lastUpdatedAfter,
                ageRanges: ageRanges, genders: genders, filterOnParentHd: filterOnParentHd, filterOnUd: filterOnUd,
                exceptUserIds: exceptUserIds);
            var result = await query.ToListAsync();
            return result.GroupBy(g => g.DepartmentId).ToDictionary(g => g.Key, g => g.Count());
        }

        private IQueryable<UserEntity> GetUserGroupByDepartmentQuery(int ownerId,
            List<int> customerIds,
            List<int> userIds,
            List<int> userGroupIds,
            List<EntityStatusEnum> statusIds,
            List<ArchetypeEnum> archetypeIds,
            List<int> userTypeIds,
            List<string> userTypeExtIds,
            List<int> parentDepartmentIds,
            List<string> userExtIds,
            List<string> ssnList,
            List<string> userNames,
            DateTime? lastUpdatedBefore,
            DateTime? lastUpdatedAfter,
            List<AgeRange> ageRanges,
            List<Gender> genders,
            bool filterOnParentHd,
            bool filterOnUd,
            List<int> exceptUserIds)
        {
            return UserQueryBuilder.InitQueryBuilder(_options, _userCryptoService, GetAllAsNoTracking())
                                      .FilterByAges(ageRanges)
                                      .FilterByArchetypeIds(archetypeIds)
                                      .FilterByCustomerIds(customerIds)
                                      .FilterByDate(lastUpdatedBefore, lastUpdatedAfter)
                                      .FilterByDepartmentIds(parentDepartmentIds, filterOnParentHd, filterOnUd)
                                      .FilterByUserGroupIds(userGroupIds, null, null, null, null, null)
                                      .FilterByGenders(genders)
                                      .FilterBySsnList(ssnList)
                                      .FilterByStatusIds(statusIds)
                                      .FilterByUserExtIds(userExtIds)
                                      .FilterByUserIds(userIds, exceptUserIds)
                                      .FilterByOwnerId(ownerId)
                                      .FilterByUserTypeExtIds(userTypeExtIds)
                                      .FilterByUserTypeIds(userTypeIds)
                                      .FilterByUserNames(userNames)
                                      .Build();
        }

        public List<string> GetModifiedProperties(UserEntity entity)
        {
            var modifiedProperties = new List<string>();
            var properties = _dbContext.Entry(entity).Properties;
            foreach (var propertyEntry in properties)
            {
                if (propertyEntry.IsModified) modifiedProperties.Add(propertyEntry.Metadata.Name);
            }

            return modifiedProperties;
        }

        public async Task<UserEntity> GetOrSetUserFromWorkContext(IWorkContext workContext)
        {
            if (string.IsNullOrEmpty(workContext.Sub))
            {
                throw new CXValidationException(cxExceptionCodes.ERROR_CUSTOM, $"WorkContext.Sub is empty.");
            }

            UserEntity userEntity = workContext.CurrentUser as UserEntity;
            if (userEntity == null || !string.Equals(workContext.Sub, userEntity.ExtId, StringComparison.InvariantCultureIgnoreCase))
            {
                userEntity =
                    await GetAllAsNoTracking()
                    .Include(q => q.UT_Us)  // Don't include UserType since it should be getting from memory cache.
                    .FirstOrDefaultAsync(u => u.ExtId == workContext.Sub);  // TODO SPT filter on ExtID;

                if (userEntity == null)
                {
                    throw new CXValidationException(cxExceptionCodes.ERROR_NOT_FOUND, $"Not found the logged-in user with sub '{workContext.Sub}'.");
                }
                workContext.CurrentUser = userEntity;
            }

            return userEntity;
        }

        public async Task<IList<UserRole>> GetOrSetUserRoleFromWorkContext(IWorkContext workContext)
        {
            UserEntity userEntity = await GetOrSetUserFromWorkContext(workContext);

            if (userEntity != null && workContext.CurrentUserRoles == null)
            {
                var userTypes = _userTypeRepository.GetAllUserTypesInCache()
                    .Where(ut => ut.ArchetypeId == (int)ArchetypeEnum.SystemRole || ut.ArchetypeId == (int)ArchetypeEnum.Role).ToList();

                var currentUserRoles = 
                    (from utu in userEntity.UT_Us
                     join userType in userTypes on utu.UserTypeId equals userType.UserTypeId
                     select new UserRole { Id = utu.UserTypeId, ExtId = userType.ExtId });

                // in case current user has current roles, we will remove them after retrieved from DB (extId is empty) and add the default one.
                var hasCustomRoles = currentUserRoles.Any(currentUserRole => string.IsNullOrEmpty(currentUserRole.ExtId));
                if (hasCustomRoles)
                {
                    currentUserRoles = currentUserRoles.Where(currentUserRole => !string.IsNullOrEmpty(currentUserRole.ExtId));
                    currentUserRoles = currentUserRoles.Append(new UserRole
                    {
                        ExtId = "customrole",
                        Id = 9999
                    });
                }
                workContext.CurrentUserRoles = currentUserRoles.ToList();
            }
            return workContext.CurrentUserRoles;
        }

        public IQueryable<UserEntity> GetQueryAsNoTracking(params EntityStatusEnum[] entityStatus)
        {
            return GetAllAsNoTracking(entityStatus);
        }
        public IQueryable<UserEntity> GetQueryIncludeDeletedUsers()
        {
            return _dbSet.FromSqlRaw($"select * from org.[User] where Ownerid = {_workContext.CurrentOwnerId} AND CustomerID = {_workContext.CurrentCustomerId}");
        }

        public async Task<List<UserAccountStatisticsInfo>> GetUserStatisticsInfos(int ownerId, int customerId, List<EntityStatusEnum> entityStatuses,
            DateTime? createdAfter, DateTime? createdBefore)
        {
            var sqlParameters = new List<SqlParameter>();

            var sqFilter = new StringBuilder();
            if (!entityStatuses.IsNullOrEmpty())
            {
                sqFilter.Append(entityStatuses.Count == 1
                    ? $" and u.EntityStatusId = {(int) entityStatuses[0]}"
                    : $" and u.EntityStatusId in ({string.Join(",", entityStatuses.Select(e => (int) e))})");
            }
            if (createdAfter.HasValue)
            {
                sqlParameters.AddSingleValueParameter("@createdAfter", createdAfter.Value);
                sqFilter.Append(" and u.Created >=@createdAfter");
            }
            if (createdBefore.HasValue)
            {
                sqlParameters.AddSingleValueParameter("@createdBefore", createdBefore.Value);
                sqFilter.Append(" and u.Created <=@createdBefore");
            }
            var sqlCommand =
                $@"select u.EntityStatusId, u.Locked, ISNULL(JSON_VALUE(d.DynamicAttributes, '$.typeOfOrganizationUnits'),'')  as TypeOfOrganizationUnitId,  count (*)  NumberOfUser from org.[User] u
                INNER JOIN org.Department d on d.DepartmentID=u.DepartmentID
                where d.Deleted is NULL and u.OwnerId= {ownerId} and u.CustomerID = {customerId}{sqFilter}
                GROUP by  u.EntityStatusID, u.Locked, isnull(JSON_VALUE(d.DynamicAttributes, '$.typeOfOrganizationUnits'),'') 
                ";

            return await _dbContext.ExecSQLAsync<UserAccountStatisticsInfo>(sqlCommand, null, sqlParameters.ToArray());

        }

        public async Task<PaginatedList<UserEntity>> GetAllUsers(int pageIndex)
        {
            IQueryable<UserEntity> query = GetAllAsNoTracking()
                .Include(j => j.UT_Us)
                    .ThenInclude(t => t.UserType)
                .Include(j => j.UserGroups)
                .Include(j => j.Department)
                .Include(j => j.UGMembers)
                    .ThenInclude(t => t.UserGroup)
                    .AsQueryable();

            //Build paging from IQueryable
            var hasMoreData = false;
            var paginatedJsonItems = await query.ToPagingAsync(pageIndex, 5000, false);
            return new PaginatedList<UserEntity>(paginatedJsonItems.Items, pageIndex, 5000, paginatedJsonItems.HasMoreData)
            { TotalItems = paginatedJsonItems.TotalItems };
        }


        public async Task<(List<CountUserEntity> UserCountValues,int TotalUser)> CountUserGroupByAsync(int ownerId,
            List<int> customerIds = null,
            List<int> userIds = null,
            List<int> userGroupIds = null,
            List<EntityStatusEnum> statusIds = null,
            List<ArchetypeEnum> archetypeIds = null,
            List<int> userTypeIds = null,
            List<int> departmentIds = null,
            List<string> extIds = null,
            List<string> jsonDynamicData = null,
            List<int> exceptUserIds = null,
            List<List<int>> multiUserTypeFilters = null,
            List<List<int>> multiUserGroupFilters = null,
            DateTime? lastUpdatedBefore = null,
            DateTime? lastUpdatedAfter = null,
            DateTime? createdBefore = null,
            DateTime? createdAfter = null,
            UserGroupByField groupByField = UserGroupByField.None)
        {
            var groupField = GetGroupByField("u", groupByField, out var leftJoinExpressionBuilder);

            string selectGroupField = null;
            string groupByExpression = null;

            if (!string.IsNullOrEmpty(groupField))
            {
                selectGroupField = groupField;
                groupByExpression = $"GROUP BY {groupField}";
            }
            else
            {
                selectGroupField = "null";
            }

            var whereMainExpressions = new List<string>();
            var sqlParameters = new List<SqlParameter>();

            AddSqlExpressionFilter("u", ownerId: ownerId, customerIds: customerIds,
                userIds: userIds,
                userGroupIds: userGroupIds,
                statusIds: statusIds,
                archetypeIds: archetypeIds,
                userTypeIds: userTypeIds,
                departmentIds: departmentIds,
                extIds: extIds,
                jsonDynamicData: jsonDynamicData,
                exceptUserIds: exceptUserIds,
                multiUserTypeFilters: multiUserTypeFilters,
                multiUserGroupFilters: multiUserGroupFilters,
                lastUpdatedBefore: lastUpdatedBefore,
                lastUpdatedAfter: lastUpdatedAfter,
                createdBefore: createdBefore,
                createdAfter: createdAfter,
                whereMainExpressions: whereMainExpressions,
                sqlParameters: sqlParameters);

            var whereMain = string.Join(" AND ", whereMainExpressions.Where(w => !string.IsNullOrEmpty(w)));
            if (!string.IsNullOrEmpty(whereMain))
            {
                whereMain = $" AND {whereMain}";
            }

            var sql = $@"SELECT {selectGroupField} {nameof(CountUserEntity.GroupValue)}, count(DISTINCT u.UserID) {nameof(CountUserEntity.UserCount)}
                       FROM org.[User] u
                       {leftJoinExpressionBuilder}
                       WHERE u.Deleted is NULL {whereMain}
                       {groupByExpression}";
            var countingData = await _dbContext.ExecSQLAsync<CountUserEntity>(sql, null, sqlParameters.ToArray());

            int totalUser;
            if (groupByField == UserGroupByField.None)
            {
                totalUser = countingData.Any() ? countingData[0].UserCount : 0;
            }
            else
            {
                //One user could be a member of multiple user group, user type,
                //we cannot sum value of each group to get total unique user, we must get it separately
                var sqlTotalUser = $@"SELECT count(DISTINCT u.UserID)
                       FROM org.[User] u
                       WHERE u.Deleted is NULL {whereMain}";
                totalUser = await _dbContext.ExecuteScalarAsync<int>(sqlTotalUser, null, sqlParameters.ToArray());
            }

            return (countingData, totalUser);
        }

        private static  string GetGroupByField(string userTableAlias, UserGroupByField groupByField, out StringBuilder leftJoinExpressionBuilder)
        {
            leftJoinExpressionBuilder = new StringBuilder();
            string groupField = null;
            switch (groupByField)
            {
                case UserGroupByField.UserGroupId:
                    leftJoinExpressionBuilder.Append(
                        $@" LEFT JOIN org.UGMember ugm on ugm.UserID={userTableAlias}.UserID and ugm.Deleted is NULL and ugm.EntityStatusID={(int)EntityStatusEnum.Active} and (ugm.ValidFrom is null OR ugm.ValidFrom<=GETDATE()) and (ugm.ValidTo IS NULL or GETDATE()<= ugm.ValidTo)");
                    groupField = "ugm.UserGroupID";
                    break;
                case UserGroupByField.UserTypeId:

                    leftJoinExpressionBuilder.Append($" LEFT JOIN org.UT_U utu on utu.UserID={userTableAlias}.UserID");
                    groupField = "utu.UserTypeID";
                    break;
                case UserGroupByField.DepartmentId:
                    groupField = $"{userTableAlias}.DepartmentId";
                    break;
                case UserGroupByField.EntityStatusId:
                    groupField = $"{userTableAlias}.EntityStatusId";
                    break;
            }

            return groupField;
        }

        private static void AddSqlExpressionFilter(string userTableAlias, int ownerId, List<int> customerIds, List<int> userIds, List<int> userGroupIds, List<EntityStatusEnum> statusIds,
            List<ArchetypeEnum> archetypeIds, List<int> userTypeIds, List<int> departmentIds, List<string> extIds, List<string> jsonDynamicData, List<int> exceptUserIds,
            List<List<int>> multiUserTypeFilters, List<List<int>> multiUserGroupFilters, DateTime? lastUpdatedBefore, DateTime? lastUpdatedAfter,
            DateTime? createdBefore, DateTime? createdAfter, List<string> whereMainExpressions, List<SqlParameter> sqlParameters)
        {
            if (multiUserGroupFilters == null) multiUserGroupFilters = new List<List<int>>();
            if (multiUserTypeFilters == null) multiUserTypeFilters = new List<List<int>>();

            if (!userGroupIds.IsNullOrEmpty())
            {
                multiUserGroupFilters.Add(userGroupIds);
            }

            if (!userTypeIds.IsNullOrEmpty())
            {
                multiUserTypeFilters.Add(userTypeIds);
            }
            if (ownerId > 0)
            {
                whereMainExpressions.Add($"{userTableAlias}.OwnerId={ownerId}");
            }
            whereMainExpressions.Add(
                statusIds.ToEqualOrContainSqlExpression($"{userTableAlias}.EntityStatusID", EntityStatusEnum.Active));
            whereMainExpressions.Add(userIds.ToEqualOrContainSqlExpression($"{userTableAlias}.UserId"));
            whereMainExpressions.Add(extIds.ToEqualOrContainSqlExpression($"{userTableAlias}.ExtId", true));
            whereMainExpressions.Add(customerIds.ToEqualOrContainSqlExpression($"{userTableAlias}.CustomerId"));
            whereMainExpressions.Add(departmentIds.ToEqualOrContainSqlExpression($"{userTableAlias}.DepartmentID"));
            whereMainExpressions.Add(userIds.ToEqualOrContainSqlExpression($"{userTableAlias}.UserId"));
            whereMainExpressions.Add(exceptUserIds.ToNotEqualOrNotContainSqlExpression($"{userTableAlias}.UserId"));
            whereMainExpressions.Add(archetypeIds.ToEqualOrContainSqlExpression($"{userTableAlias}.ArchetypeID"));

            AddSqlExpressionForDateTimeFilter(userTableAlias, lastUpdatedBefore, lastUpdatedAfter, createdBefore, createdAfter,
                whereMainExpressions, sqlParameters);
            AddSqlExpressionForJsonDynamicFilter(userTableAlias, jsonDynamicData, whereMainExpressions);
            AddSqlExpressionForMultiUserTypeFilter(userTableAlias, multiUserTypeFilters, whereMainExpressions);
            AddSqlExpressionForMultiUserGroupFilter(userTableAlias, multiUserGroupFilters, whereMainExpressions);
        }

        private static void AddSqlExpressionForMultiUserTypeFilter(string userTableAlias, List<List<int>> multiUserTypeFilters, List<string> whereMainExpressions)
        {
            for (int i = 0; i < multiUserTypeFilters.Count; i++)
            {
                var filterUserTypeIds = multiUserTypeFilters[i];
                if (filterUserTypeIds.IsNullOrEmpty()) continue;

                var filterOnUserTypeExpression =
                    $"EXISTS(select 1 from org.UT_U utu{i} where utu{i}.UserID={userTableAlias}.UserID and {filterUserTypeIds.ToEqualOrContainSqlExpression($"utu{i}.UserTypeID")}";
                whereMainExpressions.Add(filterOnUserTypeExpression);
            }
        }
        private static void AddSqlExpressionForMultiUserGroupFilter(string userTableAlias, List<List<int>> multiUserGroupFilters, List<string> whereMainExpressions)
        {
            for (int i = 0; i < multiUserGroupFilters.Count; i++)
            {
                var filterUserGroupIds = multiUserGroupFilters[i];
                if (filterUserGroupIds.IsNullOrEmpty()) continue;

                var filterOnUserTypeExpression =
                    $@"EXISTS(SELECT 1 from org.UGMember ugm{i} 
                      where ugm{i}.UserID={userTableAlias}.UserID 
                      and {filterUserGroupIds.ToEqualOrContainSqlExpression($"ugm{i}.UserGroupId")}
                      and ugm{i}.Deleted is NULL 
                      and ugm{i}.EntityStatusID={(int)EntityStatusEnum.Active} 
                      and (ugm{i}.ValidFrom is null OR ugm{i}.ValidFrom<=GETDATE()) and (ugm{i}.ValidTo IS NULL or GETDATE()<= ugm{i}.ValidTo))";
                whereMainExpressions.Add(filterOnUserTypeExpression);
            }
        }
        private static void AddSqlExpressionForDateTimeFilter(string userTableAlias, DateTime? lastUpdatedBefore, DateTime? lastUpdatedAfter,
            DateTime? createdBefore, DateTime? createdAfter, List<string> whereMainExpression, List<SqlParameter> sqlParameters)
        {
            if (lastUpdatedAfter.HasValue)
            {
                whereMainExpression.Add($"{userTableAlias}.LastUpdated >= @lastUpdatedAfter");
                sqlParameters.AddSingleValueParameter("@lastUpdatedAfter", lastUpdatedAfter.Value);
            }

            if (lastUpdatedBefore.HasValue)
            {
                whereMainExpression.Add($"{userTableAlias}.LastUpdated <= @lastUpdatedBefore");
                sqlParameters.AddSingleValueParameter("@lastUpdatedBefore", lastUpdatedBefore.Value);
            }


            if (createdAfter.HasValue)
            {
                whereMainExpression.Add($"{userTableAlias}.Created >= @createdAfter");
                sqlParameters.AddSingleValueParameter("@createdAfter", createdAfter.Value);
            }

            if (createdBefore.HasValue)
            {
                whereMainExpression.Add($"{userTableAlias}.Created <= @createdBefore");
                sqlParameters.AddSingleValueParameter("@createdBefore", createdBefore.Value);
            }
        }

        private static void AddSqlExpressionForJsonDynamicFilter(string userTableAlias, List<string> jsonDynamicData, List<string> whereMainExpression)
        {
            if (jsonDynamicData != null && jsonDynamicData.Count > 0)
            {
                var jsonDynamicFilters = QueryBuilderExtension.BuildJsonDynamicFilters(jsonDynamicData);
                foreach (var jsonDynamicFilter in jsonDynamicFilters)
                {
                    var jsonValues = jsonDynamicFilter.Values;
                    if (jsonValues != null && jsonValues.Count > 0)
                    {
                        var orIsNull = jsonValues.Contains("null");
                        var comparableValues = orIsNull ? jsonValues.Where(v => v != "null").ToList() : jsonValues;

                        var jsonPath = jsonDynamicFilter.JsonPath;
                        var isArray = jsonPath.EndsWith("[]");
                        if (isArray)
                        {
                            jsonPath = jsonPath.Replace("[]", "");
                        }

                        var jsonFunction = isArray ? "JSON_QUERY" : "JSON_VALUE";
                        var jonLeftExpression = $"{jsonFunction}({userTableAlias}.DynamicAttributes,{jsonPath})";
                        var jsonComparisionExpressions = new List<string>();
                        if (orIsNull)
                        {
                            jsonComparisionExpressions.Add($"{jonLeftExpression} is null");
                            jsonComparisionExpressions.Add($"{jonLeftExpression} = 'null'");
                        }

                        if (comparableValues.Count > 0)
                        {
                            string multiValueOperator = null;
                            if (jsonDynamicFilter.Operator == ComparisonOperator.Equal)
                            {
                                multiValueOperator = "IN";
                            }
                            else if (jsonDynamicFilter.Operator == ComparisonOperator.NotEqual)
                            {
                                multiValueOperator = "NOT IN";
                            }

                            if (comparableValues.Count > 1 && multiValueOperator == null)
                            {
                                throw new NotSupportedException(
                                    $"Do not support filter on json attribute '{jsonPath}' with comparison operator '{jsonDynamicFilter.Operator}' against multiple values");
                            }

                            jsonComparisionExpressions.Add(comparableValues.ToSqlExpression(jonLeftExpression,
                                jsonDynamicFilter.OperatorSign,
                                multiValueOperator, true));
                        }

                        whereMainExpression.Add(string.Join(" OR ", jsonComparisionExpressions));
                    }
                }
            }
        }
    }
   
}


