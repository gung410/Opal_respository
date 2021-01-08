using System;

namespace Microservice.Analytics.Domain.Entities
{
    public partial class CSL_UserFollowUser
    {
        public int Id { get; set; }

        public Guid UserId { get; set; }

        public Guid UserIdFollowing { get; set; }

        public string DepartmentId { get; set; }

        public byte? PendingApproval { get; set; }

        public Guid UserHistoryId { get; set; }

        public DateTime? FromDate { get; set; }

        public DateTime? ToDate { get; set; }

        public Guid UserFollowingHistoryId { get; set; }
    }
}
