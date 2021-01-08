using System;
using System.Collections.Generic;

namespace Microservice.Analytics.Application.Consumers.Learner.Messages.Payloads
{
    public class LearnerActivitySearchCatalogPayload
    {
        public string SearchText { get; set; }

        public List<string> SearchFields { get; set; }

        public List<string> Types { get; set; }

        public int Page { get; set; }

        public int Limit { get; set; }

        public Tags Tags { get; set; }
    }

    public class Tags
    {
        public List<Guid> PdActivityType { get; set; }

        public List<Guid> CourseLevel { get; set; }

        public List<Guid> ServiceSchemeIds { get; set; }

        public List<Guid> SubjectAreaIds { get; set; }

        public List<Guid> LearningFrameworkIds { get; set; }

        public List<Guid> LearningDimensionIds { get; set; }

        public List<Guid> LearningAreaIds { get; set; }

        public List<Guid> LearningSubAreaIds { get; set; }

        public List<Guid> CategoryIds { get; set; }

        public List<Guid> TeachingCourseStudyIds { get; set; }

        public List<Guid> TeachingLevels { get; set; }

        public List<Guid> TeachingSubjectIds { get; set; }

        public List<Guid> DevelopmentalRoleIds { get; set; }

        public List<Guid> LearningMode { get; set; }

        public List<Guid> NatureOfCourse { get; set; }
    }
}
