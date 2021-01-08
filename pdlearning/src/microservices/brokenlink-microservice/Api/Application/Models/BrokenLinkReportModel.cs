using System;
using Conexus.Opal.BrokenLinkChecker;
using BrokenLinkReportEntity = Microservice.BrokenLink.Domain.Entities.BrokenLinkReport;

namespace Microservice.BrokenLink.Application.Models
{
    public class BrokenLinkReportModel
    {
        public BrokenLinkReportModel()
        {
        }

        public BrokenLinkReportModel(BrokenLinkReportEntity brokenLinkReportEntity)
        {
            Id = brokenLinkReportEntity.Id;
            Url = brokenLinkReportEntity.Url;
            Module = brokenLinkReportEntity.Module;
            Description = brokenLinkReportEntity.Description;
            ContentType = brokenLinkReportEntity.ContentType;

            ReportBy = brokenLinkReportEntity.ReportBy;
            ReporterName = brokenLinkReportEntity.ReporterName;
            IsSystemReport = brokenLinkReportEntity.IsSystemReport;

            OriginalObjectId = brokenLinkReportEntity.OriginalObjectId;
            ObjectId = brokenLinkReportEntity.ObjectId;

            ObjectTitle = brokenLinkReportEntity.ObjectTitle;
            ObjectDetailUrl = brokenLinkReportEntity.ObjectDetailUrl;
            ObjectOwnerName = brokenLinkReportEntity.ObjectOwnerName;
            ObjectOwnerId = brokenLinkReportEntity.ObjectOwnerId;

            CreatedDate = brokenLinkReportEntity.CreatedDate;
        }

        public Guid Id { get; set; }

        /// <summary>
        /// The ID of object, where object is the content contains the <see cref="Url"/>.
        /// </summary>
        public Guid ObjectId { get; set; }

        public string Url { get; set; }

        /// <summary>
        /// The owner (module) of the <see cref="Url"/>.
        /// </summary>
        public ModuleIdentifier Module { get; set; }

        /// <summary>
        /// Describe invalid reason of <see cref="Url"/>.
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// User ID. Equals <b>null</b> if the report was reported by system.
        /// </summary>
        public Guid? ReportBy { get; set; }

        /// <summary>
        /// Reporter'full name.
        /// </summary>
        public string ReporterName { get; set; }

        /// <summary>
        /// The name of object like digital content title, course name...
        /// </summary>
        public string ObjectTitle { get; set; }

        /// <summary>
        /// The owner's full name of object.
        /// </summary>
        public string ObjectOwnerName { get; set; }

        /// <summary>
        /// The deep link to the object.
        /// </summary>
        public string ObjectDetailUrl { get; set; }

        /// <summary>
        /// The owner's Id of object.
        /// </summary>
        public Guid ObjectOwnerId { get; set; }

        /// <summary>
        /// <b>True</b> if the report was reported by system schedule.
        /// </summary>
        public bool IsSystemReport { get; set; }

        /// <summary>
        /// Used to support multiple version feature of the object.
        /// </summary>
        public Guid? OriginalObjectId { get; set; }

        /// <summary>
        /// Used to support CAM get report for class run/course.
        /// </summary>
        public Guid? ParentId { get; set; }

        public DateTime CreatedDate { get; set; }

        public BrokenLinkContentType ContentType { get; set; }
    }
}
