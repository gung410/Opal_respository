using System;
using System.Collections.Generic;
using Microservice.Content.Domain.ValueObject;

namespace Microservice.Content.Application.RequestDtos
{
    public class CreateChapterRequest
    {
        public CreateChapterRequest()
        {
            Attachments = new List<CreateVideoChapterAttachmentsRequest>();
        }

        public Guid Id { get; set; }

        public Guid ObjectId { get; set; }

        public Guid OriginalObjectId { get; set; }

        public VideoSourceType SourceType { get; set; }

        public string Title { get; set; }

        public string Description { get; set; }

        public int TimeStart { get; set; }

        public int TimeEnd { get; set; }

        public List<CreateVideoChapterAttachmentsRequest> Attachments { get; set; }
    }

    public class CreateVideoChapterRequest
    {
        public CreateVideoChapterRequest()
        {
            Chapters = new List<CreateChapterRequest>();
        }

        public VideoSourceType SourceType { get; set; } = VideoSourceType.CSL; // [OP-13505] At this feature, ChapterController only used by CSL

        public List<CreateChapterRequest> Chapters { get; set; }

        public Guid ObjectId { get; set; }
    }
}
