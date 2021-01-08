using System;
using System.Collections.Generic;
using System.Web;
using Microservice.Content.Application.Services;
using Microservice.Content.Domain.Entities;
using Microservice.Content.Domain.Enums;

namespace Microservice.Content.Application.Models
{
    public class DigitalContentModel
    {
        public DigitalContentModel()
        {
        }

        public DigitalContentModel(DigitalContent digitalContent)
        {
            Id = digitalContent.Id;
            Title = digitalContent.Title;
            Status = digitalContent.Status;
            Type = digitalContent.Type;
            Description = HttpUtility.HtmlDecode(digitalContent.Description);
            CreatedDate = digitalContent.CreatedDate;
            ChangedDate = digitalContent.ChangedDate;
            OwnerId = digitalContent.OwnerId;
            ExternalId = digitalContent.ExternalId;
            ExpiredDate = digitalContent.ExpiredDate;
            Source = digitalContent.Source;
            Publisher = digitalContent.Publisher;
            Copyright = digitalContent.Copyright;
            TermsOfUse = digitalContent.TermsOfUse;
            LicenseType = digitalContent.LicenseType;
            StartDate = digitalContent.StartDate;
            AcknowledgementAndCredit = digitalContent.AcknowledgementAndCredit;
            IsAllowDownload = digitalContent.IsAllowDownload;
            IsArchived = digitalContent.IsArchived;
            IsAllowModification = digitalContent.IsAllowModification;
            IsAllowReusable = digitalContent.IsAllowReusable;
            LicenseTerritory = digitalContent.LicenseTerritory;
            Remarks = digitalContent.Remarks;
            Ownership = digitalContent.Ownership;
            OriginalObjectId = digitalContent.OriginalObjectId;
            PrimaryApprovingOfficerId = digitalContent.PrimaryApprovingOfficerId;
            AlternativeApprovingOfficerId = digitalContent.AlternativeApprovingOfficerId;
            AttributionElements = new System.Collections.Generic.List<AttributionElementModel>();
            ArchiveDate = digitalContent.ArchiveDate;
            ArchivedBy = digitalContent.ArchivedBy;
            Chapters = new List<ChapterModel>();
            AutoPublishDate = digitalContent.AutoPublishDate;
            IsAutoPublish = digitalContent.IsAutoPublish;

            if (digitalContent is LearningContent learningContent)
            {
                MapLearningAttributes(learningContent);
            }

            if (digitalContent is UploadedContent uploadedContent)
            {
                MapUploadedAttributes(uploadedContent);
            }
        }

        public Guid Id { get; set; }

        public Guid OriginalObjectId { get; set; }

        public string Title { get; set; }

        public string Description { get; set; }

        public ContentType Type { get; set; }

        public DigitalContentStatus Status { get; set; }

        public string FileType { get; set; }

        public string FileExtension { get; set; }

        public string FileName { get; set; }

        public string FileLocation { get; set; }

        public double FileSize { get; set; }

        public int FileDuration { get; set; }

        public string HtmlContent { get; set; }

        public DateTime CreatedDate { get; set; }

        public DateTime? ChangedDate { get; set; }

        public Guid OwnerId { get; set; }

        public string ExternalId { get; set; }

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

        public int ViewsCount { get; private set; }

        public int DownloadsCount { get; private set; }

        public string AttributionUrl { get; set; }

        public List<AttributionElementModel> AttributionElements { get; set; }

        public Guid? PrimaryApprovingOfficerId { get; set; }

        public Guid? AlternativeApprovingOfficerId { get; set; }

        public DateTime? ArchiveDate { get; set; }

        public Guid? ArchivedBy { get; set; }

        public bool? IsArchived { get; set; }

        public List<ChapterModel> Chapters { get; set; }

        public bool? IsAutoPublish { get; set; }

        public DateTime? AutoPublishDate { get; set; }

        public DigitalContentModel WithViewsCount(int viewsCount)
        {
            ViewsCount = viewsCount;
            return this;
        }

        public DigitalContentModel WithDownloadsCount(int downloadsCount)
        {
            DownloadsCount = downloadsCount;
            return this;
        }

        public DigitalContentModel MapLearningAttributes(LearningContent learningContent)
        {
            HtmlContent = HttpUtility.HtmlDecode(learningContent.HtmlContent);
            return this;
        }

        public DigitalContentModel MapUploadedAttributes(UploadedContent uploadedContent)
        {
            FileExtension = uploadedContent.FileExtension;
            FileSize = uploadedContent.FileSize;
            FileName = uploadedContent.FileName;
            FileType = uploadedContent.FileType;
            FileLocation = uploadedContent.FileLocation;
            FileDuration = uploadedContent.FileDuration;
            return this;
        }
    }
}
