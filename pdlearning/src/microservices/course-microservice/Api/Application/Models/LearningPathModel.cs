using System;
using System.Collections.Generic;
using System.Linq;
using Microservice.Course.Domain.Entities;
using Microservice.Course.Domain.Enums;

namespace Microservice.Course.Application.Models
{
    public class LearningPathModel
    {
        public Guid Id { get; set; }

        public string Title { get; set; }

        public string ThumbnailUrl { get; set; }

        public DateTime? CreatedDate { get; set; }

        public Guid CreatedBy { get; set; }

        public LearningPathStatus Status { get; set; }

        public List<LearningPathCourseModel> ListCourses { get; set; }

        // Metadata
        public IEnumerable<string> CourseLevelIds { get; set; }

        public IEnumerable<string> PDAreaThemeIds { get; set; }

        public IEnumerable<string> ServiceSchemeIds { get; set; }

        public IEnumerable<string> SubjectAreaIds { get; set; }

        public IEnumerable<string> LearningFrameworkIds { get; set; }

        public IEnumerable<string> LearningDimensionIds { get; set; }

        public IEnumerable<string> LearningAreaIds { get; set; }

        public IEnumerable<string> LearningSubAreaIds { get; set; }

        public IEnumerable<string> MetadataKeys { get; set; }

        public static LearningPathModel Create(LearningPath learningPath, IEnumerable<LearningPathCourse> learningPathCourses)
        {
            return new LearningPathModel
            {
                Id = learningPath.Id,
                Title = learningPath.Title,
                ThumbnailUrl = learningPath.ThumbnailUrl,
                CreatedDate = learningPath.CreatedDate,
                CreatedBy = learningPath.CreatedBy,
                Status = learningPath.Status,
                ListCourses = learningPathCourses != null ? learningPathCourses.Select(p => LearningPathCourseModel.Create(p)).OrderBy(o => o.Order).ToList() : new List<LearningPathCourseModel>(),
                CourseLevelIds = learningPath.CourseLevelIds,
                PDAreaThemeIds = learningPath.PDAreaThemeIds,
                ServiceSchemeIds = learningPath.ServiceSchemeIds,
                SubjectAreaIds = learningPath.SubjectAreaIds,
                LearningFrameworkIds = learningPath.LearningFrameworkIds,
                LearningDimensionIds = learningPath.LearningDimensionIds,
                LearningAreaIds = learningPath.LearningAreaIds,
                LearningSubAreaIds = learningPath.LearningSubAreaIds,
                MetadataKeys = learningPath.MetadataKeys
            };
        }
    }
}
