using System;
using Thunder.Platform.Core.Domain.Entities;

namespace Microservice.Analytics.Domain.Entities
{
    public partial class CSL_WikiPageRevision : Entity<int>
    {
        public int Revision { get; set; }

        public byte IsLatest { get; set; }

        public int WikiPageId { get; set; }

        public Guid UserId { get; set; }

        public Guid UserHistoryId { get; set; }

        public string Content { get; set; }

        public DateTime? ToDate { get; set; }

        public DateTime? UpdatedDate { get; set; }

        public virtual CSL_WikiPage WikiPage { get; set; }
    }
}
