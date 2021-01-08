using System;

namespace Microservice.Analytics.Domain.Entities
{
    public partial class SAM_UserProfessionalInterestArea
    {
        public Guid UserId { get; set; }

        public Guid UserHistoryId { get; set; }

        public Guid SubjectId { get; set; }

        public virtual MT_Subject Subject { get; set; }

        public virtual SAM_UserHistory UserHistory { get; set; }
    }
}
