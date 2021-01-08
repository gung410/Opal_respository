using System;
using Microservice.Course.Domain.Enums;

namespace Microservice.Course.Application.RequestDtos
{
    public class CreateBrokenLinkReportRequest
    {
        public Guid ObjectId { get; set; }

        public string Url { get; set; }

        public string Description { get; set; }

        public ContentType ObjectType { get; set; }
    }
}
