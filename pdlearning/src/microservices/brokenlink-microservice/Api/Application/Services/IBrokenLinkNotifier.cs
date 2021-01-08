using System;
using System.Threading.Tasks;
using Microservice.BrokenLink.Application.Models;

namespace Microservice.BrokenLink.Application.Services
{
    /// <summary>
    /// To notify to user about the broken link.
    /// </summary>
    public interface IBrokenLinkNotifier
    {
        Task NotifyBrokenLinkFound(BrokenLinkReportModel brokenLinkReportModel);

        Task NotifyLearnerReport(BrokenLinkReportModel brokenLinkReportModel);
    }
}
