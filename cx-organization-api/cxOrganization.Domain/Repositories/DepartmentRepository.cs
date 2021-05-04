using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using cxOrganization.Adapter.Shared.Extensions;
using cxOrganization.Domain.AdvancedWorkContext;
using cxOrganization.Domain.Entities;
using cxPlatform.Client.ConexusBase;
using cxPlatform.Core;
using cxPlatform.Core.Extentions;
using Microsoft.EntityFrameworkCore;

namespace cxOrganization.Domain.Repositories
{
    /// <summary>
    /// Class DepartmentRepository
    /// </summary>
    public class DepartmentRepository : EntityBaseRepository<DepartmentEntity>, IDepartmentRepository
    {
        const int MaximumRecordsReturn = 5000;
        private readonly IDepartmentTypeRepository _departmentTypeRepository;

        /// <summary>
        /// Initializes a new instance of the <see cref="DepartmentRepository" /> class.
        /// </summary>
        /// <param name="dbContext">The unit of work.</param>
        /// <param name="cacheProvider"></param>
        /// <param name="departmentTypeRepository">The user group repository.</param>
        /// <param name="entityFrameworkCache"></param>
        public DepartmentRepository(OrganizationDbContext dbContext,
            IAdvancedWorkContext workContext,
            IDepartmentTypeRepository departmentTypeRepository)
            : base(dbContext, workContext)
        {
            _departmentTypeRepository = departmentTypeRepository;
        }

        public async Task<PaginatedList<DepartmentEntity>> GetDepartmentsAsync(int ownerId, List<int> userIds,
            List<int> customerIds,
            List<int> departmentIds,
            List<EntityStatusEnum> statusIds,
            List<int> archetypeIds,
            int parentDepartmentId,
            int childrenDepartmentId,
            List<string> departmetTypeExtIds,
            List<string> extIds,
            List<int> parentDepartmentIds = null,
            DateTime? lastUpdatedBefore = null,
            DateTime? lastUpdatedAfter = null,
            bool? externallyMastered = null,
            bool? includeDepartmentType = true,
            string searchText = "",
            int pageIndex = 0,
            int pageSize = 0,
            string orderBy = "")
        {
            var departmentTypeIds = await _departmentTypeRepository.GetDepartmentTypeIdsAsync(departmetTypeExtIds);

            var query = BuildDepartmentQuery(
                ownerId: ownerId,
                userIds: userIds,
                customerIds: customerIds,
                departmentIds: departmentIds,
                statusIds: statusIds,
                archetypeIds: archetypeIds,
                parentDepartmentId: parentDepartmentId,
                childrenDepartmentId: childrenDepartmentId,
                departmentTypeIds: departmentTypeIds,
                extIds: extIds,
                lastUpdatedBefore: lastUpdatedBefore,
                lastUpdatedAfter: lastUpdatedAfter,
                parentDepartmentIds: parentDepartmentIds,
                externallyMastered: externallyMastered,
                includeDepartmentType: includeDepartmentType,
                searchText: searchText,
                orderBy: orderBy);

            // Additional order
            query = query.OrderBy(department => department.Name);

            var pagingResult = await query.ToPagingAsync(pageIndex, pageSize);

            return new PaginatedList<DepartmentEntity>(pagingResult.Items, pageIndex, pageSize, pagingResult.HasMoreData);
        }

        public PaginatedList<DepartmentEntity> GetDepartments(int ownerId, List<int> userIds,
            List<int> customerIds,
            List<int> departmentIds,
            List<EntityStatusEnum> statusIds,
            List<int> archetypeIds,
            int parentDepartmentId,
            int childrenDepartmentId,
            List<string> departmetTypeExtIds,
            List<string> extIds,
            DateTime? lastUpdatedBefore = null,
            DateTime? lastUpdatedAfter = null,
            int pageIndex = 0,
            int pageSize = 0,
            string orderBy = "")
        {
            var departmentTypeIds = _departmentTypeRepository.GetDepartmentTypeIds(departmetTypeExtIds);

            var query = BuildDepartmentQuery(
                ownerId,
                userIds,
                customerIds,
                departmentIds,
                statusIds,
                archetypeIds,
                parentDepartmentId,
                childrenDepartmentId,
                departmentTypeIds,
                extIds,
                lastUpdatedBefore,
                lastUpdatedAfter,
                null,
                null,
                true,
                orderBy);
            var hasMoreData = false;
            //Build paging from IQueryable
            var totalItem = 0;
            var items = query.ToPaging(pageIndex, pageSize, out hasMoreData, out totalItem);
            return new PaginatedList<DepartmentEntity>(items, pageIndex, pageSize, hasMoreData);
        }

