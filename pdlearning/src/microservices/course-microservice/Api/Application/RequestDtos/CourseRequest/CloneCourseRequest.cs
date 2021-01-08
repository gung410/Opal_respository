using System;

namespace Microservice.Course.Application.RequestDtos
{
    public class CloneCourseRequest
    {
        public Guid Id { get; set; }

        public bool FromCoursePlanning { get; set; }
    }
}
