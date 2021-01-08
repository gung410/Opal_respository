using System;
using System.Collections.Generic;

namespace Microservice.Learner.Application.Dtos
{
    public class TriggerLearningProcessRequestDto
    {
        public List<Guid> CourseIds { get; set; }

        public int BatchSize { get; set; }
    }
}
