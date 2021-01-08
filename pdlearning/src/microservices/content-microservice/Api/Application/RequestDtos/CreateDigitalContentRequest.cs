using System;
using System.Collections.Generic;
using Microservice.Content.Domain.Entities;
using Microservice.Content.Domain.Enums;

namespace Microservice.Content.Application.RequestDtos
{
    public class CreateDigitalContentRequest
    {
        public Guid? Id { get; set; }

        public string Title { get; set; }

        public string Description { get; set; }

        public ContentType Type { get; set; }

        public DigitalContentStatus Status { get; set; }

        public string HtmlContent { get; set; }

        public string FileName { get; set; }

        public string FileType { get; set; }

        public string FileExtension { get; set; }

        public double FileSize { get; set; }

        public string FileLocation { get; set; }

        public int FileDuration { get; set; }

        // Copyright section
        public string Source { get; set; }

        public Ownership Ownership { get; set; }

        public LicenseType LicenseType { get; set; }

        public string TermsOfUse { get; set; }

        public DateTime? StartDate { get; set; }

        public DateTime? ExpiredDate { get; set; }

        public LicenseTerritory LicenseTerritory { get; set; }

        public string Publisher { get; set; }

        public string Copyright { get; set; }

        public bool IsAllowReusable { get; set; }

        public bool IsAllowDownload { get; set; }

        public bool IsAllowModification { get; set; }

        public string AcknowledgementAndCredit { get; set; }

        public string Remarks { get; set; }

        public string AttributionUrl { get; set; }

        public List<CreateAttributionElementRequest> AttributionElements { get; set; }

        public Guid? PrimaryApprovingOfficerId { get; set; }

        public Guid? AlternativeApprovingOfficerId { get; set; }

        public DateTime? ArchiveDate { get; set; }

        // Chapters
        public List<CreateChapterRequest> Chapters { get; set; }

        public DateTime? AutoPublishDate { get; set; }

        public bool? IsAutoPublish { get; set; }
    }
}
