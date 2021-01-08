using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using cxOrganization.Domain.Entities;
using cxOrganization.Domain.Extensions;
using cxPlatform.Client.ConexusBase;
using cxPlatform.Core;
using cxPlatform.Core.Cache;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace cxOrganization.Domain.Repositories
{
    /// <summary>
    /// Class UserTypeRepository
    /// </summary>
    public class UserTypeRepository : RepositoryBase<UserTypeEntity>, IUserTypeRepository
    {
        private static readonly object lockobject = new object();
        private readonly IMemoryCacheProvider _memoryCacheProvider;
        private readonly ILogger<UserTypeRepository> _logger;
        private const string ALL_USER_TYPE_CACHE_KEY = "ALL_USER_TYPE_CACHE_KEY";
        private const string ALL_USER_TYPE_LOOK_UP_CACHE_KEY = "ALL_USER_TYPE_LOOK_UP_CACHE_KEY";
        private static readonly SemaphoreSlim SemaphoreSlim = new SemaphoreSlim(1);

        /// <summary>
        /// Initializes a new instance of the <see cref="UserTypeRepository" /> class.
        /// </summary>
        /// <param name="dbContext">The unit of work.</param>
        /// <param name="userRepository">The user repository.</param>
        /// <param name="entityFrameworkCache"></param>
        public UserTypeRepository(OrganizationDbContext dbContext,
            IMemoryCacheProvider memoryCacheProvider,
            ILogger<UserTypeRepository> logger)
            : base(dbContext)
        {
            _memoryCacheProvider = memoryCacheProvider;
            _logger = logger;
        }

        public List<UserTypeEntity> GetUserTypes(int ownerId = 0,
            List<int> userIds = null,
            List<ArchetypeEnum> archetypeIds = null,
            List<int> userTypeIds = null,
            List<string> extIds = null,
            bool includeLocalizedData = false,
            List<int> parentIds = null)
        {
            var query = GetAllAsNoTracking();
            
            if (!parentIds.IsNullOrEmpty())
            {
                if (parentIds.Count == 1)
                {
                    int parentId = parentIds[0];
                    query = query.Where(t => t.ParentId == parentId);
                }
                else
                {
                    var nullableParentIds = parentIds.Select(id => (int?) id);
                    query = query.Where(t => nullableParentIds.Contains(t.ParentId));
                }
            }

            if (ownerId > 0)
            {
                query = query.Where(t => t.OwnerId == ownerId);
            }
            if (userIds != null && userIds.Any())
            {
                query = query.Where(t => t.UT_Us.Any(u => userIds.Contains(u.UserId)));
            }
            if (userTypeIds != null && userTypeIds.Any())
            {
                query = query.Where(t => userTypeIds.Contains(t.UserTypeId));
            }
            if (extIds != null && extIds.Any())
            {
                query = query.Where(t => extIds.Contains(t.ExtId));
            }
            if (archetypeIds != null && archetypeIds.Any())
            {
                query = query.Where(t => archetypeIds.Contains((ArchetypeEnum)t.ArchetypeId));
            }
            if (includeLocalizedData)
            {
                query = query.Include(q => q.LT_UserType);
            }
            return query.ToList();
        }
        public UserTypeEntity GetUserTypeByExtId(string extId, int? archetypeId = null)
        {
            return GetAll().FirstOrDefault(x => x.ExtId == extId && (!archetypeId.HasValue || x.ArchetypeId == archetypeId.Value));
        }

        public List<UserTypeEntity> GetAllUserTypesInCache()
        {
            return GetAll().Include(t => t.LT_UserType).ToList();
        }

        public async Task<List<UserTypeEntity>> GetAllUserTypesInCacheAsync()
        {
            return await GetAllAsNoTracking().Include(t => t.LT_UserType).ToListAsync();
        }

        public ILookup<string, UserTypeEntity> GetAllUserTypesLookupByExtIdInCache()
        {
            _logger.LogWarning("ALL_USER_TYPE_LOOK_UP_CACHE_KEY not found in cache, retrive data from database");
            return GetAll().Include(t => t.LT_UserType)
                .ToLookup(t => t.ExtId, StringComparer.CurrentCultureIgnoreCase);
        }
    }
}
