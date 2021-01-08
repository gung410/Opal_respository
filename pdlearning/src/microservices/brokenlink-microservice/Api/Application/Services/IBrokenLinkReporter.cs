using System;
using System.Threading.Tasks;
using Microservice.BrokenLink.Application.Models;
using Microservice.BrokenLink.Application.RequestDtos;
using Thunder.Platform.Core.Application.Dtos;

namespace Microservice.BrokenLink.Application.Services
{
    /// <summary>
    /// To report/check broken link(s).
    /// </summary>
    public interface IBrokenLinkReporter
    {
        Task ReportBrokenLink(CreateBrokenLinkReportRequest request, Guid userId);

        Task<PagedResultDto<BrokenLinkReportModel>> SearchBrokenLinkReport(SearchBrokenLinkReportRequest dto);
    }
}
