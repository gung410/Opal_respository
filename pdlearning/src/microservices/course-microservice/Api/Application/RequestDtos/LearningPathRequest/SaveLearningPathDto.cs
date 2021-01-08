using System;
using System.Collections.Generic;
using Microservice.Course.Application.Commands;
using Microservice.Course.Domain.Enums;

namespace Microservice.Course.Application.RequestDtos
{
    public class SaveLearningPathDto
    {
        public Guid? Id { get; set; }

        public LearningPathStatus Status { get; set; } = LearningPathStatus.Draft;

        // Basic info
        public string Title { get; set; }

        public string ThumbnailUrl { get; set; }

        public List<SaveLearningPathCourseDto> ListCourses { get; set; } = new List<SaveLearningPathCourseDto>();

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

        public SaveLearningPathCommand ToCommand()
        {
            return new SaveLearningPathCommand()
            {
                Status = Status,
                Id = Id ?? Guid.NewGuid(),
                IsCreateNew = !Id.HasValue,
                Title = Title,
                ThumbnailUrl = ThumbnailUrl,
                CourseLevelIds = CourseLevelIds,
                PDAreaThemeIds = PDAreaThemeIds,
                ServiceSchemeIds = ServiceSchemeIds,
                SubjectAreaIds = SubjectAreaIds,
                LearningFrameworkIds = LearningFrameworkIds,
                LearningDimensionIds = LearningDimensionIds,
                LearningAreaIds = LearningAreaIds,
                LearningSubAreaIds = LearningSubAreaIds,
                MetadataKeys = MetadataKeys,
                ToSaveLearningPathCourses = ListCourses,
            };
        }
    }
}
