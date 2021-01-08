using LearnerApp.ViewModels.Base;

namespace LearnerApp.Models.Course
{
    public class ParticipantAssignmentTracksRequest : BaseViewModel
    {
        public string CourseId { get; set; }

        public string ClassRunId { get; set; }

        public string RegistrationId { get; set; }

        public bool? ForCurrentUser { get; set; }

        public bool IncludeQuizAssignmentFormAnswer { get; set; } = false;
    }
}