        private IQueryable<DepartmentEntity> BuildDepartmentQuery(
            int ownerId, List<int> userIds, List<int> customerIds, List<int> departmentIds,
            List<EntityStatusEnum> statusIds, List<int> archetypeIds, int parentDepartmentId, int childrenDepartmentId,
            List<int> departmentTypeIds, List<string> extIds, DateTime? lastUpdatedBefore, DateTime? lastUpdatedAfter,
            List<int> parentDepartmentIds,
            bool? externallyMastered = null,
            bool? includeDepartmentType = true,
            string searchText = "",
            string orderBy = "")
        {
            var query = GetAllAsNoTracking();
            if (ownerId > 0)
            {
                query = query.Where(p => p.OwnerId == ownerId);
            }
            var combinedParentDepartmentIds = parentDepartmentIds.IsNullOrEmpty()
                ? new List<int>() { parentDepartmentId }
                : parentDepartmentIds;
            combinedParentDepartmentIds = combinedParentDepartmentIds.Where(p => p > 0).ToList();
            if (!combinedParentDepartmentIds.IsNullOrEmpty())
            {
                if (combinedParentDepartmentIds.Count == 1)
                {
                    query = query.Where(t => t.H_D.Any(x => combinedParentDepartmentIds[0] == x.Parent.DepartmentId));
                }
                else
                {
                    query = query.Where(t => t.H_D.Any(x => combinedParentDepartmentIds.Contains(x.Parent.DepartmentId)));
                }
            }
            if (childrenDepartmentId > 0)
            {
                query = query.Where(t => t.H_D.Any(x => x.H_Ds.Any(o => o.DepartmentId == childrenDepartmentId)));
            }
            if (departmentTypeIds != null && departmentTypeIds.Any())
            {
                query = query.Where(t => t.DT_Ds.Any(x => departmentTypeIds.Contains(x.DepartmentTypeId)));
            }
            if (customerIds != null && customerIds.Any())
            {
                query = query.Where(p => customerIds.Contains(p.CustomerId.Value));
            }
            if (departmentIds != null && departmentIds.Any())
            {
                query = query.Where(p => departmentIds.Contains(p.DepartmentId));
            }
            if (extIds != null && extIds.Any())
            {
                query = query.Where(p => extIds.Contains(p.ExtId));
            }
            if (!string.IsNullOrWhiteSpace(searchText))
            {
                query = query.Where(p => p.Name.Contains(searchText));
            }
            if (statusIds == null || !statusIds.Any())
            {
                query = query.Where(p => p.EntityStatusId.Value == (int)EntityStatusEnum.Active);
            }
            else
                if (!statusIds.Contains(EntityStatusEnum.All))
            {
                query = query.Where(p => statusIds.Contains((EntityStatusEnum)p.EntityStatusId.Value));
            }
            if (archetypeIds != null && archetypeIds.Any())
            {
                query = query.Where(p => archetypeIds.Contains(p.ArchetypeId.Value));
            }
            if (lastUpdatedBefore.HasValue)
            {
                query = query.Where(p => p.LastUpdated <= lastUpdatedBefore);
            }
            if (lastUpdatedAfter.HasValue)
            {
                query = query.Where(p => p.LastUpdated >= lastUpdatedAfter);
            }
            if (userIds != null && userIds.Any())
            {
                query = query.Where(p => p.Users.Any(u => userIds.Contains(u.UserId)));
            }
            if (externallyMastered.HasValue)
            {
                var locked = externallyMastered.Value ? 1 : 0;
                query = query.Where(x => x.Locked == locked);
            }

            if (includeDepartmentType == true)
            {
                query = query.Include(t => t.H_D).ThenInclude(j => j.Parent).Include(x => x.DT_Ds).ThenInclude(j => j.DepartmentType).ThenInclude(x => x.LT_DepartmentType);
            }
            else
            {
                query = query.Include(t => t.H_D).ThenInclude(j => j.Parent).Include(x => x.DT_Ds);
            }

            //Query must be ordered before apply paging
            query = query.ApplyOrderBy(p => p.DepartmentId, orderBy);
            return query;
        }

        public List<DepartmentEntity> GetParentDepartment(int departmentId)
        {
            var department = GetAllAsNoTracking().Include(p => p.H_D).ThenInclude(x => x.Parent).Where(t => t.DepartmentId == departmentId).FirstOrDefault();
            List<DepartmentEntity> result = new List<DepartmentEntity>();
            foreach (var hd in department.H_D)
            {
                result.Add(GetAllAsNoTracking().Include(t => t.H_D.Select(x => x.Parent)).Where(t => t.DepartmentId == hd.Parent.DepartmentId).FirstOrDefault());
            }
            return result;
        }
        public DepartmentEntity GetDepartmentByExtId(string departmentExtId, int customerId, bool includedInActiveStatus = false)
        {
            var query = GetAll().Where(t => t.ExtId == departmentExtId && t.CustomerId == customerId);
            if (!includedInActiveStatus)
            {
                query = query.Where(t => t.EntityStatusId == (short)EntityStatusEnum.Active);
            }
            return query.Include(t => t.H_D).ThenInclude(x => x.Parent).FirstOrDefault();
        }

