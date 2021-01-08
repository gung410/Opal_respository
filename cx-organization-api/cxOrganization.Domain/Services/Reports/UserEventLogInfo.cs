using System;
using cxOrganization.Domain.Dtos.Users;

namespace cxOrganization.Domain.Services.Reports
{
    public class UserEventLogInfo
    {
        public string EventId { get; set; }
        public UserEventType Type { get; set; }
        public DateTime Created { get; set; }
        public UserGenericDto CreatedByUser { get; set; } 
        public EventLogLevel Level { get; set; }
        public dynamic EventInfo { get; set; }
        public string RoutingAction { get; set; }
        public string SourceIp { get; set; }
    }
}