using System;

namespace Microservice.Course.Application.RequestDtos
{
    public class CheckCourseEndDateValidWithClassEndDateRequest
    {
        public Guid CourseId { get; set; }

        public DateTime EndDate { get; set; }
    }
}
