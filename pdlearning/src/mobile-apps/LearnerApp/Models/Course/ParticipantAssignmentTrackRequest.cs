namespace LearnerApp.Models.Course
{
    public class ParticipantAssignmentTrackRequest
    {
        public string CourseId { get; set; }

        public string ClassRunId { get; set; }

        public bool ForCurrentUser { get; set; } = true;

        public bool IncludeQuizAssignmentFormAnswer { get; set; } = false;
    }
}
