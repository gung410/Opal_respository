using System;

namespace Microservice.Analytics.Domain.Entities
{
    public partial class SAM_UserTeachingSubject
    {
        public Guid UserId { get; set; }

        public Guid UserHistoryId { get; set; }

        public Guid TeachingSubjectId { get; set; }

        public virtual MT_TeachingSubject TeachingSubject { get; set; }

        public virtual SAM_UserHistory UserHistory { get; set; }
    }
}
