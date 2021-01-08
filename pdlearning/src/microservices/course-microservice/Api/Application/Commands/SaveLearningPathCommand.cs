using System;
using System.Collections.Generic;
using Microservice.Course.Application.RequestDtos;
using Microservice.Course.Domain.Enums;
using Thunder.Platform.Cqrs;

namespace Microservice.Course.Application.Commands
{
    public class SaveLearningPathCommand : BaseThunderCommand
    {
        public Guid Id { get; set; }

        public string Title { get; set; }

        public string ThumbnailUrl { get; set; }

        public bool IsCreateNew { get; set; }

        public LearningPathStatus Status { get; set; }

        // Metadata
        public List<string> CourseLevelIds { get; set; }

        public List<string> PDAreaThemeIds { get; set; }

        public List<string> ServiceSchemeIds { get; set; }

        public List<string> SubjectAreaIds { get; set; }

        public List<string> LearningFrameworkIds { get; set; }

        public List<string> LearningDimensionIds { get; set; }

        public List<string> LearningAreaIds { get; set; }

        public List<string> LearningSubAreaIds { get; set; }

        public List<string> MetadataKeys { get; set; }

        public List<SaveLearningPathCourseDto> ToSaveLearningPathCourses { get; set; } = new List<SaveLearningPathCourseDto>();
    }
}
