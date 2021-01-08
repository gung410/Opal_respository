using System;
using System.Collections.Generic;

namespace Microservice.Learner.Application.Dtos
{
    public class MigrateLearningProcessRequestDto
    {
        public List<Guid> CourseIds { get; set; }

        public int BatchSize { get; set; }
    }
}
