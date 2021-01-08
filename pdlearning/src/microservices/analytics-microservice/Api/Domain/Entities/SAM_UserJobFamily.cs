using System;

namespace Microservice.Analytics.Domain.Entities
{
    public partial class SAM_UserJobFamily
    {
        public Guid UserHistoryId { get; set; }

        public Guid UserId { get; set; }

        public Guid JobFamilyId { get; set; }

        public virtual MT_JobFamily JobFamily { get; set; }

        public virtual SAM_UserHistory UserHistory { get; set; }
    }
}
