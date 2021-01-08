using System;
using System.Collections.Generic;
using Microservice.Learner.Application.Dtos;
using Thunder.Platform.Cqrs;

namespace Microservice.Learner.Application.Commands
{
    public class CreateLearnerLearningPathCommand : BaseThunderCommand
    {
        public Guid Id { get; } = Guid.NewGuid();

        public string Title { get; set; }

        public string ThumbnailUrl { get; set; }

        public List<SaveLearnerLearningPathCourseRequestDto> Courses { get; set; } = new List<SaveLearnerLearningPathCourseRequestDto>();
    }
}
