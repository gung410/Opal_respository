using System;
using Microservice.Course.Application.Models;
using Microservice.Course.Domain.Enums;
using Thunder.Platform.Cqrs;

namespace Microservice.Course.Application.Queries
{
    public class GetSendAnnouncementDefaultTemplateQuery : BaseThunderQuery<SendAnnouncementEmailTemplateModel>
    {
        public AnnouncementType AnnouncementType { get; set; }

        public Guid? CourseId { get; set; }
    }
}
