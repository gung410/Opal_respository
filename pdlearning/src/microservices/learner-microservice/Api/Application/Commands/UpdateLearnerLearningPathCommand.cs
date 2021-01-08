using System;
using System.Collections.Generic;
using Microservice.Learner.Application.Dtos;
using Thunder.Platform.Cqrs;

namespace Microservice.Learner.Application.Commands
{
    public class UpdateLearnerLearningPathCommand : BaseThunderCommand
    {
        public UpdateLearnerLearningPathCommand(Guid id)
        {
            Id = id;
        }

        public Guid Id { get; }

        public string Title { get; set; }

        public string ThumbnailUrl { get; set; }

        public List<SaveLearnerLearningPathCourseRequestDto> Courses { get; set; } = new List<SaveLearnerLearningPathCourseRequestDto>();
    }
}
