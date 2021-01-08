using LearnerApp.Common;

namespace LearnerApp.Models.Course
{
    public class Registration
    {
        public string CourseId { get; set; }

        public string ClassRunId { get; set; }

        public RegistrationStatus Status { get; set; }
    }
}
