using cxOrganization.Domain;
using cxOrganization.Domain.Settings;
using cxPlatform.Core.Cache;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace cxPlatform.ManagementAPI.Controllers
{
    [Authorize(AuthenticationSchemes = "Basic", Policy = "AdminOnly")]
    [Route("management")]
    [ApiController]
    public class ManagementController : ControllerBase
    {
        public AppSettings AppSettings { get; set; }
        private readonly OrganizationDbContext _dbContext;
        private readonly IMemoryCacheProvider _cache;
        public ManagementController(IOptions<AppSettings> appSettings,
            IMemoryCacheProvider cache,
            OrganizationDbContext dbContext)
        {
            _dbContext = dbContext;
            AppSettings = appSettings.Value;
            _cache = cache;
        }
        [HttpGet("appsettings")]
        public IActionResult GetAppsettings()
        {
            return StatusCode(200, AppSettings);
        }
        [HttpGet("databaseinfo")]
        public IActionResult GetDatabaseInfo()
        {
            var statusJson = new
            {
                DatabaseName = _dbContext.Database.GetDbConnection().Database,
                DatabaseServer = _dbContext.Database.GetDbConnection().DataSource
            };
            return StatusCode(200, statusJson);
        }
        [HttpGet("cacheinfo")]
        public IActionResult GetCacheInfo()
        {
            return StatusCode(200, new
            {
                UseCache = true
            });
        }
        [HttpGet("clearallcache")]
        public IActionResult ClearAllCache()
        {
            try
            {
                _cache.Clear();

                return StatusCode(200, "Clear All Cache Succeeded!");
            }
            catch
            {
                return StatusCode(503, "Clear All Cache Fail!");
            }
        }
    }
}
