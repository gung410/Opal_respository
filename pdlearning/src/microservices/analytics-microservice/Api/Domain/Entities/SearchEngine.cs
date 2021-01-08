using System;
using System.Collections.Generic;
using Thunder.Platform.Core.Domain.Entities;

namespace Microservice.Analytics.Domain.Entities
{
    public partial class SearchEngine : Entity<Guid>
    {
        public Guid UserId { get; set; }

        public Guid UserHistoryId { get; set; }

        public string DepartmentId { get; set; }

        public string SearchText { get; set; }

        public DateTime CreatedDate { get; set; }

        public Guid? PdactivityTypeId { get; set; }

        public Guid? LearningModeId { get; set; }

        public Guid? NatureOfCourseId { get; set; }

        public Guid? CourseLevelId { get; set; }

        public virtual ICollection<SearchEngineCategory> SearchEngineCategories { get; set; }

        public virtual ICollection<SearchEngineDevelopmentRole> SearchEngineDevelopmentRoles { get; set; }

        public virtual ICollection<SearchEngineLearningArea> SearchEngineLearningAreas { get; set; }

        public virtual ICollection<SearchEngineLearningDimension> SearchEngineLearningDimensions { get; set; }

        public virtual ICollection<SearchEngineLearningFramework> SearchEngineLearningFrameworks { get; set; }

        public virtual ICollection<SearchEngineLearningSubArea> SearchEngineLearningSubAreas { get; set; }

        public virtual ICollection<SearchEngineServiceScheme> SearchEngineServiceSchemes { get; set; }

        public virtual ICollection<SearchEngineSubject> SearchEngineSubjects { get; set; }

        public virtual ICollection<SearchEngineTeachingLevel> SearchEngineTeachingLevels { get; set; }
    }
}
