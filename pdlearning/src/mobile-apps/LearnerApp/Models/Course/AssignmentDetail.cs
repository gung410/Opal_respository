using System;
using LearnerApp.Common;
using LearnerApp.Models.Learner;

namespace LearnerApp.Models.Course
{
    public class AssignmentDetail
    {
        public AssignmentDetail()
        {
        }

        public string Id { get; set; }

        public string CourseId { get; set; }

        public string ClassRunId { get; set; }

        public string Title { get; set; }

        public DateTime StartDate { get; set; }

        public DateTime EndDate { get; set; }

        public bool RandomizedQuestions { get; set; }

        public AssignmentType Type { get; set; }

        public string CreatedBy { get; set; }

        public string ChangedBy { get; set; }

        public DateTime CreatedDate { get; set; }

        public DateTime? DeletedDate { get; set; }

        public DateTime? ChangedDate { get; set; }

        public string ParticipantAssignmentTrackId { get; set; }

        public string RegistrationId { get; set; }

        public DateTime AssignedDate { get; set; }

        public DateTime? SubmittedDate { get; set; }

        public ParticipantAssignmentTrackStatus Status { get; set; }

        public bool IsLastItem { get; set; }

        public QuizAnswer QuizAnswer { get; set; }

        public static AssignmentDetail CreateFrom(
            Assignment assignment,
            AssignmentInfo assignmentInfo,
            ParticipantAssignmentTrack participantAssignmentTrack)
        {
            return new AssignmentDetail()
            {
                Id = assignmentInfo.Id,
                CourseId = assignmentInfo.CourseId,
                ClassRunId = assignmentInfo.ClassRunId,
                Title = assignmentInfo.Title,
                StartDate = participantAssignmentTrack.StartDate,
                EndDate = participantAssignmentTrack.EndDate,
                RandomizedQuestions = assignmentInfo.RandomizedQuestions,
                Type = assignmentInfo.Type,
                CreatedBy = assignmentInfo.CreatedBy,
                ChangedBy = assignmentInfo.ChangedBy,
                CreatedDate = assignmentInfo.CreatedDate,
                DeletedDate = assignmentInfo.DeletedDate,
                ChangedDate = assignmentInfo.ChangedDate,
                ParticipantAssignmentTrackId = participantAssignmentTrack.Id,
                RegistrationId = participantAssignmentTrack.RegistrationId,
                AssignedDate = participantAssignmentTrack.AssignedDate,
                SubmittedDate = participantAssignmentTrack.SubmittedDate,
                Status = assignment.Status,
                IsLastItem = false,
                QuizAnswer = participantAssignmentTrack.QuizAnswer,
            };
        }
    }
}
