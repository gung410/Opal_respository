using System;
using System.Collections.Generic;
using System.Linq;
using Microservice.Course.Domain.Enums;
using Microservice.Course.Domain.Enums.CourseCriteria;

#pragma warning disable SA1402 // File may only contain a single type
namespace Microservice.Course.Domain.ValueObjects
{
    public enum CourseCriteriaPlaceOfWorkType
    {
        DepartmentUnitTypes,
        DepartmentLevelTypes,
        SpecificDepartments
    }

    public class CourseCriteriaLearnerViolation : BaseValueObject
    {
        public CourseCriteriaLearnerViolation()
        {
        }

        public Guid CourseId { get; set; }

        public bool IsViolated { get; set; } = false;

        public DateTime UpdatedDate { get; set; }

        public LearnerCourseCriteria LearnerCriteria { get; set; }

        public CourseCriteriaLearnerViolationAccountType AccountType { get; set; } = new CourseCriteriaLearnerViolationAccountType();

        public IEnumerable<CourseCriteriaLearnerViolationTaggingMetadata> ServiceSchemes { get; set; } = new List<CourseCriteriaLearnerViolationTaggingMetadata>();

        public IEnumerable<CourseCriteriaLearnerViolationTaggingMetadata> Tracks { get; set; } = new List<CourseCriteriaLearnerViolationTaggingMetadata>();

        public IEnumerable<CourseCriteriaLearnerViolationTaggingMetadata> DevRoles { get; set; } = new List<CourseCriteriaLearnerViolationTaggingMetadata>();

        public IEnumerable<CourseCriteriaLearnerViolationTaggingMetadata> TeachingLevels { get; set; } = new List<CourseCriteriaLearnerViolationTaggingMetadata>();

        public IEnumerable<CourseCriteriaLearnerViolationTaggingMetadata> TeachingCourseOfStudy { get; set; } = new List<CourseCriteriaLearnerViolationTaggingMetadata>();

        public IEnumerable<CourseCriteriaLearnerViolationTaggingMetadata> JobFamily { get; set; } = new List<CourseCriteriaLearnerViolationTaggingMetadata>();

        public IEnumerable<CourseCriteriaLearnerViolationTaggingMetadata> CoCurricularActivity { get; set; } = new List<CourseCriteriaLearnerViolationTaggingMetadata>();

        public IEnumerable<CourseCriteriaLearnerViolationTaggingMetadata> SubGradeBanding { get; set; } = new List<CourseCriteriaLearnerViolationTaggingMetadata>();

        public CourseCriteriaLearnerViolationPlaceOfWork PlaceOfWork { get; set; } = new CourseCriteriaLearnerViolationPlaceOfWork();

        public IEnumerable<CourseCriteriaLearnerViolationPreRequisiteCourse> PreRequisiteCourses { get; set; } = new List<CourseCriteriaLearnerViolationPreRequisiteCourse>();

        public bool CheckIsViolated()
        {
            return AccountType.ViolationType != CourseCriteriaLearnerViolationType.NotViolate
                || ServiceSchemes.Any(p => p.ViolationType != CourseCriteriaLearnerViolationType.NotViolate)
                || Tracks.Any(p => p.ViolationType != CourseCriteriaLearnerViolationType.NotViolate)
                || DevRoles.Any(p => p.ViolationType != CourseCriteriaLearnerViolationType.NotViolate)
                || TeachingLevels.Any(p => p.ViolationType != CourseCriteriaLearnerViolationType.NotViolate)
                || TeachingCourseOfStudy.Any(p => p.ViolationType != CourseCriteriaLearnerViolationType.NotViolate)
                || JobFamily.Any(p => p.ViolationType != CourseCriteriaLearnerViolationType.NotViolate)
                || CoCurricularActivity.Any(p => p.ViolationType != CourseCriteriaLearnerViolationType.NotViolate)
                || SubGradeBanding.Any(p => p.ViolationType != CourseCriteriaLearnerViolationType.NotViolate)
                || PreRequisiteCourses.Any(p => p.ViolationType != CourseCriteriaLearnerViolationType.NotViolate)
                || (PlaceOfWork.Type == CourseCriteriaPlaceOfWorkType.DepartmentLevelTypes && PlaceOfWork.DepartmentLevelTypes.Any(p => p.ViolationType != CourseCriteriaLearnerViolationType.NotViolate))
                || (PlaceOfWork.Type == CourseCriteriaPlaceOfWorkType.DepartmentUnitTypes && PlaceOfWork.DepartmentUnitTypes.Any(p => p.ViolationType != CourseCriteriaLearnerViolationType.NotViolate))
                || (PlaceOfWork.Type == CourseCriteriaPlaceOfWorkType.SpecificDepartments && PlaceOfWork.SpecificDepartments.Any(p => p.ViolationType != CourseCriteriaLearnerViolationType.NotViolate));
        }

