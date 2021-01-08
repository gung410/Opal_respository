using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Microservice.Course.Domain.Enums.CourseCriteria;
using Microservice.Course.Domain.ValueObjects;
using Thunder.Platform.Core.Domain.Auditing;
using Thunder.Platform.Core.Domain.Entities;

namespace Microservice.Course.Domain.Entities
{
    [SuppressMessage("Microsoft.Naming", "CA1724", Justification = "Toan Nguyen confirmed this.")]
    public class CourseCriteria : FullAuditedEntity, ISoftDelete
    {
        public CourseCriteriaAccountType AccountType { get; set; } = CourseCriteriaAccountType.AllLearners;

        public IEnumerable<string> Tracks { get; set; } = new List<string>();

        public IEnumerable<string> DevRoles { get; set; } = new List<string>();

        public IEnumerable<string> TeachingLevels { get; set; } = new List<string>();

        public IEnumerable<string> TeachingCourseOfStudy { get; set; } = new List<string>();

        public IEnumerable<string> JobFamily { get; set; } = new List<string>();

        public IEnumerable<string> CoCurricularActivity { get; set; } = new List<string>();

        public IEnumerable<string> SubGradeBanding { get; set; } = new List<string>();

        public IEnumerable<CourseCriteriaServiceScheme> CourseCriteriaServiceSchemes { get; set; } = new List<CourseCriteriaServiceScheme>();

        public CourseCriteriaPlaceOfWork PlaceOfWork { get; set; } = new CourseCriteriaPlaceOfWork();

        public IEnumerable<Guid> PreRequisiteCourses { get; set; } = new List<Guid>();

        public Guid CreatedBy { get; set; }

        public Guid? ChangedBy { get; set; }

        public bool IsDeleted { get; set; }
    }

    public class CourseCriteriaServiceScheme
    {
        public string ServiceSchemeId { get; set; }

        public int? MaxParticipant { get; set; }
    }

    public class CourseCriteriaPlaceOfWork
    {
        public CourseCriteriaPlaceOfWorkType Type { get; set; }

        public IEnumerable<CourseCriteriaDepartmentUnitType> DepartmentUnitTypes { get; set; }

        public IEnumerable<CourseCriteriaDepartmentLevelType> DepartmentLevelTypes { get; set; }

        public IEnumerable<CourseCriteriaSpecificDepartment> SpecificDepartments { get; set; }
    }

    public class CourseCriteriaDepartmentUnitType
    {
        public Guid DepartmentUnitTypeId { get; set; }

        public int? MaxParticipant { get; set; }
    }

    public class CourseCriteriaDepartmentLevelType
    {
        public int DepartmentLevelTypeId { get; set; }

        public int? MaxParticipant { get; set; }
    }

    public class CourseCriteriaSpecificDepartment
    {
        public int DepartmentId { get; set; }

        public int? MaxParticipant { get; set; }
    }
}
