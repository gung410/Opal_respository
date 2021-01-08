using System;
using Thunder.Platform.Core.Domain.Entities;

namespace Microservice.Analytics.Domain.Entities
{
    public partial class CSL_LikeForumThread : Entity<int>
    {
        public int ForumThreadId { get; set; }

        public DateTime? FromDate { get; set; }

        public DateTime? ToDate { get; set; }

        public Guid CreatedByUserId { get; set; }

        public Guid CreatedByUserHistoryId { get; set; }

        public string CreatedByDepartmentId { get; set; }

        public virtual CSL_ForumThread ForumThread { get; set; }
    }
}
