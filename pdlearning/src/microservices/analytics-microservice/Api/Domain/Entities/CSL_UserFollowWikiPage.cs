using System;

namespace Microservice.Analytics.Domain.Entities
{
    public partial class CSL_UserFollowWikiPage
    {
        public int Id { get; set; }

        public Guid UserId { get; set; }

        public Guid UserHistoryId { get; set; }

        public int WikiPageId { get; set; }

        public byte? PendingApproval { get; set; }

        public DateTime? FromDate { get; set; }

        public DateTime? ToDate { get; set; }

        public virtual CSL_WikiPage WikiPage { get; set; }
    }
}
