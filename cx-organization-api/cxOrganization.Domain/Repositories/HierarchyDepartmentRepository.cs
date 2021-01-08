using cxOrganization.Domain.Dtos.Departments;
using cxOrganization.Domain.Entities;
using cxOrganization.Domain.Extensions;
using cxOrganization.Domain.Repositories.QueryBuilders;
using cxPlatform.Client.ConexusBase;
using cxPlatform.Core;
using cxPlatform.Core.BaseEnums;
using cxPlatform.Core.Cache;
using cxPlatform.Core.Extentions;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace cxOrganization.Domain.Repositories
{
    /// <summary>
    /// Class HierarchyDepartmentRepository
    /// </summary>
    public class HierarchyDepartmentRepository : RepositoryBase<HierarchyDepartmentEntity>, IHierarchyDepartmentRepository
    {
        private readonly IMemoryCacheProvider _memoryCacheProvider;
        private readonly ILogger<HierarchyDepartmentRepository> _logger;
        private static readonly object Lockobject = new object();
        private static readonly SemaphoreSlim SemaphoreSlim = new SemaphoreSlim(1);
        /// <summary>
        /// Initializes a new instance of the <see cref="HierarchyDepartmentRepository" /> class.
        /// </summary>
        public HierarchyDepartmentRepository(OrganizationDbContext dbContext,
            IMemoryCacheProvider memoryCacheProvider,
            ILogger<HierarchyDepartmentRepository> logger)
            : base(dbContext)
        {
            _memoryCacheProvider = memoryCacheProvider;
            _logger = logger;
        }

        public override HierarchyDepartmentEntity GetById(params object[] Id)
        {
            var cacheKey = BuildCacheKey(Id);

            if (_memoryCacheProvider.TryGetValue(cacheKey, out var hierarchyDepartmentEntity))
            {
                return (HierarchyDepartmentEntity)hierarchyDepartmentEntity;
            }

            lock (Lockobject)
            {
                _logger.LogWarning($"{cacheKey} HierarchyDepartmentRepository GetById not found in cache");
                return _memoryCacheProvider.GetOrCreate(cacheKey, (cache) => base.GetById(Id));
            }
        }

        public override async Task<HierarchyDepartmentEntity> GetByIdAsync(params object[] Id)
        {
            var cacheKey = BuildCacheKey(Id);

            if (_memoryCacheProvider.TryGetValue(cacheKey, out var hierarchyDepartmentEntity))
            {
                return (HierarchyDepartmentEntity) hierarchyDepartmentEntity;
            }

            await SemaphoreSlim.WaitAsync();

            var hdEntity = await _memoryCacheProvider.GetOrCreateAsync(cacheKey, async (cache) => await base.GetByIdAsync(Id));

            SemaphoreSlim.Release();

            return hdEntity;
        }

        private static string BuildCacheKey(object[] Id)
        {
            var cacheKey = $"HierarchyDepartmentEntity#Id={string.Join(",", Id)}";
            return cacheKey;
        }

        /// <summary>
        /// Gets the hierachy department.
        /// </summary>
        /// <param name="hierarchyId">The hierarchy id.</param>
        /// <param name="departmentId">The department id.</param>
        /// <returns>H_D.</returns>
        public HierarchyDepartmentEntity GetHierachyDepartment(int hierarchyId, int departmentId)
        {
            return GetAllAsNoTracking()
                .Include(h => h.H_Ds)
                .ThenInclude(d => d.Department)
                .Include(h => h.Department)
                .ThenInclude(j => j.DT_Ds)
                .ThenInclude(j => j.DepartmentType)
                .ThenInclude(j => j.LT_DepartmentType)
                .FirstOrDefault(p => p.HierarchyId == hierarchyId && p.DepartmentId == departmentId);
        }
        public Task<HierarchyDepartmentEntity> GetHierachyDepartmentAsync(int hierarchyId, int departmentId)
        {
            return GetAllAsNoTracking()
                .Include(h => h.H_Ds)
                .ThenInclude(d => d.Department)
                .Include(h => h.Department)
                .ThenInclude(j => j.DT_Ds)
                .ThenInclude(j => j.DepartmentType)
                .ThenInclude(j => j.LT_DepartmentType)
                .FirstOrDefaultAsync(p => p.HierarchyId == hierarchyId && p.DepartmentId == departmentId);
        }

        /// <summary>
        /// Gets the hierachy department by hierachy id and department id.
        /// </summary>
        /// <param name="hierarchyId">The hierarchy id.</param>
        /// <param name="departmentId">The department id.</param>
        /// <returns>H_D.</returns>
        public HierarchyDepartmentEntity GetHierachyDepartmentByHierachyIdAndDepartmentId(int hierarchyId, int departmentId, int ownerId = 0, List<int> customerIds = null, List<EntityStatusEnum> departmentEntityStatuses = null, List<int> departmentTypeIds = null)
        {
            IQueryable<HierarchyDepartmentEntity> query = BuildGetHDByHierachyIdAndDepartmentId(hierarchyId, departmentId, ownerId, customerIds, departmentEntityStatuses, departmentTypeIds);
            return query.FirstOrDefault();
        }
        public Task<HierarchyDepartmentEntity> GetHierachyDepartmentByHierachyIdAndDepartmentIdAsync(int hierarchyId, int departmentId, int ownerId = 0, List<int> customerIds = null, List<EntityStatusEnum> departmentEntityStatuses = null, List<int> departmentTypeIds = null)
        {
            IQueryable<HierarchyDepartmentEntity> query = BuildGetHDByHierachyIdAndDepartmentId(hierarchyId, departmentId, ownerId, customerIds, departmentEntityStatuses, departmentTypeIds);
            return query.FirstOrDefaultAsync();
        }

        private IQueryable<HierarchyDepartmentEntity> BuildGetHDByHierachyIdAndDepartmentId(int hierarchyId, int departmentId, int ownerId, List<int> customerIds, List<EntityStatusEnum> departmentEntityStatuses, List<int> departmentTypeIds)
        {
            var query = GetAll()
                            .Include(x => x.Department).ThenInclude(x => x.DT_Ds).ThenInclude(x => x.DepartmentType).ThenInclude(x => x.LT_DepartmentType)
                            .Where(p => p.Deleted == 0 & p.HierarchyId == hierarchyId && p.DepartmentId == departmentId);

            query = QueryWithOwnerAndCustomer(ownerId, customerIds, query);
            query = QueryWithDepartmentEntityStatuses(query, departmentEntityStatuses);
            if (departmentTypeIds != null && departmentTypeIds.Any())
            {
                query = query.Where(p => p.Department.DT_Ds.Any(o => departmentTypeIds.Contains(o.DepartmentTypeId)));
            }

            return query;
        }

        /// <summary>
        /// Gets the hierachy department by hierachy id and department id.
        /// </summary>
        /// <param name="hierarchyId">The hierarchy id.</param>
        /// <param name="hdid">The hd id.</param>
        /// <returns>H_D.</returns>
        public HierarchyDepartmentEntity GetHierachyDepartmentByHierachyIdAndHdId(int hierarchyId, int hdId, int ownerId = 0, List<int> customerIds = null, List<EntityStatusEnum> departmentEntityStatuses = null, List<int> departmentTypeIds = null, bool includeDepartmentType = false)
        {
            IQueryable<HierarchyDepartmentEntity> query = BuildGetHierachyDepartmentByHdId(hierarchyId, hdId, ownerId, customerIds, departmentEntityStatuses, departmentTypeIds, includeDepartmentType);
            return query.FirstOrDefault();
        }
        public Task<HierarchyDepartmentEntity> GetHierachyDepartmentByHierachyIdAndHdIdAsync(int hierarchyId, int hdId, int ownerId = 0, List<int> customerIds = null, List<EntityStatusEnum> departmentEntityStatuses = null, List<int> departmentTypeIds = null, bool includeDepartmentType = false)
        {
            IQueryable<HierarchyDepartmentEntity> query = BuildGetHierachyDepartmentByHdId(hierarchyId, hdId, ownerId, customerIds, departmentEntityStatuses, departmentTypeIds, includeDepartmentType);
            return query.FirstOrDefaultAsync();
        }
        private IQueryable<HierarchyDepartmentEntity> BuildGetHierachyDepartmentByHdId(int hierarchyId, int hdId, int ownerId, List<int> customerIds, List<EntityStatusEnum> departmentEntityStatuses, List<int> departmentTypeIds, bool includeDepartmentType)
        {
            var query = GetAll()
                            .Include(x => x.Department)
                            .Where(p => p.HierarchyId == hierarchyId && p.HDId == hdId);

            query = QueryWithOwnerAndCustomer(ownerId, customerIds, query);
            query = QueryWithDepartmentEntityStatuses(query, departmentEntityStatuses);
            if (departmentTypeIds != null && departmentTypeIds.Any())
            {
                query = query.Where(p => p.Department.DT_Ds.Any(o => departmentTypeIds.Contains(o.DepartmentTypeId)));
            }
            if (includeDepartmentType)
            {
                query = query.Include(x => x.Department).ThenInclude(x => x.DT_Ds).ThenInclude(x => x.DepartmentType).ThenInclude(x => x.LT_DepartmentType);
            }

            return query;
        }

        /// <summary>
        /// Gets the hd by hierarchy id and department id.
        /// </summary>
        /// <param name="hierarchyId">The hierarchy id.</param>
        /// <param name="departmentId">The department id.</param>
        /// <param name="includeInActiveStatus"></param>
        /// <returns></returns>
        public HierarchyDepartmentEntity GetHdByHierarchyIdAndDepartmentId(int hierarchyId, int departmentId, bool includeInActiveStatus = false)
        {
            if (includeInActiveStatus)
            {
                return GetAll().FirstOrDefault(t => t.HierarchyId == hierarchyId && t.DepartmentId == departmentId);
            }
            else
            {
                return GetAll().FirstOrDefault(t => t.HierarchyId == hierarchyId && t.DepartmentId == departmentId && t.Deleted != 1);
            }
        }
        public async Task<HierarchyDepartmentEntity> GetHdByHierarchyIdAndDepartmentIdAsync(int hierarchyId, int departmentId, bool includeInActiveStatus = false)
        {
            if (includeInActiveStatus)
            {
                return await GetAll().FirstOrDefaultAsync(t => t.HierarchyId == hierarchyId && t.DepartmentId == departmentId);
            }
            else
            {
                return await GetAll().FirstOrDefaultAsync(t => t.HierarchyId == hierarchyId && t.DepartmentId == departmentId && t.Deleted != 1);
            }
        }

        /// <summary>
        /// Get all hd Ids from a hierachy department to below
        /// </summary>
        /// <param name="hierarchyDepartmentId">The hierarchy department id.</param>
        /// <param name="maxLevel">Maximum hd level should be get. if value maxLevel >0, we will get hd to that maxLevel, otherwise, we will get all level. Min value is 1</param>
        /// <returns>List of int H_D.</returns>
        public List<int> GetAllHDIdsFromAHierachyDepartmentToBelow(int hierarchyDepartmentId, bool getAllStatus = false, int? maxLevel = null)
        {
            var statusFilter = "and hd.[Deleted] = 0";
            if (getAllStatus)
            {
                statusFilter = string.Empty;
            }

            var sqlCommand = "";
            if (maxLevel > 0)
            {
                sqlCommand = string.Format(";WITH H AS " +
                         "(  select hd.[HDID], hd.[DepartmentID], 1 as [RecursiveLevel] " +
                             "FROM[org].[H_D] hd WHERE hd.[HDID] = {0} {1} " +
                             "UNION all " +
                             "select hd.[HDID], hd.[DepartmentID], H.[RecursiveLevel] + 1 AS [RecursiveLevel] " +
                             "FROM[org].[H_D] hd JOIN H ON hd.[ParentID] = h.HDID {1} " +
                          ") " +
                          "SELECT HDID FROM H " +
                          "WHERE H.[RecursiveLevel] <={2}", hierarchyDepartmentId, statusFilter, maxLevel + 1);

            }
            else
            {

                sqlCommand = string.Format(";WITH H AS " +
                           "(  select hd.[HDID], hd.[DepartmentID] " +
                               "FROM[org].[H_D] hd WHERE hd.[HDID] = {0} {1} " +
                               "UNION all " +
                               "select hd.[HDID], hd.[DepartmentID] " +
                               "FROM[org].[H_D] hd JOIN H ON hd.[ParentID] = h.HDID {1} " +
                            ") " +
                            "SELECT HDID FROM H", hierarchyDepartmentId, statusFilter);
            }


            var hdIds = ExecuteCommand<int>(sqlCommand).ToList();

            return hdIds;
        }

        /// <summary>
        /// Get all hd Ids from a hierachy department to below
        /// </summary>
        /// <param name="hierarchyDepartmentId">The hierarchy department id.</param>
        /// <param name="maxLevel">Maximum hd level should be get. if value maxLevel >0, we will get hd to that maxLevel, otherwise, we will get all level. Min value is 1</param>
        /// <param name="departmentName">filter by departmentName</param>
        /// <returns>List of int H_D.</returns>
        public List<int> GetAllHDIdsFromAHierachyDepartmentToBelowWithDepartmentName(int hierarchyDepartmentId, bool getAllStatus = false, int? maxLevel = null, string departmentName = null)
        {
            var sqlCommand = "";
            var result = new List<int>();

            var statusFilter = "and hd.[Deleted] = 0";

            if (getAllStatus)
            {
                statusFilter = string.Empty;
            }

            if (maxLevel > 0)
            {
                sqlCommand = string.Format(";WITH H (HDID, DepartmentID, PathName, Path, RecursiveLevel) AS " +
                         "(  select hd.[HDID], hd.[DepartmentID], hd.[PathName], hd.[Path], 1 as [RecursiveLevel] " +
                             "FROM[org].[H_D] hd WHERE hd.[HDID] = {0} {1} " +
                             "UNION all " +
                             "select hd.[HDID], hd.[DepartmentID], hd.[PathName], hd.[Path], H.[RecursiveLevel] + 1 AS [RecursiveLevel] " +
                             "FROM[org].[H_D] hd JOIN H ON hd.[ParentID] = h.HDID {1} " +
                          ") " +
                          " SELECT Path FROM (SELECT Path, PathName, RecursiveLevel FROM H " +
                          "WHERE [RecursiveLevel] <={2} AND PathName like '%" + departmentName + "%')A", hierarchyDepartmentId, statusFilter, maxLevel + 1);

            }
            else
            {

                sqlCommand = string.Format(";WITH H (HDID, DepartmentID, PathName, Path) AS " +
                           "(  select hd.[HDID], hd.[DepartmentID], hd.[PathName], hd.[Path] " +
                               "FROM[org].[H_D] hd WHERE hd.[HDID] = {0} {1} " +
                               "UNION all " +
                               "select hd.[HDID], hd.[DepartmentID], hd.[PathName], hd.[Path] " +
                               "FROM[org].[H_D] hd JOIN H ON hd.[ParentID] = h.HDID {1} " +
                            ") " +
                            "SELECT Path FROM (SELECT Path, PathName FROM H Where PathName like '%" + departmentName + "%')A", hierarchyDepartmentId, statusFilter);

            }

            var hdIds = ExecuteCommand<string>(sqlCommand).ToList();
            if (hdIds != null)
            {
                foreach (var item in hdIds)
                {
                    var path = item.TrimEnd(new char[] { '\\' });
                    var itemIds = path.Split(new char[] { '\\' });
                    foreach (var hdId in itemIds)
                    {
                        if (!string.IsNullOrEmpty(hdId))
                        {
                            if (!result.Contains(Convert.ToInt32(hdId)))
                                result.Add(Convert.ToInt32(hdId));
                        }
                    }
                }
            }

            return result.OrderBy(x => x).ToList();
        }
        /// <summary>
        /// Get all department Ids from a hierachy department to the top
        /// </summary>
        /// <param name="hierarchyDepartmentId">The hierarchy department id.</param>
        /// <param name="getAllStatus">Get all status or not.</param>
        /// <returns>List of int H_D.</returns>
        public List<int> GetAllHDIdsFromAHierachyDepartmentToTheTop(int hierarchyDepartmentId, bool getAllStatus = false)
        {
            var statusFilter = "and hd.[Deleted] = 0";
            if (getAllStatus)
            {
                statusFilter = string.Empty;
            }
            var sqlCommand = string.Format(";WITH H AS " +
                            "(  select hd.[HDID], hd.[DepartmentID], hd.[ParentID] " +
                                "FROM[org].[H_D] hd WHERE hd.[HDID] = {0} {1} " +
                                "UNION all " +
                                "select hd.[HDID], hd.[DepartmentID], hd.[ParentID] " +
                                "FROM[org].[H_D] hd JOIN H ON h.[ParentID] = hd.HDID {1} " +
                             ") " +
                             "SELECT HDID FROM H", hierarchyDepartmentId, statusFilter);
            return ExecuteCommand<int>(sqlCommand).ToList();
        }

        /// <summary>
        /// Get all department Ids from a hierachy department to below
        /// </summary>
        /// <param name="hierarchyDepartmentId">The hierarchy department id.</param>
        /// <returns>List of int H_D.</returns>
        public List<int> GetAllDepartmentIdsFromAHierachyDepartmentToBelow(int hierarchyDepartmentId, bool getAllStatus = false)
        {
            var statusFilter = "and hd.[Deleted] = 0";
            if (getAllStatus)
            {
                statusFilter = string.Empty;
            }
            var sqlCommand = string.Format(";WITH H AS " +
                            "(  select hd.[HDID], hd.[DepartmentID] " +
                                "FROM[org].[H_D] hd WHERE hd.[HDID] = {0} {1} " +
                                "UNION all " +
                                "select hd.[HDID], hd.[DepartmentID] " +
                                "FROM[org].[H_D] hd JOIN H ON hd.[ParentID] = h.HDID {1} " +
                             ") " +
                             "SELECT DepartmentID FROM H", hierarchyDepartmentId, statusFilter);

            var departmentIds = ExecuteCommand<int>(sqlCommand).ToList();

            return departmentIds;
        }

        /// <summary>
        /// Get all department Ids from a hierachy department to below
        /// </summary>
        /// <param name="hierarchyDepartmentId">The hierarchy department id.</param>
        /// <returns>List of int H_D.</returns>
        public List<int> GetAllDepartmentIdsFromAHierachyDepartmentToBelow(int hierarchyId, List<int> parentDepartmentIds, bool getAllStatus = false)
        {
            if (parentDepartmentIds.IsNullOrEmpty()) return new List<int>();
            var statusFilter = "and hd.[Deleted] = 0";
            if (getAllStatus)
            {
                statusFilter = string.Empty;
            }

            var departmentFilter = parentDepartmentIds.Count == 1
                ? $"hd.[DepartmentID] = {parentDepartmentIds.First()}"
                : $"hd.[DepartmentID] in ({string.Join(',', parentDepartmentIds)}) ";

            var sqlCommand = ";WITH H AS " +
                            "(  select hd.[HDID], hd.[DepartmentID] " +
                                $"FROM[org].[H_D] hd WHERE hd.[HierarchyID] = {hierarchyId} and {departmentFilter} {statusFilter}" +

                                "UNION all " +
                                "select hd.[HDID], hd.[DepartmentID] " +
                                $"FROM[org].[H_D] hd JOIN H ON hd.[ParentID] = h.HDID {statusFilter} " +
                             ") " +
                             "SELECT distinct DepartmentID FROM H";

            var departmentIds = ExecuteCommand<int>(sqlCommand).ToList();

            return departmentIds;
        }
        /// <summary>
        /// Get all department Ids from a hierachy department to below
        /// </summary>
        /// <param name="hierarchyDepartmentId">The hierarchy department id.</param>
        /// <returns>List of int H_D.</returns>
        public async Task<List<int>> GetAllDepartmentIdsFromAHierachyDepartmentToBelowAsync(int hierarchyId, List<int> parentDepartmentIds, bool getAllStatus = false)
        {
            if (parentDepartmentIds.IsNullOrEmpty()) return new List<int>();
            var statusFilter = "and hd.[Deleted] = 0";
            if (getAllStatus)
            {
                statusFilter = string.Empty;
            }

            var departmentFilter = parentDepartmentIds.Count == 1
                ? $"hd.[DepartmentID] = {parentDepartmentIds.First()}"
                : $"hd.[DepartmentID] in ({string.Join(',', parentDepartmentIds)}) ";

            var sqlCommand = ";WITH H AS " +
                             "(  select hd.[HDID], hd.[DepartmentID] " +
                             $"FROM[org].[H_D] hd WHERE hd.[HierarchyID] = {hierarchyId} and {departmentFilter} {statusFilter}" +

                             "UNION all " +
                             "select hd.[HDID], hd.[DepartmentID] " +
                             $"FROM[org].[H_D] hd JOIN H ON hd.[ParentID] = h.HDID {statusFilter} " +
                             ") " +
                             "SELECT distinct DepartmentID FROM H";

            var departmentIds = await ExecuteCommandAsync<int>(sqlCommand);

            return departmentIds;
        }
        /// <summary>
        /// Get all department Ids from a hierachy department to the top
        /// </summary>
        /// <param name="getAllStatus">Get all status or not.</param>
        /// <returns>List of int H_D.</returns>
        public List<int> GetAllDepartmentIdsFromAHierachyDepartmentToTheTop(int hierarchyId, List<int> departmentIds, bool getAllStatus = false)
        {
            if (departmentIds.IsNullOrEmpty()) return new List<int>();
            var statusFilter = "and hd.[Deleted] = 0";
            if (getAllStatus)
            {
                statusFilter = string.Empty;
            }
            var departmentFilter = departmentIds.Count == 1
                ? $"hd.[DepartmentID] = {departmentIds.First()}"
                : $"hd.[DepartmentID] in ({string.Join(',', departmentIds)}) ";

            var sqlCommand = ";WITH H AS " +
                             "(  select hd.[HDID], hd.[DepartmentID], hd.[ParentID] " +
                             $"FROM[org].[H_D] hd WHERE hd.[HierarchyID] = {hierarchyId} and {departmentFilter} {statusFilter} " +
                             "UNION all " +
                             "select hd.[HDID], hd.[DepartmentID], hd.[ParentID] " +
                             $"FROM[org].[H_D] hd JOIN H ON h.[ParentID] = hd.HDID {statusFilter} " +
                             ") " +
                             "SELECT DepartmentID FROM H";

            return ExecuteCommand<int>(sqlCommand).ToList();
        }

        public async Task<List<int>> GetAllDepartmentIdsFromAHierachyDepartmentToTheTopAsync(int hierarchyId,
            List<int> departmentIds, bool getAllStatus = false)
        {
            if (departmentIds.IsNullOrEmpty()) return new List<int>();
            var statusFilter = "and hd.[Deleted] = 0";
            if (getAllStatus)
            {
                statusFilter = string.Empty;
            }

            var departmentFilter = departmentIds.Count == 1
                ? $"hd.[DepartmentID] = {departmentIds.First()}"
                : $"hd.[DepartmentID] in ({string.Join(',', departmentIds)}) ";

            var sqlCommand = ";WITH H AS " +
                             "(  select hd.[HDID], hd.[DepartmentID], hd.[ParentID] " +
                             $"FROM[org].[H_D] hd WHERE hd.[HierarchyID] = {hierarchyId} and {departmentFilter} {statusFilter} " +
                             "UNION all " +
                             "select hd.[HDID], hd.[DepartmentID], hd.[ParentID] " +
                             $"FROM[org].[H_D] hd JOIN H ON h.[ParentID] = hd.HDID {statusFilter} " +
                             ") " +
                             "SELECT DepartmentID FROM H";

            return await ExecuteCommandAsync<int>(sqlCommand);
        }

        /// <summary>
        /// Gets the child HDS.
        /// </summary>
        /// <param name="parentPath">The parent path.</param>
        /// <param name="includeDepartment"></param>
        /// <param name="includeInActiveStatus"></param>
        /// <param name="departmentTypeId"></param>
        /// <param name="departmentIds"></param>
        /// <returns>IList{H_D}.</returns>
        public IList<HierarchyDepartmentEntity> GetChildHds(string parentPath,
            bool includeDepartment = true,
            bool includeInActiveStatus = false,
            List<int> departmentTypeIds = null,
            List<int> departmentIds = null,
            int ownerId = 0, List<int> customerIds = null,
            List<EntityStatusEnum> departmentEntityStatuses = null,
            int? maxLevel = null,
            bool includeChildren = false,
            string departmentName = null,
            bool includeDepartmentType = false,
            List<string> jsonDynamicData = null)
        {
            int hdId;
            bool manipulateMaxLevel;
            IQueryable<HierarchyDepartmentEntity> query;
            parentPath = BuildGetChildQuery(parentPath, includeDepartment, includeInActiveStatus,
                departmentTypeIds, departmentIds, ownerId,
                customerIds, departmentEntityStatuses, maxLevel,
                includeChildren, departmentName, includeDepartmentType,
                jsonDynamicData, out hdId, out manipulateMaxLevel, out query);
            var result = query.Include(x => x.Parent).ToList();
            return BuildChildHdResult(maxLevel, hdId, manipulateMaxLevel, result);
        }
        public async Task<IList<HierarchyDepartmentEntity>> GetChildHdsAsync(string parentPath,
            bool includeDepartment = true,
            bool includeInActiveStatus = false,
            List<int> departmentTypeIds = null,
            List<int> departmentIds = null,
            int ownerId = 0, List<int> customerIds = null,
            List<EntityStatusEnum> departmentEntityStatuses = null,
            int? maxLevel = null,
            bool includeChildren = false,
            string departmentName = null,
            bool includeDepartmentType = false,
            List<string> jsonDynamicData = null)
        {
            int hdId;
            bool manipulateMaxLevel;
            IQueryable<HierarchyDepartmentEntity> query;
            parentPath = BuildGetChildQuery(parentPath, includeDepartment, includeInActiveStatus,
                departmentTypeIds, departmentIds, ownerId,
                customerIds, departmentEntityStatuses, maxLevel,
                includeChildren, departmentName, includeDepartmentType,
                jsonDynamicData, out hdId, out manipulateMaxLevel, out query);
            var result = await query.Include(x => x.Parent).ToListAsync();
            return BuildChildHdResult(maxLevel, hdId, manipulateMaxLevel, result);
        }

        private static IList<HierarchyDepartmentEntity> BuildChildHdResult(int? maxLevel, int hdId, bool manipulateMaxLevel, List<HierarchyDepartmentEntity> result)
        {
            if (manipulateMaxLevel)
            {
                var finalResult = new List<HierarchyDepartmentEntity>(result.Where(r => r.HDId == hdId));
                var level = 1;
                var parentHdIds = new List<int> { hdId };
                while (level <= maxLevel && parentHdIds.Count > 0)
                {
                    var childrenHds = result.Where(r => r.ParentId.HasValue && parentHdIds.Contains(r.ParentId.Value)).ToList();
                    finalResult.AddRange(childrenHds);
                    parentHdIds.Clear();
                    parentHdIds.AddRange(childrenHds.Select(h => h.HDId));
                    level++;
                }
                return finalResult;
            }
            else
            {
                return result;
            }
        }

        private string BuildGetChildQuery(string parentPath, bool includeDepartment, bool includeInActiveStatus, List<int> departmentTypeIds, List<int> departmentIds, int ownerId, List<int> customerIds, List<EntityStatusEnum> departmentEntityStatuses, int? maxLevel, bool includeChildren, string departmentName, bool includeDepartmentType, List<string> jsonDynamicData, out int hdId, out bool manipulateMaxLevel, out IQueryable<HierarchyDepartmentEntity> query)
        {
            parentPath = parentPath.TrimEnd(new char[] { '\\' });
            var itemIds = parentPath.Split(new char[] { '\\' });
            hdId = 0;
            if (itemIds.Any())
            {
                int.TryParse(itemIds.LastOrDefault(), out hdId);
            }
            //var query = GetAll().Where(t => t.Path.StartsWith(parentPath) && t.Path != parentPath);

            //when we need to include children H_Ds in each H_D, we need to retrieve one more than given value
            var hdIds = new List<int>();
            manipulateMaxLevel = maxLevel > 0 && includeChildren;
            var runtimeMaxLevel = manipulateMaxLevel ? (maxLevel + 1) : maxLevel;

            if (string.IsNullOrEmpty(departmentName))
            {
                hdIds = GetAllHDIdsFromAHierachyDepartmentToBelow(hdId, includeInActiveStatus, runtimeMaxLevel);
            }
            else
            {
                hdIds = GetAllHDIdsFromAHierachyDepartmentToBelowWithDepartmentName(hdId, includeInActiveStatus, runtimeMaxLevel, departmentName);
            }

            hdIds = hdIds.Distinct().ToList();
            query = GetAll().Where(t => hdIds.Contains(t.HDId) && t.ParentId != null);
            if (departmentIds != null && departmentIds.Any())
            {
                query = query.Where(t => departmentIds.Contains(t.DepartmentId));
            }
            if (departmentTypeIds != null && departmentTypeIds.Any())
            {
                query = query.Where(p => p.Department.DT_Ds.Any(o => departmentTypeIds.Contains(o.DepartmentTypeId)));
            }
            if (jsonDynamicData != null && jsonDynamicData.Any())
            {
                query = query.FilterByJsonValue(jsonDynamicData,
                    (path, comparisonOperator, value) => x => EfJsonQueryExtensions.JsonQuery(x.Department.DynamicAttributes, path).Contains(value),
                    (path, comparisonOperator, value) => x => EfJsonExtensions.JsonValue(x.Department.DynamicAttributes, path) == value,
                    (jsonValues, comparisonOperator, path) => x => jsonValues.Contains(EfJsonExtensions.JsonValue(x.Department.DynamicAttributes, path)));
            }
            if (!includeInActiveStatus)
            {
                query = query.Where(t => t.Deleted == (int)StatusEnum.Active);
            }

            query = QueryWithOwnerAndCustomer(ownerId, customerIds, query);
            query = QueryWithDepartmentEntityStatuses(query, departmentEntityStatuses);

            if (includeDepartment)
            {
                query = query.Include(d => d.Department);
            }
            if (includeChildren)
            {
                query = query.Include(q => q.H_Ds);
            }
            if (includeDepartmentType)
            {
                query = query.Include(x => x.Department).ThenInclude(x => x.DT_Ds);
            }

            return parentPath;
        }


        /// <summary>
        /// Gets the child HDS.
        /// </summary>
        /// <param name="parentPath">The parent path.</param>
        /// <param name="includeDepartment"></param>
        /// <param name="includeInActiveStatus"></param>
        /// <param name="departmentTypeIds"></param>
        /// <param name="ownerId"></param>
        /// <param name="customerId"></param>
        /// <returns>IList{H_D}.</returns>
        public IList<HierarchyDepartmentEntity> GetChildHds(string parentPath,
            List<int> departmentTypeIds,
            bool includeDepartment = true,
            bool includeInActiveStatus = false,
            int ownerId = 0,
            int customerId = 0)
        {
            List<HierarchyDepartmentEntity> result;
            var query = GetAll().Where(t => t.Path.StartsWith(parentPath) && t.Path != parentPath);
            //Filter by department type
            if (departmentTypeIds != null)
            {
                query = query.Where(p => p.Department.DT_Ds.Any(o => departmentTypeIds.Contains(o.DepartmentTypeId)));
            }
            //Include Deleted or not
            if (!includeInActiveStatus)
            {
                query = query.Where(t => t.Deleted == (int)StatusEnum.Active);
            }
            if (ownerId > 0)
            {
                query = query.Where(t => t.Department.OwnerId == ownerId);
            }
            if (customerId > 0)
            {
                query = query.Where(t => t.Department.CustomerId == customerId);
            }
            if (includeDepartment)
            {
                query = query.Include(d => d.Department);
            }
            result = query.ToList();
            return result;
        }


        /// <summary>
        /// Get children HDs by HD id.
        /// </summary>
        /// <param name="hDId">HD Id</param>
        /// <returns></returns>
        public List<HierarchyDepartmentEntity> GetChildrenHDsByHDID(int hDId, int childrenDepartmentArchetype = 0)
        {

            var hDs = GetAll().Where(p => p.ParentId == hDId && p.Deleted == 0 && (p.Department == null || p.Department.EntityStatusId == (short)EntityStatusEnum.Active));
            if (childrenDepartmentArchetype > 0)
            {
                hDs = hDs.Where(f => f.Department.ArchetypeId == childrenDepartmentArchetype);
            }
            return hDs.Include(p => p.Department).ToList();
        }

        /// <summary>
        /// Get department by department type and current Hd
        /// </summary>
        /// <param name="departmentTypeIds">The department type ids</param>
        /// <param name="hdId">The HD id</param>
        /// <returns></returns>
        public List<DepartmentEntity> GetDepartmentByDepartmentTypeAndHD(List<int> departmentTypeIds, int hdId)
        {
            var hD = GetById(hdId);
            if (hD == null)
                return null;

            var path = hD.Path;
            var chars = new[] { '\\' };
            path = path.TrimStart(chars);
            path = path.TrimEnd(chars);
            var hdIds = path.ToIntList("\\");
            var departments = new List<DepartmentEntity>();
            foreach (var hdid in hdIds)
            {
                var hDOfDepartmentType = GetAll().Include(p => p.Department)
                    .ThenInclude(j => j.DT_Ds)
                    .ThenInclude(j => j.DepartmentType)
                    .FirstOrDefault(hd => hd.Department.EntityStatusId == (short)EntityStatusEnum.Active && hd.Deleted != 1 && hdId == hd.HDId
                                              && hd.Department.DT_Ds.Any(dt => departmentTypeIds.Any(p => p == dt.DepartmentTypeId)));

                if (hDOfDepartmentType != null && hDOfDepartmentType.Department != null)
                    departments.Add(hDOfDepartmentType.Department);
            }


            return departments;
        }

        /// <summary>
        /// Get all department Ids from a hierachy department to the top
        /// </summary>
        /// <param name="hierarchyDepartmentId">The hierarchy department id.</param>
        /// <param name="getAllStatus">Get all status or not.</param>
        /// <returns>List of int H_D.</returns>
        public List<int> GetAllDepartmentIdsFromAHierachyDepartmentToTheTop(int hierarchyDepartmentId, bool getAllStatus = false)
        {
            var statusFilter = "and hd.[Deleted] = 0";
            if (getAllStatus)
            {
                statusFilter = string.Empty;
            }
            var sqlCommand = string.Format(";WITH H AS " +
                            "(  select hd.[HDID], hd.[DepartmentID], hd.[ParentID] " +
                                "FROM[org].[H_D] hd WHERE hd.[HDID] = {0} {1} " +
                                "UNION all " +
                                "select hd.[HDID], hd.[DepartmentID], hd.[ParentID] " +
                                "FROM[org].[H_D] hd JOIN H ON h.[ParentID] = hd.HDID {1} " +
                             ") " +
                             "SELECT DepartmentID FROM H", hierarchyDepartmentId, statusFilter);

            return ExecuteCommand<int>(sqlCommand).ToList();
        }
        /// <returns>List of int H_D.</returns>

        /// <summary>
        /// Get all department Ids from a hierachy department to the top
        /// </summary>
        /// <param name="hierarchyDepartmentId">The hierarchy department id.</param>
        /// <param name="getAllStatus">Get all status or not.</param>
        /// <returns>List of int H_D.</returns>
        public async Task<List<int>> GetAllDepartmentIdsFromAHierachyDepartmentToTheTopAsync(int hierarchyDepartmentId,
            bool getAllStatus = false)
        {
            var statusFilter = "and hd.[Deleted] = 0";
            if (getAllStatus)
            {
                statusFilter = string.Empty;
            }

            var sqlCommand = string.Format(";WITH H AS " +
                                           "(  select hd.[HDID], hd.[DepartmentID], hd.[ParentID] " +
                                           "FROM[org].[H_D] hd WHERE hd.[HDID] = {0} {1} " +
                                           "UNION all " +
                                           "select hd.[HDID], hd.[DepartmentID], hd.[ParentID] " +
                                           "FROM[org].[H_D] hd JOIN H ON h.[ParentID] = hd.HDID {1} " +
                                           ") " +
                                           "SELECT DepartmentID FROM H", hierarchyDepartmentId, statusFilter);

            return await ExecuteCommandAsync<int>(sqlCommand);
        }

        private List<TS> ExecuteCommand<TS>(string commandText,
            params object[] parameters)
        {

            return Execute<TS>(commandText, parameters).ToList();
        }

        private async Task<List<TS>> ExecuteCommandAsync<TS>(string commandText, params object[] parameters)
        {
            return await _dbContext.ExecSQLAsync<TS>(commandText, null, parameters);

        }



        /// <summary>
        /// Get list HD by a list department
        /// </summary>
        /// <param name="hierarchyId">The hierarchy id.</param>
        /// <param name="departmentIds">The list department id.</param>
        /// <returns>H_D.</returns>
        public List<HierarchyDepartmentEntity> GetListHierarchyDepartmentEntity(int hierarchyId, params int[] departmentIds)
        {
            return GetAllAsNoTracking()
                    .Where(p => departmentIds.Contains(p.DepartmentId) && p.HierarchyId == hierarchyId)
                    .Include(p => p.Department).Include(p => p.Parent).ToList();
        }

        /// <summary>
        /// Get list HDs by a list department
        /// </summary>
        /// <param name="hdIds"></param>
        /// <param name="includeDepartment"></param>
        /// <param name="includeParent"></param>
        /// <returns>H_D.</returns>
        public Task<List<HierarchyDepartmentEntity>> GetListHierarchyDepartmentEntityAsync(List<int> hdIds,
            bool includeDepartment, bool includeParent)
        {
            var query = GetAllAsNoTracking();

            if (includeParent)
            {
                query = query.Include(p => p.Parent); 
            }
            if (includeDepartment)
            {
                query = query.Include(p => p.Department);
            }

            if (hdIds != null)
            {
                if (hdIds.Count == 1)
                {
                    query = query.Where(p => hdIds[0] == p.HDId);
                }
                else
                {
                    query = query.Where(p => hdIds.Contains(p.HDId));
                }
            }

            return query.ToListAsync();
        }

        /// <summary>
        /// Get all department Ids from a hierarchy department to below
        /// </summary>
        /// <param name="hierarchyId"></param>
        /// <param name="departmentId">The hierarchy department id.</param>
        /// <param name="archetype"></param>
        /// <returns>List of int H_D.</returns>
        public List<int> GetAllDepartmentIdsFromAHierachyDepartmentToBelow(int hierarchyId, int departmentId, ArchetypeEnum archetype)
        {
            var sqlCommand = string.Format(";WITH H AS " +
                            "(  select hd.[HDID], hd.[DepartmentID] " +
                                "FROM[org].[H_D] hd WHERE hd.[HierarchyID] = {0} and hd.[DepartmentID] = {1} " +
                                "UNION all " +
                                "select hd.[HDID], hd.[DepartmentID] " +
                                "FROM[org].[H_D] hd JOIN H ON hd.[ParentID] = h.HDID " +
                                "JOIN [org].[Department] d ON d.[DepartmentID] = hd.[DepartmentID] and d.[ArchetypeID] = {2} " +
                             ") " +
                             "SELECT DepartmentID FROM H", hierarchyId, departmentId, (int)archetype);
            return ExecuteCommand<int>(sqlCommand).ToList();
        }

        /// <summary>
        /// Get list HD by a filter argument
        /// </summary>
        /// <returns>H_D.</returns>
        public List<HierarchyDepartmentEntity> GetHierarchyDepartmentEntities(int ownerId,
            int hierarchyId,
            List<int?> customerIds = null,
            List<int> hdIds = null,
            List<int> departmentIds = null,
            List<string> departmentExtIds = null,
            List<ArchetypeEnum> departmentArchetypes = null,
            List<EntityStatusEnum> departmentStatuses = null,
            bool includeDepartment = false,
            string orderBy = null)
        {
            List<int?> departmentEntityStatusIds = null;
            if (departmentStatuses == null || departmentStatuses.Count > 0)
            {
                departmentEntityStatusIds = new List<int?> { (int)EntityStatusEnum.Active };
            }
            else if (!departmentStatuses.Contains(EntityStatusEnum.All))//Will not filter with Status all
            {
                departmentEntityStatusIds = departmentStatuses.Select(a => (int?)a).ToList();
            }

            var departmentArchetypeIds = departmentArchetypes == null
                ? null
                : departmentArchetypes.Select(a => (int?)a).ToList();


            var query = GetAll().Where(q => q.HierarchyId == hierarchyId && q.Department.OwnerId == ownerId);
            
            query = QueryWithHdIds(hdIds, query);
            query = QueryWithDepartmentIds(departmentIds, query);
            query = QueryWithDepartmentCustomers(customerIds, query);
            query = QueryWithDepartmentArchetypes(departmentArchetypeIds, query);
            query = QueryWithDepartmentEntityStatuses(departmentEntityStatusIds, query);
            query = QueryWithDepartmentExtIds(departmentExtIds, query);

            if (includeDepartment)
            {
                query = query.Include(q => q.Department);
            }
            if (!string.IsNullOrEmpty(orderBy))
            {
                query = query.ApplyOrderBy(p => p.HDId, orderBy);
            }

            return query.ToList();
        }
        private static IQueryable<HierarchyDepartmentEntity> QueryWithDepartmentIds(List<int> departmentIds, IQueryable<HierarchyDepartmentEntity> query)
        {
            if (departmentIds.IsNotNullOrEmpty())
            {
                if (departmentIds.Count == 1)
                {
                    var departmentId = departmentIds[0];
                    query = query.Where(q => departmentId == q.DepartmentId);

                }
                else
                {
                    query = query.Where(q => departmentIds.Contains(q.DepartmentId));

                }
            }

            return query;
        }
        private static IQueryable<HierarchyDepartmentEntity> QueryWithHdIds(List<int> hdIs, IQueryable<HierarchyDepartmentEntity> query)
        {
            if (hdIs.IsNotNullOrEmpty())
            {
                if (hdIs.Count == 1)
                {
                    var hdId = hdIs[0];
                    query = query.Where(q => hdId == q.HDId);

                }
                else
                {
                    query = query.Where(q => hdIs.Contains(q.HDId));

                }
            }

            return query;
        }
        private static IQueryable<HierarchyDepartmentEntity> QueryWithDepartmentCustomers(List<int?> customerIds, IQueryable<HierarchyDepartmentEntity> query)
        {
            if (customerIds.IsNotNullOrEmpty())
            {
                if (customerIds.Count == 1)
                {
                    var customerId = customerIds[0];
                    query = query.Where(q => customerId == q.Department.CustomerId);

                }
                else
                {
                    query = query.Where(q => customerIds.Contains(q.Department.CustomerId));

                }

            }

            return query;
        }
        private static IQueryable<HierarchyDepartmentEntity> QueryWithDepartmentExtIds(List<string> departmentExtIds, IQueryable<HierarchyDepartmentEntity> query)
        {
            if (departmentExtIds.IsNotNullOrEmpty())
            {
                if (departmentExtIds.Count == 1)
                {
                    var extId = departmentExtIds[0];
                    query = query.Where(q => extId == q.Department.ExtId);

                }
                else
                {
                    query = query.Where(q => departmentExtIds.Contains(q.Department.ExtId));

                }

            }

            return query;
        }
        private static IQueryable<HierarchyDepartmentEntity> QueryWithDepartmentArchetypes(List<int?> departmentArchetypeIds, IQueryable<HierarchyDepartmentEntity> query)
        {
            if (departmentArchetypeIds.IsNotNullOrEmpty())
            {
                if (departmentArchetypeIds.Count == 1)
                {
                    var archetypeId = departmentArchetypeIds[0];
                    query = query.Where(q => archetypeId == q.Department.ArchetypeId);
                }
                else
                {
                    query = query.Where(q => departmentArchetypeIds.Contains(q.Department.ArchetypeId));

                }
            }

            return query;
        }

        private static IQueryable<HierarchyDepartmentEntity> QueryWithDepartmentEntityStatuses(List<int?> departmentEntityStatusIds, IQueryable<HierarchyDepartmentEntity> query)
        {
            if (departmentEntityStatusIds.IsNotNullOrEmpty())
            {
                if (departmentEntityStatusIds.Count == 1)
                {
                    var statusId = departmentEntityStatusIds[0];
                    query = query.Where(q => statusId == q.Department.EntityStatusId);

                }
                else
                {
                    query = query.Where(q => departmentEntityStatusIds.Contains(q.Department.EntityStatusId));

                }

            }

            return query;
        }
        private static IQueryable<HierarchyDepartmentEntity> QueryWithOwnerAndCustomer(int ownerId, List<int> customerIds, IQueryable<HierarchyDepartmentEntity> query)
        {
            if (ownerId > 0)
            {
                query = query.Where(q => q.Department.OwnerId == ownerId);
            }
            if (customerIds.IsNotNullOrEmpty())
            {
                if (customerIds.Count == 1)
                {
                    var customerId = customerIds[0];
                    query = query.Where(q => q.Department.CustomerId == null || customerId == q.Department.CustomerId);
                }
                else
                {
                    query = query.Where(q => q.Department.CustomerId == null || customerIds.Contains((int)q.Department.CustomerId));
                }
            }

            return query;
        }
        private static IQueryable<HierarchyDepartmentEntity> QueryWithDepartmentEntityStatuses(IQueryable<HierarchyDepartmentEntity> query,
            List<EntityStatusEnum> departmentEntityStatues)
        {
            //We do not use Active as default for this filter to keep backward compatible with old consumer
            if (departmentEntityStatues.IsNotNullOrEmpty() && !departmentEntityStatues.Contains(EntityStatusEnum.All))
            {
                if (departmentEntityStatues.Count == 1)
                {
                    var firsValue = (int)departmentEntityStatues[0];
                    query = query.Where(p => p.Department.EntityStatusId == firsValue);
                }
                else
                {
                    query = query.Where(p => departmentEntityStatues.Contains((EntityStatusEnum)p.Department.EntityStatusId.Value));
                }
            }
            return query;
        }

        /// <summary>
        /// Get parent HDs of HD's user.
        /// </summary>
        /// <param name="hD"></param>
        /// <returns></returns>
        public List<HierarchyDepartmentEntity> GetParentHDs(HierarchyDepartmentEntity hD, int ownerId = 0, List<int> customerIds = null, List<EntityStatusEnum> departmentEntityStatues = null, bool includeChildren = false, List<int> departmentTypeIds = null, bool includeDepartmentType = false)
        {
            IQueryable<HierarchyDepartmentEntity> query = BuildQuery(hD, ownerId, customerIds, departmentEntityStatues, includeChildren, departmentTypeIds, includeDepartmentType);
            return query.ToList();
        }
        public Task<List<HierarchyDepartmentEntity>> GetParentHDsAsync(HierarchyDepartmentEntity hD, int ownerId = 0,
            List<int> customerIds = null, List<EntityStatusEnum> departmentEntityStatues = null,
            bool includeChildren = false, List<int> departmentTypeIds = null,
            bool includeDepartmentType = false)
        {
            IQueryable<HierarchyDepartmentEntity> query = BuildQuery(hD, ownerId, customerIds, departmentEntityStatues, includeChildren, departmentTypeIds, includeDepartmentType);
            return query.ToListAsync();
        }

        private IQueryable<HierarchyDepartmentEntity> BuildQuery(HierarchyDepartmentEntity hD, int ownerId, List<int> customerIds, List<EntityStatusEnum> departmentEntityStatues, bool includeChildren, List<int> departmentTypeIds, bool includeDepartmentType)
        {
            var path = hD.Path;
            var chars = new[] { '\\' };
            path = path.TrimStart(chars);
            path = path.TrimEnd(chars);
            var hdStringIds = path.Split(chars);
            var hdIds = hdStringIds.Where(p => !p.Equals(hD.HDId.ToString(CultureInfo.InvariantCulture))).Select(int.Parse).ToList();
            var query = GetAll().Include(p => p.Department)
                .Where(p => hdIds.Contains(p.HDId) && p.Deleted != 1);
            query = QueryWithOwnerAndCustomer(ownerId, customerIds, query);
            query = QueryWithDepartmentEntityStatuses(query, departmentEntityStatues);
            if (includeChildren)
            {
                query = query.Include(q => q.H_Ds);
            }
            if (departmentTypeIds != null && departmentTypeIds.Any())
            {
                query = query.Where(p => p.Department.DT_Ds.Any(o => departmentTypeIds.Contains(o.DepartmentTypeId)));
            }
            if (includeDepartmentType)
            {
                query = query.Include(x => x.Department).ThenInclude(x => x.DT_Ds).ThenInclude(x => x.DepartmentType).ThenInclude(x => x.LT_DepartmentType);
            }

            return query;
        }

        public PaginatedList<HierarchyDepartmentEntity> GetHierarchyDepartments(string path, string departmentName,
            int pageIndex, int pageSize, string orderBy,
            List<string> jsonDynamicData = null,
            List<EntityStatusEnum> departmentEntityStatuses = null,
            List<int> departmentTypeIds = null,
            bool includeDepartmentType = false,
            bool includeParent = false)
        {
            IQueryable<HierarchyDepartmentEntity> query = BuildHirachyDepartments(path, departmentName, orderBy, jsonDynamicData, departmentEntityStatuses, departmentTypeIds, includeDepartmentType, includeParent);
            var hasMoreData = false;
            //Build paging from IQueryable
            var totalItem = 0;
            var items = query.ToPaging(pageIndex, pageSize, false, out hasMoreData, out totalItem);
            var paginatedEntities = new PaginatedList<HierarchyDepartmentEntity>(items, pageIndex, pageSize, hasMoreData) { TotalItems = totalItem };
            return paginatedEntities;
        }
        public async Task<PaginatedList<HierarchyDepartmentEntity>> GetHierarchyDepartmentsAsync(string path, string departmentName,
            int pageIndex, int pageSize, string orderBy,
            List<string> jsonDynamicData = null,
            List<EntityStatusEnum> departmentEntityStatuses = null,
            List<int> departmentTypeIds = null,
            bool includeDepartmentType = false,
            bool includeParent = false,
            List<int> departmentIds = null)
        {
            IQueryable<HierarchyDepartmentEntity> query = BuildHirachyDepartments(path, departmentName, orderBy, jsonDynamicData, departmentEntityStatuses, departmentTypeIds, includeDepartmentType, includeParent, departmentIds);
            //Build paging from IQueryable
            var paginatedResult = await query.ToPagingAsync(pageIndex, pageSize, false);
            var paginatedEntities = new PaginatedList<HierarchyDepartmentEntity>(paginatedResult.Items, pageIndex, pageSize, paginatedResult.HasMoreData) { TotalItems = paginatedResult.TotalItems };
            return paginatedEntities;
        }
        private IQueryable<HierarchyDepartmentEntity> BuildHirachyDepartments(
            string path,
            string departmentName,
            string orderBy,
            List<string> jsonDynamicData,
            List<EntityStatusEnum> departmentEntityStatuses,
            List<int> departmentTypeIds,
            bool includeDepartmentType,
            bool includeParent,
            List<int> departmentIds = null)
        {
            var query = base.GetAllAsNoTracking()
                            .Include(x => x.Department)
                            .Where(x => x.Path.StartsWith(path));

            if (!string.IsNullOrEmpty(departmentName))
            {
                query = query.Where(q => q.Department.Name.Contains(departmentName));
            }
            if (departmentTypeIds != null && departmentTypeIds.Any())
            {
                query = query.Where(p => p.Department.DT_Ds.Any(o => departmentTypeIds.Contains(o.DepartmentTypeId)));
            }

            if (departmentIds is object && departmentIds.Any())
            {
                query = query.Where(hd => departmentIds.Contains(hd.DepartmentId));
            }

            query = QueryWithDepartmentEntityStatuses(query, departmentEntityStatuses);

            if (jsonDynamicData != null && jsonDynamicData.Any())
            {
                query = query.FilterByJsonValue(jsonDynamicData,
                    (pathInFunc, comparisonOperator, value) => x => EfJsonQueryExtensions.JsonQuery(x.Department.DynamicAttributes, pathInFunc).Contains(value),
                    (pathInFunc, comparisonOperator, value) => x => EfJsonExtensions.JsonValue(x.Department.DynamicAttributes, pathInFunc) == value,
                    (jsonValues, comparisonOperator, pathInFunc) => x => jsonValues.Contains(EfJsonExtensions.JsonValue(x.Department.DynamicAttributes, pathInFunc)));
            }

            if (includeDepartmentType)
            {
                query = query.Include(x => x.Department).ThenInclude(x => x.DT_Ds);
            }

            if (includeParent)
            {
                query = query.Include(x => x.Parent);
            }

            if (!string.IsNullOrEmpty(orderBy))
                query = query.ApplyOrderBy(x => x.HDId, orderBy);
            return query;
        }

        public List<HierarchyDepartmentEntity> GetListHierarchyDepartmentEntity(int hierarchyId, List<int> hdId)
        {
            return GetAllAsNoTracking()
                   .Where(p => hdId.Contains(p.HDId) && p.HierarchyId == hierarchyId)
                   .Include(p => p.Department).Include(p => p.Parent).ToList();
        }
        public Task<List<HierarchyDepartmentEntity>> GetListHierarchyDepartmentEntityAsync(int hierarchyId, List<int> hdId)
        {
            return GetAllAsNoTracking()
                   .Where(p => hdId.Contains(p.HDId) && p.HierarchyId == hierarchyId)
                   .Include(p => p.Department).Include(p => p.Parent).ToListAsync();
        }
        public HierarchyInfo GetHierarchyInfo(int currentHdId, int departmentId, HierarchyDepartmentEntity hierarchyDepartmentEntity, bool getDepartmentPath = false)
        {
            string where;
            if (hierarchyDepartmentEntity == null)
            {
                var currentHd = GetById(currentHdId);
                var hierarchyId = currentHd.HierarchyId;
                where = $"HD.HierarchyId={hierarchyId} AND HD.DepartmentId={departmentId}";
            }
            else
            {
                where = $"HD.HdId={hierarchyDepartmentEntity.HDId}";
            }

            var hierarchyInfos = ExecuteGettingHierarchyInfos(@where, getDepartmentPath);
            return hierarchyInfos.FirstOrDefault();
        }

        public List<HierarchyInfo> GetHierarchyInfos(int currentHdId, List<int> departmentIds, bool getDepartmentPath = false, int? hierarchyId = null)
        {
            if (departmentIds.IsNullOrEmpty()) return new List<HierarchyInfo>();
            if (hierarchyId == null || hierarchyId <= 0)
            {
                var currentHd = GetById(currentHdId);
                hierarchyId = currentHd.HierarchyId;
            }

            var where = departmentIds.Count == 1
                ? $"HD.HierarchyId={hierarchyId} AND HD.DepartmentId={departmentIds[0]}"
                : $"HD.HierarchyId={hierarchyId} AND HD.DepartmentId in ({string.Join(",", departmentIds)})";

            var hierarchyInfos = ExecuteGettingHierarchyInfos(@where, getDepartmentPath);

            return hierarchyInfos;

        }
        public async Task<List<HierarchyInfo>> GetHierarchyInfosAsync(int currentHdId, List<int> departmentIds, bool getDepartmentPath = false, int? hierarchyId = null)
        {
            if (departmentIds.IsNullOrEmpty()) return new List<HierarchyInfo>();
            if (hierarchyId == null || hierarchyId <= 0)
            {
                var currentHd = GetById(currentHdId);
                hierarchyId = currentHd.HierarchyId;
            }

            var where = departmentIds.Count == 1
                ? $"HD.HierarchyId={hierarchyId} AND HD.DepartmentId={departmentIds[0]}"
                : $"HD.HierarchyId={hierarchyId} AND HD.DepartmentId in ({string.Join(",", departmentIds)})";

            var hierarchyInfos = await ExecuteGettingHierarchyInfosAsync(@where, getDepartmentPath);

            return hierarchyInfos;

        }
        public async Task<List<HierarchyInfo>> GetAllHierarchyInfoFromAHierachyDepartmentToTheTopAsync(int hierarchyId,
            List<int> departmentIds, bool getAllStatus = false, bool getDepartmentPath = false)
        {

            var sqlCommand =
                BuildGetAllHierarchyInfoFromAHierachyDepartmentToTheTopCommand(hierarchyId, departmentIds, getAllStatus,
                    getDepartmentPath);

            if (string.IsNullOrEmpty(sqlCommand)) return new List<HierarchyInfo>();

            return await ExecuteCommandAsync<HierarchyInfo>(sqlCommand);
        }

        public List<HierarchyInfo> GetAllHierarchyInfoFromAHierachyDepartmentToTheTop(int hierarchyId,
            List<int> departmentIds, bool getAllStatus = false, bool getDepartmentPath = false)
        {

            var sqlCommand =
                BuildGetAllHierarchyInfoFromAHierachyDepartmentToTheTopCommand(hierarchyId, departmentIds, getAllStatus,
                    getDepartmentPath);

            if (string.IsNullOrEmpty(sqlCommand)) return new List<HierarchyInfo>();

            return ExecuteCommand<HierarchyInfo>(sqlCommand);
        }
        /// <summary>
        /// Get all department Ids from a hierachy department to below
        /// </summary>
        /// <returns>List of int H_D.</returns>
        public List<HierarchyInfo> GetAllHierarchyInfoFromAHierachyDepartmentToBelow(int hierarchyId, List<int> parentDepartmentIds, bool getAllStatus = false, bool getDepartmentPath = false)
        {
           
            var sqlCommand = BuildGetAllHierarchyInfoFromAHierachyDepartmentToBelowCommand(hierarchyId, parentDepartmentIds, getAllStatus, getDepartmentPath);
            if(string.IsNullOrEmpty(sqlCommand)) return new List<HierarchyInfo>(); 
            
            return ExecuteCommand<HierarchyInfo>(sqlCommand);

        }
        /// <summary>
        /// Get all department Ids from a hierachy department to below
        /// </summary>
        /// <returns>List of int H_D.</returns>
        public async Task<List<HierarchyInfo>> GetAllHierarchyInfoFromAHierachyDepartmentToBelowAsync(int hierarchyId, List<int> parentDepartmentIds, bool getAllStatus = false, bool getDepartmentPath = false)
        {

            var sqlCommand = BuildGetAllHierarchyInfoFromAHierachyDepartmentToBelowCommand(hierarchyId, parentDepartmentIds, getAllStatus, getDepartmentPath);
            if (string.IsNullOrEmpty(sqlCommand)) return new List<HierarchyInfo>();

            return await ExecuteCommandAsync<HierarchyInfo>(sqlCommand);

        }
        private static string BuildGetAllHierarchyInfoFromAHierachyDepartmentToBelowCommand(int hierarchyId,
            List<int> parentDepartmentIds, bool getAllStatus, bool getDepartmentPath)
        {
            if (parentDepartmentIds.IsNullOrEmpty()) return null;
            var statusFilter = "and hd.[Deleted] = 0";
            if (getAllStatus)
            {
                statusFilter = string.Empty;
            }

            var departmentFilter = parentDepartmentIds.Count == 1
                ? $"hd.[DepartmentID] = {parentDepartmentIds.First()}"
                : $"hd.[DepartmentID] in ({string.Join(',', parentDepartmentIds)}) ";

            var selectDepartmentPath = getDepartmentPath
                ? ", [dbo].[GetPathDepartmentID](HD.HDID) DepartmentPath "
                : ", null DepartmentPath";

            var sqlCommand = $@";WITH H AS 
                             (  select hd.[HierarchyId], hd.[HDID], hd.[DepartmentID], hd.[ParentID],  HD.[Path]{selectDepartmentPath} 
                             FROM[org].[H_D] hd(NOLOCK) WHERE hd.[HierarchyID] = {hierarchyId} and {departmentFilter} {statusFilter}
                             UNION all 
                             select hd.[HierarchyId], hd.[HDID], hd.[DepartmentID] , hd.[ParentID],  HD.[Path]{selectDepartmentPath} 
                             FROM[org].[H_D] hd(NOLOCK) JOIN H ON hd.[ParentID] = h.HDID {statusFilter} 
                             )  
                             SELECT [HierarchyId], HDID, DepartmentID, [Path], ParentID ParentHdId, DepartmentPath FROM H ORDER BY [PATH]";
            return sqlCommand;
        }

        private static string BuildGetAllHierarchyInfoFromAHierachyDepartmentToTheTopCommand(int hierarchyId, List<int> departmentIds,
            bool getAllStatus, bool getDepartmentPath)
        {
            if (departmentIds.IsNullOrEmpty()) return null;
            var statusFilter = "and hd.[Deleted] = 0";
            if (getAllStatus)
            {
                statusFilter = string.Empty;
            }

            var departmentFilter = departmentIds.Count == 1
                ? $"hd.[DepartmentID] = {departmentIds.First()}"
                : $"hd.[DepartmentID] in ({string.Join(',', departmentIds)}) ";

           var selectDepartmentPath = getDepartmentPath
                ? ", [dbo].[GetPathDepartmentID](HD.HDID) DepartmentPath "
                : ", null DepartmentPath";


            var sqlCommand = $@";WITH H AS 
                             (  select hd.[HierarchyId], hd.[HDID], hd.[DepartmentID], hd.[ParentID],  HD.[Path]{selectDepartmentPath} 
                             FROM[org].[H_D] hd(NOLOCK) WHERE hd.[HierarchyID] = {hierarchyId} and {departmentFilter} {statusFilter} 
                             UNION all 
                             select hd.[HierarchyId], hd.[HDID], hd.[DepartmentID], hd.[ParentID], HD.[Path]{selectDepartmentPath}
                             FROM[org].[H_D] hd(NOLOCK) JOIN H ON h.[ParentID] = hd.HDID {statusFilter} 
                             )
                             SELECT [HierarchyId], HDID, DepartmentID, [Path], ParentID ParentHdId, DepartmentPath FROM H ORDER BY [Path]";
            return sqlCommand;
        }

        private List<HierarchyInfo> ExecuteGettingHierarchyInfos(string @where, bool getDepartmentPath)
        {

            string selectDepartmentPath =
                getDepartmentPath ? ", [dbo].[GetPathDepartmentID](HD.HDID) DepartmentPath" : ", null DepartmentPath";

            var sql = $@"SELECT hd.HierarchyId, HD.HdId, HD.DepartmentId, HD.[Path], hd.ParentID ParentHdId{selectDepartmentPath}
                        FROM org.H_D HD(NOLOCK)
                        where hd.Deleted !=1  AND {@where}";

            var hierarchyInfos = ExecuteCommand<HierarchyInfo>(sql);
            return hierarchyInfos;
        }
        private async Task<List<HierarchyInfo>> ExecuteGettingHierarchyInfosAsync(string @where, bool getDepartmentPath)
        {

            string selectDepartmentPath =
                getDepartmentPath ? ", [dbo].[GetPathDepartmentID](HD.HDID) DepartmentPath" : ", null DepartmentPath";

            var sql = $@"SELECT hd.HierarchyId, HD.HdId, HD.DepartmentId, HD.[Path], hd.ParentID ParentHdId{selectDepartmentPath}
                        FROM org.H_D HD(NOLOCK)
                        where hd.Deleted !=1  AND {@where}";

            var hierarchyInfos = await ExecuteCommandAsync<HierarchyInfo>(sql);
            return hierarchyInfos;
        }
    }
}

