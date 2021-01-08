using System;

namespace Microservice.Course.Application.RequestDtos
{
    public class ToggleCourseCriteriaRequest
    {
        public Guid ClassRunId { get; set; }

        public bool CourseCriteriaActivated { get; set; }
    }
}
