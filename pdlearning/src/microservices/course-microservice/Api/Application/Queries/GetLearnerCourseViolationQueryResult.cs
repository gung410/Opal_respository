using Microservice.Course.Domain.ValueObjects;

namespace Microservice.Course.Application.Queries
{
    public class GetLearnerCourseViolationQueryResult
    {
        public CourseCriteriaLearnerViolation ViolationDetail { get; set; }

        public bool IsCourseCriteriaForClassRunActivated { get; set; }
    }
}
