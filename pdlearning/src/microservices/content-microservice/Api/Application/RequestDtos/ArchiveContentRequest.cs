using System;

namespace Microservice.Content.Application.RequestDtos
{
    public class ArchiveContentRequest
    {
        public Guid ObjectId { get; set; }

        public Guid? ArchiveByUserId { get; set; }
    }
}
