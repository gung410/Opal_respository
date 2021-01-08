using System;
using System.Collections.Generic;
using Conexus.Opal.BrokenLinkChecker;
using JetBrains.Annotations;
using Thunder.Platform.Core.Application.Dtos;

namespace Microservice.BrokenLink.Application.RequestDtos
{
    public class SearchBrokenLinkReportRequest
    {
        public Guid? ObjectId { get; set; }

        public Guid? UserId { get; set; }

        public Guid? OriginalObjectId { get; set; }

        public ModuleIdentifier? Module { get; set; }

        public BrokenLinkContentType? ContentType { get; set; }

        [CanBeNull]
        public List<Guid> ParentIds { get; set; }

        public PagedResultRequestDto PagedInfo { get; set; }
    }
}
