using System;
using Thunder.Platform.Core.Domain.Entities;

namespace Microservice.Analytics.Domain.Entities
{
    public class Learner_UserActivityLandingPage : Entity
    {
        public Guid? UserSessionId { get; set; }

        public DateTime? CreatedDate { get; set; }

        public string ActionName { get; set; }

        public Guid UserId { get; set; }

        public Guid UserHistoryId { get; set; }

        public string DepartmentId { get; set; }

        public virtual SAM_UserHistory UserHistory { get; set; }
    }
}
