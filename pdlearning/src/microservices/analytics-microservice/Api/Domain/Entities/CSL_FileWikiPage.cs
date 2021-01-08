using System;
using Thunder.Platform.Core.Domain.Entities;

namespace Microservice.Analytics.Domain.Entities
{
    public partial class CSL_FileWikiPage : Entity<int>
    {
        public string Guid { get; set; }

        public int? WikiPageId { get; set; }

        public string FileName { get; set; }

        public string Title { get; set; }

        public string MimeType { get; set; }

        public string Size { get; set; }

        public DateTime? FromDate { get; set; }

        public DateTime? ToDate { get; set; }

        public Guid CreatedByUserId { get; set; }

        public Guid CreatedByUserHistoryId { get; set; }

        public string CreatedByDepartmentId { get; set; }
    }
}
