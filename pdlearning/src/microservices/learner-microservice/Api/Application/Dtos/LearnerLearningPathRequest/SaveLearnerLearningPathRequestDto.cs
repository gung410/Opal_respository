using System;
using System.Collections.Generic;

namespace Microservice.Learner.Application.Dtos
{
    public class SaveLearnerLearningPathRequestDto
    {
        public Guid? Id { get; set; }

        public string Title { get; set; }

        public string ThumbnailUrl { get; set; }

        public List<SaveLearnerLearningPathCourseRequestDto> Courses { get; set; } = new List<SaveLearnerLearningPathCourseRequestDto>();
    }
}
