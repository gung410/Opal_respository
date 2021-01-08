using System;

namespace Microservice.Course.Application.RequestDtos
{
    public class CheckExistedCourseFieldRequest
    {
        public string ExternalCode { get; set; }

        public string CourseCode { get; set; }

        public Guid? CourseId { get; set; }
    }
}