        public Task<DepartmentEntity> GetDepartmentByExtIdAsync(string departmentExtId, List<int> customerIds, bool includedInActiveStatus = false)
        {
            var query = GetAllAsNoTracking()
                .Where(t => t.ExtId == departmentExtId);
            if (!customerIds.IsNullOrEmpty())
            {
                query = customerIds.Count == 1
                    ? query.Where(dept => dept.CustomerId == customerIds.First())
                    : query.Where(dept => dept.CustomerId.HasValue && customerIds.Contains(dept.CustomerId.Value));
            }
            if (!includedInActiveStatus)
            {
                query = query.Where(t => t.EntityStatusId == (short)EntityStatusEnum.Active);
            }
            return query.FirstOrDefaultAsync();
        }

        public DepartmentEntity GetDepartmentByExtIdIncludeHd(string extId, int ownerId, bool includedInActiveStatus = false)
        {
            var query = GetAll().Include(t => t.H_D).Where(t => t.ExtId == extId && t.OwnerId == ownerId);
            if (!includedInActiveStatus)
            {
                query = query.Where(t => t.EntityStatusId == (short)EntityStatusEnum.Active);
            }
            return query.FirstOrDefault();
        }

        public DepartmentEntity GetDepartmentByIdIncludeHd(int departmentId, int ownerId, int customerId)
        {
            var query = GetAll().Include(x=>x.DT_Ds).ThenInclude(y => y.DepartmentType).Include(t => t.H_D)
                .Where(t => t.DepartmentId == departmentId && t.OwnerId == ownerId && t.CustomerId == customerId);
            return query.FirstOrDefault();
        }

        public List<DepartmentEntity> GetDepartmentByIdsAndArchetypeId(List<long?> departmentIds, List<int> allowArchetypeIds)
        {
            return GetAll().Include(x => x.H_D.Select(t => t.Parent))
                .Where(x => departmentIds.Contains(x.DepartmentId) && x.ArchetypeId.HasValue && allowArchetypeIds.Contains(x.ArchetypeId.Value))
                .ToList();
        }

        public List<DepartmentEntity> GetDepartmentsByDepartmentIds(List<int> departmentIds, bool includeDepartmentTypes = false)
        {
            var query = GetAll().Where(t => departmentIds.Contains(t.DepartmentId));
            if (includeDepartmentTypes)
                query = query.Include(x => x.DT_Ds)
                             .ThenInclude(j => j.DepartmentType);
            return query.ToList();
        }

        public DepartmentEntity GetDepartmentIncludeDepartmentTypes(int departmentId, int ownerId)
        {
            return GetAll().Include(x => x.DT_Ds)
                             .ThenInclude(j => j.DepartmentType)
                             .ThenInclude(t => t.LT_DepartmentType)
                .FirstOrDefault(d => d.DepartmentId == departmentId && d.OwnerId == ownerId);
        }

        public DepartmentEntity GetDepartmentIncludeDepartmentTypes(int departmentId)
        {
            return GetAll().Include(j => j.DT_Ds)
                           .ThenInclude(j => j.DepartmentType)
                           .Include(t => t.H_D)
                           .ThenInclude(x => x.Parent)
                           .FirstOrDefault(d => d.DepartmentId == departmentId);
        }

        public List<DepartmentEntity> GetDepartmentsByIdOrExtId(int? id, string extId)
        {
            var filterById = id.HasValue && id > 0;
            var filterByExtId = !string.IsNullOrEmpty(extId);
            if (!(filterById || filterByExtId))
            {
                return new List<DepartmentEntity>();
            }
            return GetAll().Where(f => (f.DepartmentId == id || (filterByExtId && f.ExtId.ToLower() == extId.ToLower())))
            .Include(t => t.H_D)
            .ThenInclude(x => x.Parent)
            .Include(x => x.DT_Ds)
            .ThenInclude(j => j.DepartmentType).ToList();
        }

        public DepartmentEntity GetDepartment(int departmentId, int ownerId, bool includeInActiveStatus = false)
        {
            var query = GetAllAsNoTracking().Include(u => u.Users)
                                            .ThenInclude(t => t.UT_Us)
                                            .ThenInclude(j => j.UserType)
                                            .Where(d => d.DepartmentId == departmentId && d.OwnerId == ownerId);
            if (!includeInActiveStatus)
            {
                query = query.Where(u => u.EntityStatusId == (short)EntityStatusEnum.Active);
            }
            return query.FirstOrDefault();
        }

        public List<DepartmentEntity> GetDepartmentByNames(List<string> departmentNames)
        {
            return GetAllAsNoTracking()
                        .Include(department => department.H_D)
                        .Where(department => departmentNames.Contains(department.Name))
                        .ToList();
        }
        public List<string> GetModifiedProperties(DepartmentEntity entity)
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
