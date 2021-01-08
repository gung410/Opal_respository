using System;
using Microservice.Course.Application.Queries;
using Microservice.Course.Domain.Enums;

namespace Microservice.Course.Application.RequestDtos
{
    public class PreviewAnnouncementTemplateRequest
    {
        public AnnouncementType AnnouncementType { get; set; }

        public Guid ClassRunId { get; set; }

        public string Base64Message { get; set; }

        public string UserNameTag { get; set; }

        public string CourseTitleTag { get; set; }

        public string CourseCodeTag { get; set; }

        public string CourseAdminNameTag { get; set; }

        public string CourseAdminEmailTag { get; set; }

        public string ListSessionTag { get; set; }

        public string DetailUrlTag { get; set; }

        public PreviewAnnouncementTemplateQuery ToQuery()
        {
            return new PreviewAnnouncementTemplateQuery()
            {
                AnnouncementType = AnnouncementType,
                ClassRunId = ClassRunId,
                Base64Message = Base64Message,
                UserNameTag = UserNameTag,
                CourseTitleTag = CourseTitleTag,
                CourseCodeTag = CourseCodeTag,
                CourseAdminNameTag = CourseAdminNameTag,
                CourseAdminEmailTag = CourseAdminEmailTag,
                ListSessionTag = ListSessionTag,
                DetailUrlTag = DetailUrlTag
            };
        }
    }
}
