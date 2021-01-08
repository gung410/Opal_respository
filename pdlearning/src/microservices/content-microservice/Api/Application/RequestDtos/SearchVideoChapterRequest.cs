using System;
using System.Collections.Generic;
using Microservice.Content.Domain.ValueObject;

namespace Microservice.Content.Application.RequestDtos
{
    public class SearchVideoChapterRequest
    {
        public Guid ObjectId { get; set; } = Guid.Empty; // ObjectId also use like VideoId for CSL

        public VideoSourceType SourceType { get; set; } = VideoSourceType.CSL; // [OP-13505] At this feature, ChapterController only used by CSL
    }
}
