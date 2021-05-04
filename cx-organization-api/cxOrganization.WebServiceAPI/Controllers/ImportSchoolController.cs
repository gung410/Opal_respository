using System;
using System.Net;
using System.Net.Http;
using System.Text;
using cxOrganization.Business.JsonAnalyzer;
using cxOrganization.Domain.AdvancedWorkContext;
using cxPlatform.Core;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace cxOrganization.WebServiceAPI.Controllers
{
    [Authorize]
    public class ImportSchoolController : ApiControllerBase
    {
        private readonly ILogger _logger;
        private readonly IImportSchoolAnalyzer _importOrganizationAnalyzer;
        private readonly IAdvancedWorkContext _workContext;
        public ImportSchoolController(IImportSchoolAnalyzer importOrganizationAnalyzer, IAdvancedWorkContext workContext, ILoggerFactory loggerFactory)
        {
            _importOrganizationAnalyzer = importOrganizationAnalyzer;
            _workContext = workContext;
            _logger = loggerFactory.CreateLogger<ImportSchoolController>();
        }
        /// <summary>
        /// Get school state when compare the import file with current status in the database to find out which one is new/updated...
        /// </summary>
        /// <param name="jsonString"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("importschool/buildstates", Name = "importschool/buildstate")]
        public IActionResult GetSchoolState([FromBody]string jsonString)
        {
            try
            {
                var data = _importOrganizationAnalyzer.GetSchoolDataState(jsonString, _workContext.CurrentOwnerId, _workContext.CurrentCustomerId);
                return Ok(new StringContent(Newtonsoft.Json.JsonConvert.SerializeObject(data, converters: new Newtonsoft.Json.Converters.StringEnumConverter()),
                    Encoding.UTF8, "application/json"));
            }
            catch (Exception ex)
            {
                return CreateErrorReponseWithStackTrace(ex);
            }
        }
        /// <summary>
        /// Analyze school state
        /// </summary>
        /// <param name="jsonString"></param>
        /// <returns></returns>
        [Route("importschool/analyze", Name = "importschool/analyze")]
        [HttpPost]
        public IActionResult AnalyzeSchoolState([FromBody]string jsonString)
        {

            try
            {
                var data = _importOrganizationAnalyzer.AnalyzeSchoolState(jsonString, _workContext.CurrentOwnerId, _workContext.CurrentCustomerId);
                return Ok(new StringContent(Newtonsoft.Json.JsonConvert.SerializeObject(data, converters: new Newtonsoft.Json.Converters.StringEnumConverter()),
                    Encoding.UTF8, "application/json"));
            }
            catch (Exception ex)
            {
                return CreateErrorReponseWithStackTrace(ex);
            }
        }
        /// <summary>
        /// Execute school command after analyze
        /// </summary>
        /// <param name="jsonString"></param>
        /// <returns></returns>
        [Route("importschool/executecommands", Name = "importschool/executecommand")]
        [HttpPost]
        public IActionResult ExecuteSchoolCommand([FromBody]string jsonString)
        {
            try
            {
                var data = _importOrganizationAnalyzer.ExecuteSchoolCommand(jsonString, _workContext.CurrentOwnerId, _workContext.CurrentCustomerId);
                var st = Newtonsoft.Json.JsonConvert.SerializeObject(data, converters: new Newtonsoft.Json.Converters.StringEnumConverter());
                return Ok(new StringContent(st,
                    Encoding.UTF8, "application/json"));
            }
            catch (Exception ex)
            {
                return CreateErrorReponseWithStackTrace(ex);
            }
        }

        private IActionResult CreateErrorReponseWithStackTrace(Exception ex)
        {
            var statusCode = HttpStatusCode.InternalServerError;
            if (ex is OrganizationDomainException)
                statusCode = (ex as OrganizationDomainException).ErrorCode;
            _logger.LogError(ex.Message, ex);
            return StatusCode((int)statusCode, new
            {
                ex.Message,
                ex.StackTrace
            });
        }
    }
}
