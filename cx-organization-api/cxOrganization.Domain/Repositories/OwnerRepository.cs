using System.Threading;
using System.Threading.Tasks;
using cxOrganization.Domain.Entities;
using cxPlatform.Core;
using cxPlatform.Core.Cache;
using Microsoft.Extensions.Logging;

namespace cxOrganization.Domain.Repositories
{
    /// <summary>
    /// Class OwnerRepository
    /// </summary>
    public class OwnerRepository : RepositoryBase<OwnerEntity>, IOwnerRepository
    {
        private readonly ILogger<OwnerRepository> _logger;
        private readonly IMemoryCacheProvider _memoryCacheProvider;
        private static readonly object Lockobject = new object();
        private static readonly SemaphoreSlim SemaphoreSlim = new SemaphoreSlim(1);

        /// <summary>
        /// Initializes a new instance of the <see cref="OwnerRepository" /> class.
        /// </summary>
        /// <param name="dbContext">The unit of work.</param>
        public OwnerRepository(OrganizationDbContext dbContext, 
            IMemoryCacheProvider memoryCacheProvider,
            ILogger<OwnerRepository> logger)
            : base(dbContext)
        {
            _logger = logger;
            _memoryCacheProvider = memoryCacheProvider;
        }
        public override OwnerEntity GetById(params object[] Id)
        {
            var cacheKey = BuildCacheKey(Id);

            if (_memoryCacheProvider.TryGetValue(cacheKey, out var hierarchyDepartmentEntity))
            {
                return (OwnerEntity)hierarchyDepartmentEntity;
            }

            lock (Lockobject)
            {
                _logger.LogWarning($"{cacheKey} not found in cache, retriveing data from database for Owner Id {Id}");
                return _memoryCacheProvider.GetOrCreate(cacheKey, (cache) => base.GetById(Id));
            }
        }
       

        public override async Task<OwnerEntity> GetByIdAsync(params object[] Id)
        {
            var cacheKey = BuildCacheKey(Id);

            if (_memoryCacheProvider.TryGetValue(cacheKey, out var hierarchyDepartmentEntity))
            {
                return (OwnerEntity)hierarchyDepartmentEntity;
            }

            await SemaphoreSlim.WaitAsync();

            var ownerEntity = await _memoryCacheProvider.GetOrCreateAsync(cacheKey, async (cache) => await base.GetByIdAsync(Id));

            SemaphoreSlim.Release();

            return ownerEntity;
        }
        private static string BuildCacheKey(object[] Id)
        {
            var cacheKey = $"OwnerEntity#Id={string.Join(",", Id)}";
            return cacheKey;
        }
    }
}