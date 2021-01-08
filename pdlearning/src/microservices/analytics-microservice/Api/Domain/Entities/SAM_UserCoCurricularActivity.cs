using System;

namespace Microservice.Analytics.Domain.Entities
{
    public partial class SAM_UserCoCurricularActivity
    {
        public Guid UserId { get; set; }

        public Guid UserHistoryId { get; set; }

        public Guid CoCurricularActivityId { get; set; }

        public virtual MT_CoCurricularActivity CoCurricularActivity { get; set; }

        public virtual SAM_UserHistory UserHistory { get; set; }
    }
}
