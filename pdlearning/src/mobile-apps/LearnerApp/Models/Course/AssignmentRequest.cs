namespace LearnerApp.Models.Course
{
    public class AssignmentRequest
    {
        public string[] Ids { get; set; }

        public bool IncludeQuizForm { get; set; }
    }
}
