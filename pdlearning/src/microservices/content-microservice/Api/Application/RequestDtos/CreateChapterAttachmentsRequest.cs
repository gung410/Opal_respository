using System;

namespace Microservice.Content.Application.RequestDtos
{
    public class CreateChapterAttachmentsRequest
    {
        public Guid ObjectId { get; set; }

        public string FileLocation { get; set; }

        public string FileName { get; set; }

        public DateTime? CreatedDate { get; set; }

        public DateTime? ChangedDate { get; set; }
    }
}
