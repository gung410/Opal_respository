using System.Threading.Tasks;
using Refit;

namespace LearnerApp.Services.Backend
{
    // https://cxtech.atlassian.net/wiki/spaces/MP/pages/1563230239/Malformed+and+Broken+Link+API
    public interface IBrokenLinkBackendService
    {
        [Post("/api/broken-links/report")]
        public Task ReportBrokenLink([Body] object param);
    }
}
