using System;
using Microservice.Content.Domain.Enums;

namespace Microservice.Content.Application.Models
{
    public class SearchDigitalContentModel
    {
        public Guid Id { get; set; }

        public string Title { get; set; }

        public string Description { get; set; }

        public ContentType Type { get; set; }

        public DigitalContentStatus Status { get; set; }

        public DateTime CreatedDate { get; set; }

        public DateTime? ChangedDate { get; set; }

        public string FileExtension { get; set; }

        public string FileType { get; set; }

        public int FileDuration { get; set; }

        public Guid OwnerId { get; set; }

        public string ExternalId { get; set; }

        public DateTime? ExpiredDate { get; set; }

        // Copyright section
        public string Source { get; set; }

        public string Publisher { get; set; }

        public string Copyright { get; set; }

        public string TermsOfUse { get; set; }

        public DateTime? ArchiveDate { get; set; }

        public Guid? ArchivedBy { get; set; }

        public bool IsAllowDownload { get; set; }
    }
}
