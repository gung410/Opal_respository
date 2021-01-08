using Microservice.BrokenLink.Application.Models;
using Microservice.BrokenLink.Application.RequestDtos;
using Thunder.Platform.Core.Application.Dtos;
using Thunder.Platform.Cqrs;

namespace Microservice.BrokenLink.Application.Queries
{
    public class SearchBrokenLinkReportQuery : BaseThunderQuery<PagedResultDto<BrokenLinkReportModel>>
    {
        public SearchBrokenLinkReportRequest Request { get; set; }
    }
}
