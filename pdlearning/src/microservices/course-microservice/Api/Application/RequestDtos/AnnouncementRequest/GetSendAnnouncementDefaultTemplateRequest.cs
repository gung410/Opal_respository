using System;
using Microservice.Course.Application.Queries;
using Microservice.Course.Domain.Enums;

namespace Microservice.Course.Application.RequestDtos
{
    public class GetSendAnnouncementDefaultTemplateRequest
    {
        public AnnouncementType AnnouncementType { get; set; }

        public Guid? CourseId { get; set; }

        public GetSendAnnouncementDefaultTemplateQuery ToQuery()
        {
            return new GetSendAnnouncementDefaultTemplateQuery() { AnnouncementType = AnnouncementType, CourseId = CourseId };
        }
    }
}
