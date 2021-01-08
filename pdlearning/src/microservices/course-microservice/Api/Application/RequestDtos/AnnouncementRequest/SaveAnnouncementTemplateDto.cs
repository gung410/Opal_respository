using System;

namespace Microservice.Course.Application.RequestDtos
{
    public class SaveAnnouncementTemplateDto
    {
        public Guid? Id { get; set; }

        public string Title { get; set; }

        public string Message { get; set; }
    }
}
