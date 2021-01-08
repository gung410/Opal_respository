using System;

namespace Microservice.Analytics.Domain.Entities
{
    public partial class CSL_UserFollowPoll
    {
        public int Id { get; set; }

        public Guid UserId { get; set; }

        public int PollId { get; set; }

        public string DepartmentId { get; set; }

        public byte? PendingApproval { get; set; }

        public Guid UserHistoryId { get; set; }

        public DateTime? FromDate { get; set; }

        public DateTime? ToDate { get; set; }

        public virtual CSL_Poll Poll { get; set; }
    }
}
