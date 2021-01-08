using System;
using Conexus.Opal.BrokenLinkChecker;

namespace Microservice.BrokenLink.Application.RequestDtos
{
    public class CreateBrokenLinkReportRequest
    {
        public Guid ObjectId { get; set; }

        /// <summary>
        /// Used to support multiple version feature of the object.
        /// </summary>
        public Guid? OriginalObjectId { get; set; }

        /// <summary>
        /// Used to support CAM get report for class run/course.
        /// </summary>
        public Guid? ParentId { get; set; }

        public string Url { get; set; }

        public ModuleIdentifier Module { get; set; }

        public string ObjectDetailUrl { get; set; }

        public Guid ObjectOwnerId { get; set; }

        public string ObjectTitle { get; set; }

        public string ObjectOwnerName { get; set; }

        public string Description { get; set; }

        public string ReporterName { get; set; }

        public BrokenLinkContentType ContentType { get; set; }
    }
}
