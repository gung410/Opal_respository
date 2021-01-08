using System.Threading.Tasks;
using Conexus.Opal.BrokenLinkChecker;
using Microservice.BrokenLink.Application.Models;
using Microservice.BrokenLink.Application.RequestDtos;
using Microservice.BrokenLink.Application.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Thunder.Platform.AspNetCore.Controllers;
using Thunder.Platform.Core.Application.Dtos;
using Thunder.Platform.Core.Context;

namespace Microservice.BrokenLink.Controllers
{
    [Route("api/broken-links")]
    public class BrokenLinkController : ApplicationApiController
    {
        private readonly IBrokenLinkChecker _brokenLinkChecker;
        private readonly DomainWhitelist _domainWhitelist;
        private readonly IBrokenLinkReporter _brokenLinkReporter;

        public BrokenLinkController(
            IBrokenLinkReporter brokenLinkReporter,
            IBrokenLinkChecker brokenLinkChecker,
            IOptions<DomainWhitelist> domainWhitelistOptions,
            IUserContext userContext) : base(userContext)
        {
            _brokenLinkChecker = brokenLinkChecker;
            _domainWhitelist = domainWhitelistOptions.Value;
            _brokenLinkReporter = brokenLinkReporter;
        }

        [HttpPost("report")]
        public Task ReportBrokenLink([FromBody] CreateBrokenLinkReportRequest request)
        {
            return _brokenLinkReporter.ReportBrokenLink(request, CurrentUserId);
        }

        [HttpGet("search")]
        public Task<PagedResultDto<BrokenLinkReportModel>> SearchBrokenLinkReport([FromQuery] SearchBrokenLinkReportRequest request)
        {
            return _brokenLinkReporter.SearchBrokenLinkReport(request);
        }

        [HttpPost("check")]
        public Task<LinkCheckStatus> CheckBrokenLink([FromBody] CheckBrokenLinkRequest request)
        {
            return _brokenLinkChecker.CheckUrlAsync(request.Url, _domainWhitelist.Domains);
        }
    }
}
