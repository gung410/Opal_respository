using System;
using Thunder.Platform.Core.Domain.Entities;

namespace Microservice.Analytics.Domain.Entities
{
    public partial class Learner_UserActivityResume : Entity<Guid>
    {
        public DateTime? CreatedDate { get; set; }

        public Guid? UserSessionId { get; set; }

        public bool? LoginFromMobile { get; set; }

        public Guid UserId { get; set; }

        public Guid UserHistoryId { get; set; }

        public string DepartmentId { get; set; }

        public string ClientId { get; set; }

        public virtual SAM_UserHistory UserHistory { get; set; }
    }
}
