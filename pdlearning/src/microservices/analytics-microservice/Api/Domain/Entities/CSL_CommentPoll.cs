using System;
using Thunder.Platform.Core.Domain.Entities;

namespace Microservice.Analytics.Domain.Entities
{
    public partial class CSL_CommentPoll : Entity<int>
    {
        public string Message { get; set; }

        public int PollId { get; set; }

        public int? ParentId { get; set; }

        public int? FirstCommentId { get; set; }

        public Guid CreatedByUserId { get; set; }

        public Guid CreatedByUserHistoryId { get; set; }

        public string CreatedByDepartmentId { get; set; }

        public bool? IsReply { get; set; }

        public DateTime? FromDate { get; set; }

        public DateTime? ToDate { get; set; }

        public virtual CSL_Poll Poll { get; set; }
    }
}