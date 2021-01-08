using System;
using LearnerApp.ViewModels.Base;

namespace LearnerApp.Models.Course
{
    public class ParticipantAssignmentTrack : BaseViewModel
    {
        public string Id { get; set; }

        public string RegistrationId { get; set; }

        public string AssignmentId { get; set; }

        public string UserId { get; set; }

        public DateTime AssignedDate { get; set; }

        public DateTime? SubmittedDate { get; set; }

        public DateTime StartDate { get; set; }

        public DateTime EndDate { get; set; }

        public string CreatedBy { get; set; }

        public string ChangedBy { get; set; }

        public QuizAnswer QuizAnswer { get; set; }
    }
}
