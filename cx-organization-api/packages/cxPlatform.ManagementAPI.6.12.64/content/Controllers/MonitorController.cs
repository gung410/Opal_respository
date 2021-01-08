using System.Net.Http;
using System.Web.Http;
using cxPlatform.Core;
using cxPlatform.Core.EFCache;

namespace cxPlatform.ManagementAPI.Controllers
{
    /// <summary>
    /// Monitors state of application
    /// </summary>
    public class MonitorController : MonitorControllerBase
    {
        public MonitorController(IDbContext db, IEntityFrameworkCache entityFrameworkCache) : base(db, entityFrameworkCache)
        {
        }

        /// <summary>
        /// Get version of deployed app
        /// </summary>
        /// <returns></returns>
        [Route("monitor/version")]
        public override HttpResponseMessage GetVersion()
        {
            return base.GetVersion();
        }

        /// <summary>
        /// Get status of application
        /// </summary>
        /// <returns></returns>
        [Route("monitor/status")]
        public override HttpResponseMessage GetAll()
        {
            return base.GetAll();
        }

        /// <summary>
        /// Get status of database connection
        /// </summary>
        /// <returns></returns>
        [Route("monitor/database/status")]
        public override HttpResponseMessage GetDb()
        {
            return base.GetDb();
        }

        /// <summary>
        /// Get status on distributed cache
        /// </summary>
        /// <returns></returns>
        [Route("monitor/cache/status")]
        public override HttpResponseMessage GetCache()
        {
            return base.GetCache();
        }
    }
}
