using System;
using Thunder.Platform.Core.Domain.Entities;

namespace Microservice.Analytics.Domain.Entities
{
    public class SAM_UserOtpClaim : Entity<Guid>
    {
        public DateTime? ClaimDate { get; set; }

        public Guid? SessionId { get; set; }

        public string Type { get; set; }

        public Guid? UserId { get; set; }

        public Guid? UserHistoryId { get; set; }

        public string Departmentid { get; set; }

        public virtual SAM_Department Department { get; set; }

        public virtual SAM_UserHistory UserHistory { get; set; }
    }
}
