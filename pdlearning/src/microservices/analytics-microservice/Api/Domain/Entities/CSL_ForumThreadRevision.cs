using System;
using Thunder.Platform.Core.Domain.Entities;

namespace Microservice.Analytics.Domain.Entities
{
    public class CSL_ForumThreadRevision : Entity<int>
    {
        public int Revision { get; set; }

        public byte IsLatest { get; set; }

        public int ForumThreadId { get; set; }

        public Guid UserId { get; set; }

        public Guid UserHistoryId { get; set; }

        public string Content { get; set; }

        public DateTime? FromDate { get; set; }

        public DateTime? ToDate { get; set; }

        public DateTime? UpdatedDate { get; set; }

        public virtual CSL_ForumThread ForumThread { get; set; }
    }
}
