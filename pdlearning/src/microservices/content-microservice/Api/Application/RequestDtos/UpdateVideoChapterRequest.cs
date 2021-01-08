using System;
using System.Collections.Generic;
using Microservice.Content.Domain.ValueObject;

namespace Microservice.Content.Application.RequestDtos
{
    public class UpdateChapterRequest
    {
        public UpdateChapterRequest()
        {
            Attachments = new List<UpdateChapterAttachmentsRequest>();
        }

        public Guid Id { get; set; }

        public Guid ObjectId { get; set; }

        public VideoSourceType SourceType { get; set; }

        public string Title { get; set; }

        public string Description { get; set; }

        public int TimeStart { get; set; }

        public int TimeEnd { get; set; }

        public List<UpdateChapterAttachmentsRequest> Attachments { get; set; }
    }

    public class UpdateVideoChapterRequest
    {
        public UpdateVideoChapterRequest()
        {
            Chapters = new List<UpdateChapterRequest>();
        }

        public VideoSourceType SourceType { get; set; } = VideoSourceType.CSL;

        public List<UpdateChapterRequest> Chapters { get; set; }

        public Guid ObjectId { get; set; }
    }
}
