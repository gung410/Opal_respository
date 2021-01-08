using System;
using System.Net.Http;
using System.Web.Http;
using cxPlatform.Crosscutting.Caching;
using cxPlatform.ManagementAPI.Filters;
using cxPlatform.Core;
using cxPlatform.Core.EFCache;

namespace cxPlatform.ManagementAPI.Controllers
{
    /// <summary>
    /// Provides functionality to manage application
    /// </summary>
    [AdminAuthorize]
    public class ManagementController : ManagementControllerBase
    {
        /// <summary>
        /// Controller for management API
        /// </summary>
        /// <param name="databaseConfiguration"></param>
        /// <param name="cacheProvider"></param>
        /// <param name="readOnlyDataMemoryCache"></param>
        /// <param name="applicationService"></param>
        /// <param name="entityFrameworkCache"></param>
        /// <param name="unitOfWork"></param>
        public ManagementController(IDatabaseConfiguration databaseConfiguration,
            Func<string, ICacheProvider> cacheProvider,
            IEntityFrameworkCache entityFrameworkCache,
            IUnitOfWork unitOfWork)
            : base(databaseConfiguration, cacheProvider, entityFrameworkCache, unitOfWork)
        {
        }

        /// <summary>
        /// Gets application config
        /// </summary>
        /// <returns></returns>
        [Route("management/appsettings")]
        public override HttpResponseMessage GetAppsettings()
        {
            return base.GetAppsettings();
        }

        /// <summary>
        /// Get database info
        /// </summary>
        /// <returns></returns>
        [Route("management/databaseinfo")]
        public override HttpResponseMessage GetDatabaseInfo()
        {
            return base.GetDatabaseInfo();
        }

        [Route("management/cacheinfo")]
        public override HttpResponseMessage GetCacheInfo()
        {
            return base.GetCacheInfo();
        }

        /// <summary>
        /// Clear the all cache
        /// </summary>
        /// <returns></returns>
        [Route("management/clearallcache")]
        [HttpGet]
        public override HttpResponseMessage ClearAllCache()
        {
            return base.ClearAllCache();
        }

        /// <summary>
        /// Clear memory cache
        /// </summary>
        /// <returns></returns>
        [Route("management/clearallmemorycache")]
        [HttpGet]
        public override HttpResponseMessage ClearAllMemoryCache()
        {
            return base.ClearAllMemoryCache();
        }
    }
}
