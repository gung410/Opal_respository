using System;

namespace Microservice.Course.Application.RequestDtos
{
    public class ToggleCourseAutomateRequest
    {
        public Guid ClassRunId { get; set; }

        public bool CourseAutomateActivated { get; set; }
    }
}
