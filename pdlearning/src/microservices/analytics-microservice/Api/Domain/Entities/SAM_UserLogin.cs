using System;
using Thunder.Platform.Core.Domain.Entities;

namespace Microservice.Analytics.Domain.Entities
{
    public class SAM_UserLogin : Entity<Guid>
    {
        public DateTime LoginDate { get; set; }

        public Guid? SessionId { get; set; }

        public string Type { get; set; }

        public Guid UserId { get; set; }

        public string DepartmentId { get; set; }

        public Guid? UserHistoryId { get; set; }

        public string ClientId { get; set; }

        public string SourceIp { get; set; }

        public SAM_UserHistory Sam_UserHistory { get; set; }
    }
}
