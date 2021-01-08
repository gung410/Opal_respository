using cxPlatform.Core;

namespace cxOrganization.Business.CQRSClientServices
{
    public class RequestContext : IRequestContext
    {
        public int CurrentOwnerId { get; set; }
        public int CurrentCustomerId { get; set; }
        public string CorrelationId { get; set; }
        public string ApplicationName { get; set; }
        public int? CurrentUserId { get; set; }
        public string RequestId { get; set; }
    }
}