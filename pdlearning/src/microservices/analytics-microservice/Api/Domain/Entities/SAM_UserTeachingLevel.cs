using System;

namespace Microservice.Analytics.Domain.Entities
{
    public partial class SAM_UserTeachingLevel
    {
        public Guid UserHistoryId { get; set; }

        public Guid UserId { get; set; }

        public Guid TeachingLevelId { get; set; }

        public virtual MT_TeachingLevel TeachingLevel { get; set; }

        public virtual SAM_UserHistory UserHistory { get; set; }
    }
}
