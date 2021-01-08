using cxOrganization.Domain.Entities;
using cxPlatform.Core;
using cxPlatform.Core.Cache;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Linq;

namespace cxOrganization.Domain.Repositories
{
    public class LanguageRepository : RepositoryBase<LanguageEntity>, ILanguageRepository
    {
        private readonly ILogger<LanguageRepository> _logger;
        private readonly IMemoryCacheProvider _memoryCacheProvider;
        private static readonly object lockobject = new object();
        private const string ALL_LANGUAGES_CACHE_KEY = "ALL_LANGUAGES_CACHE_KEY";

        /// <summary>
        /// Initializes a new instance of the <see cref="LanguageRepository"/> class.
        /// </summary>
        /// <param name="dbContext">The unit of work.</param>
        /// <param name="IMemoryCacheProvider"></param>
        public LanguageRepository(OrganizationDbContext dbContext,
            IMemoryCacheProvider memoryCacheProvider,
            ILogger<LanguageRepository> logger)
            : base(dbContext)
        {
            _logger = logger;
            _memoryCacheProvider = memoryCacheProvider;
        }

        public List<LanguageEntity> GetLanguages()
        {
            if (_memoryCacheProvider.TryGetValue(ALL_LANGUAGES_CACHE_KEY, out var languages))
            {
                return (List<LanguageEntity>)languages;
            }

            lock (lockobject)
            {
                return _memoryCacheProvider.GetOrCreate(ALL_LANGUAGES_CACHE_KEY, (cache) =>
                {
                    _logger.LogWarning("ALL_LANGUAGES_CACHE_KEY not found in cache, retriveing data from database");
                    return GetAllAsNoTracking().ToList();
                });
            }
        }
    }
}