        public bool CheckIsPreRequisiteCoursesViolated()
        {
            return PreRequisiteCourses.Any(p => p.ViolationType != CourseCriteriaLearnerViolationType.NotViolate);
        }
    }

    public class LearnerCourseCriteria
    {
        public Guid UserId { get; set; }

        public CourseUserAccountType AccountType { get; set; }

        public IEnumerable<string> ServiceSchemes { get; set; }

        public IEnumerable<string> Tracks { get; set; }

        public IEnumerable<string> DevRoles { get; set; }

        public IEnumerable<string> TeachingLevels { get; set; }

        public IEnumerable<string> TeachingCourseOfStudy { get; set; }

        public IEnumerable<string> JobFamily { get; set; }

        public IEnumerable<string> CoCurricularActivity { get; set; }

        public IEnumerable<string> SubGradeBanding { get; set; }

        public LearnerCourseCriteriaDepartment Department { get; set; }
    }

    public class LearnerCourseCriteriaDepartment
    {
        public int DepartmentId { get; set; }

        public Guid DepartmentUnitTypeId { get; set; }

        public int DepartmentLevelTypeId { get; set; }
    }

    public class CourseCriteriaLearnerViolationAccountType
    {
        public CourseCriteriaAccountType AccountType { get; set; }

        public CourseCriteriaLearnerViolationType ViolationType { get; set; } = CourseCriteriaLearnerViolationType.NotViolate;
    }

    public class CourseCriteriaLearnerViolationTaggingMetadata
    {
        public string TagId { get; set; }

        public int? MaxParticipant { get; set; }

        public CourseCriteriaLearnerViolationType ViolationType { get; set; } = CourseCriteriaLearnerViolationType.NotViolate;
    }

    public class CourseCriteriaLearnerViolationPlaceOfWork
    {
        public CourseCriteriaPlaceOfWorkType Type { get; set; } = CourseCriteriaPlaceOfWorkType.DepartmentUnitTypes;

        public IEnumerable<CourseCriteriaLearnerViolationDepartmentUnitType> DepartmentUnitTypes { get; set; } = new List<CourseCriteriaLearnerViolationDepartmentUnitType>();

        public IEnumerable<CourseCriteriaLearnerViolationDepartmentLevelType> DepartmentLevelTypes { get; set; } = new List<CourseCriteriaLearnerViolationDepartmentLevelType>();

        public IEnumerable<CourseCriteriaLearnerViolationSpecificDepartment> SpecificDepartments { get; set; } = new List<CourseCriteriaLearnerViolationSpecificDepartment>();
    }

    public class CourseCriteriaLearnerViolationDepartmentUnitType
    {
        public Guid DepartmentUnitTypeId { get; set; }

        public int? MaxParticipant { get; set; }

        public CourseCriteriaLearnerViolationType ViolationType { get; set; } = CourseCriteriaLearnerViolationType.NotViolate;
    }

    public class CourseCriteriaLearnerViolationDepartmentLevelType
    {
        public int DepartmentLevelTypeId { get; set; }

        public int? MaxParticipant { get; set; }

        public CourseCriteriaLearnerViolationType ViolationType { get; set; } = CourseCriteriaLearnerViolationType.NotViolate;
    }

    public class CourseCriteriaLearnerViolationSpecificDepartment
    {
        public int DepartmentId { get; set; }

        public int? MaxParticipant { get; set; }

        public CourseCriteriaLearnerViolationType ViolationType { get; set; } = CourseCriteriaLearnerViolationType.NotViolate;
    }

    public class CourseCriteriaLearnerViolationPreRequisiteCourse
    {
        public Guid CourseId { get; set; }

        public CourseCriteriaLearnerViolationType ViolationType { get; set; } = CourseCriteriaLearnerViolationType.NotViolate;
    }
}
#pragma warning restore SA1402 // File may only contain a single type
