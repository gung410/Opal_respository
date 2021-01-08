using System;

namespace Microservice.Content.Application.RequestDtos
{
    public class CreateBrokenLinkReportRequest
    {
        public Guid ObjectId { get; set; }

        public string Url { get; set; }

        public string Description { get; set; }
    }
}
