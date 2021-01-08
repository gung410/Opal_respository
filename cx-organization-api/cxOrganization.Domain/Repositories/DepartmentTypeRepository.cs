using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using cxOrganization.Adapter.Shared.Extensions;
using cxOrganization.Domain.Entities;
using cxPlatform.Client.ConexusBase;
using cxPlatform.Core;
using cxPlatform.Core.Cache;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace cxOrganization.Domain.Repositories
{
    /// <summary>
    /// Class DepartmentTypeRepository
    /// </summary>
    public class DepartmentTypeRepository : RepositoryBase<DepartmentTypeEntity>, IDepartmentTypeRepository
    {
        private static readonly object lockobject = new object();
        private readonly ILogger<DepartmentTypeRepository> _logger;
        private readonly IMemoryCacheProvider _memoryCacheProvider;
        private const string ALL_DEPARTMENT_TYPE_CACHE_KEY = "ALL_DEPARTMENT_TYPE_CACHE_KEY";
        /// <summary>
        /// Initializes a new instance of the <see cref="DepartmentTypeRepository" /> class.
        /// </summary>
        /// <param name="dbContext">The unit of work.</param>
        public DepartmentTypeRepository(OrganizationDbContext dbContext,
            IMemoryCacheProvider memoryCacheProvider,
            ILogger<DepartmentTypeRepository> logger)
            : base(dbContext)
        {
            _logger = logger;
            _memoryCacheProvider = memoryCacheProvider;
        }

        public List<DepartmentTypeEntity> GetDepartmentTypes(int ownerId,
            List<int> departmentIds = null,
            List<ArchetypeEnum> archetypeIds = null,
            List<int> departmentTypeIds = null,
            List<string> extIds = null)
        {
            var query = GetAllAsNoTracking();
            if (ownerId > 0)
            {
                query = query.Where(t => t.OwnerId == ownerId);
            }
            if (archetypeIds != null && archetypeIds.Any())
            {
                query = query.Where(t => archetypeIds.Contains((ArchetypeEnum)t.ArchetypeId));
            }
            if (departmentTypeIds != null && departmentTypeIds.Any())
            {
                query = query.Where(t => departmentTypeIds.Contains(t.DepartmentTypeId));
            }
            if (departmentIds != null && departmentIds.Any())
            {
                query = query.Where(t => t.DT_Ds.Any(x => departmentIds.Contains(x.DepartmentId)));
            }
            if (extIds != null && extIds.Any())
            {
                query = query.Where(t => extIds.Contains(t.ExtId));
            }
            return query.Include(t => t.LT_DepartmentType).ToList();
        }

        public List<DepartmentTypeEntity> GetAllDepartmentTypes(int ownerId,
            List<ArchetypeEnum> archetypeIds = null,
            List<int> departmentIds = null,
            List<int> departmentTypeIds = null,
            List<string> extIds = null,
            bool includeLocalizedData = false)
        {
            var query = GetAll();
            if (ownerId > 0)
            {
                query = query.Where(t => t.OwnerId == ownerId);
            }
            if (departmentIds != null && departmentIds.Any())
            {
                query = query.Where(t => t.DT_Ds.Any(x => departmentIds.Contains(x.DepartmentId)));
            }
            if (departmentTypeIds != null && departmentTypeIds.Any())
            {
                query = query.Where(t => departmentTypeIds.Contains(t.DepartmentTypeId));
            }
            if (archetypeIds != null && archetypeIds.Any())
            {
                query = query.Include(x => x.DT_Ds).Where(t => archetypeIds.Contains((ArchetypeEnum)t.ArchetypeId));
            }
            if (includeLocalizedData)
            {
                query = query.Include(q => q.LT_DepartmentType);
            }
            return query.ToList();
        }

        public Task<List<DepartmentTypeEntity>> GetAllDepartmentTypesAsync(int ownerId,
            List<ArchetypeEnum> archetypeIds = null,
            List<int> departmentIds = null,
            List<int> departmentTypeIds = null,
            List<string> extIds = null,
            bool includeLocalizedData = false)
        {
            var query = GetAll();
            if (ownerId > 0)
            {
                query = query.Where(t => t.OwnerId == ownerId);
            }
            if (departmentIds != null && departmentIds.Any())
            {
                query = query.Where(t => t.DT_Ds.Any(x => departmentIds.Contains(x.DepartmentId)));
            }
            if (departmentTypeIds != null && departmentTypeIds.Any())
            {
                query = query.Where(t => departmentTypeIds.Contains(t.DepartmentTypeId));
            }
            if (archetypeIds != null && archetypeIds.Any())
            {
                query = query.Include(x => x.DT_Ds).Where(t => archetypeIds.Contains((ArchetypeEnum)t.ArchetypeId));
            }
            if (includeLocalizedData)
            {
                query = query.Include(q => q.LT_DepartmentType);
            }
            return query.ToListAsync();
        }

        public DepartmentTypeEntity GetDepartmentTypeByExtId(string extId, int? archetypeId = null)
        {
            return GetAllAsNoTracking().FirstOrDefault(x => x.ExtId == extId && (!archetypeId.HasValue || x.ArchetypeId == archetypeId.Value));
        }

        public bool HasDepartmentType(int ownerId, int departmentId, int departmentTypeId)
        {
            var result = (from departmentType in GetAll()
                          from department in departmentType.DT_Ds
                          where
                              department.Department.OwnerId == ownerId && department.DepartmentId == departmentId &&
                              departmentType.DepartmentTypeId == departmentTypeId
                          select departmentType).Any();
            return result;
        }

        public List<DepartmentTypeEntity> GetAllDepartmentTypesInCache()
        {
            if (_memoryCacheProvider.TryGetValue(ALL_DEPARTMENT_TYPE_CACHE_KEY, out var departmentTypes))
            {
                return (List<DepartmentTypeEntity>)departmentTypes;
            }

            lock (lockobject)
            {
                return _memoryCacheProvider.GetOrCreate(ALL_DEPARTMENT_TYPE_CACHE_KEY, (cache) =>
                {
                    _logger.LogWarning("ALL_DEPARTMENT_TYPE_CACHE_KEY not found in cache, refresh cache value");
                    return GetAll().Include(t => t.LT_DepartmentType).ToList();
                });
            }
        }

        public Task<List<DepartmentTypeEntity>> GetAllDepartmentTypesInCacheAsync()
        {
            if (_memoryCacheProvider.TryGetValue(ALL_DEPARTMENT_TYPE_CACHE_KEY, out var departmentTypes))
            {
                return Task.FromResult(departmentTypes as List<DepartmentTypeEntity>);
            }

            lock (lockobject)
            {
                return _memoryCacheProvider.GetOrCreateAsync(ALL_DEPARTMENT_TYPE_CACHE_KEY, (cache) =>
                {
                    return GetAllAsNoTracking().Include(t => t.LT_DepartmentType).ToListAsync();
                });
            }
        }

        public List<int> GetDepartmentTypeIds(List<string> departmentTypeExtIds)
        {
            if (departmentTypeExtIds.IsNullOrEmpty()) return new List<int>();

            var departmentTypes = GetAllDepartmentTypesInCache();

            return GetDepartmentTypeIds(departmentTypes, departmentTypeExtIds);
        }

        public async Task<List<int>> GetDepartmentTypeIdsAsync(List<string> departmentTypeExtIds)
        {
            if (departmentTypeExtIds.IsNullOrEmpty()) return new List<int>();

            var departmentTypes = await GetAllDepartmentTypesInCacheAsync();

            return GetDepartmentTypeIds(departmentTypes, departmentTypeExtIds);
        }

        private List<int> GetDepartmentTypeIds(List<DepartmentTypeEntity> departmentTypes, List<string> departmentTypeExtIds)
        {
            if (departmentTypeExtIds.IsNullOrEmpty() || departmentTypes.IsNullOrEmpty()) return new List<int>();

            return departmentTypes
                .Where(t => departmentTypeExtIds.Any(departmentTypeExtId => string.Equals(departmentTypeExtId, t.ExtId, StringComparison.InvariantCultureIgnoreCase)))
                .Select(t => t.DepartmentTypeId).ToList();
        }
    }
}
