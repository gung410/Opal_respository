using LearnerApp.Common;

namespace LearnerApp.Models.Learner
{
    public class Assignment
    {
        public string AssignmentId { get; set; }

        public string RegistrationId { get; set; }

        public string ParticipantAssignmentTrackId { get; set; }

        public ParticipantAssignmentTrackStatus Status { get; set; }

        public string UserId { get; set; }
    }
}
