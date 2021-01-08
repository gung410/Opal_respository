using System;
using cxPlatform.Core;

namespace cxOrganization.Domain
{
    public class RequestContext : IRequestContext
    {

        public RequestContext(IWorkContext workContext)
        {
            RequestId = workContext.RequestId;
            CorrelationId = workContext.CorrelationId;
            CurrentOwnerId = workContext.CurrentOwnerId;
            CurrentCustomerId = workContext.CurrentCustomerId;
            UserIdCXID = workContext.UserIdCXID;
            CurrentUserId= workContext.CurrentUserId > 0 ? (int?)workContext.CurrentUserId : null;
        }
        public string ApplicationName
        {
            get
            {
                return "cxOrganization.WebServiceAPI";
            }

            set
            {
                throw new NotImplementedException();
            }
        }

        public string CorrelationId { get; set; }
        public int CurrentCustomerId { get; set; }
        public int CurrentOwnerId { get; set; }
        public int? CurrentUserId  { get; set; }
        public string RequestId { get; set; }
        public string UserIdCXID { get; set; }
      
    }
}
