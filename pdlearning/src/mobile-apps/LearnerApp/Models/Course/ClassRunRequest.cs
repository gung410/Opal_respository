namespace LearnerApp.Models.Course
{
    public class ClassRunRequest
    {
        public string CourseId { get; set; }

        public string Filter { get; set; }

        public int MaxResultCount { get; set; } = 25;

        public bool NotStarted { get; set; } = false;

        public string SearchText { get; set; } = string.Empty;

        public string SearchType { get; set; } = "Learner";

        public int SkipCount { get; set; } = 0;
    }
}
