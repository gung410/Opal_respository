using System;
using Microservice.Learner.Domain.Entities;
using Microservice.Learner.Domain.ValueObject;

namespace Microservice.Learner.Application.Models
{
    public class MyCourseModel
    {
        public MyCourseModel()
        {
        }

        public MyCourseModel(MyCourse myCourse)
        {
            Id = myCourse.Id;
            CourseId = myCourse.CourseId;
            Version = myCourse.Version;
            UserId = myCourse.UserId;
            Status = myCourse.Status;
            ReviewStatus = myCourse.ReviewStatus;
            ProgressMeasure = myCourse.ProgressMeasure;
            LastLogin = myCourse.LastLogin;
            DisenrollUtc = myCourse.DisenrollUtc;
            ReadDate = myCourse.ReadDate;
            ReminderSentDate = myCourse.ReminderSentDate;
            StartDate = myCourse.StartDate;
            EndDate = myCourse.EndDate;
            CompletedDate = myCourse.CompletedDate;
            CreatedDate = myCourse.CreatedDate;
            CreatedBy = myCourse.CreatedBy;
            ChangedDate = myCourse.ChangedDate;
            ChangedBy = myCourse.ChangedBy;
            CurrentLecture = myCourse.CurrentLecture;
            CourseType = myCourse.CourseType;
            MyRegistrationStatus = myCourse.MyRegistrationStatus;
            ResultId = myCourse.ResultId;
            MyWithdrawalStatus = myCourse.MyWithdrawalStatus;
            DisplayStatus = myCourse.DisplayStatus;
            RegistrationId = myCourse.RegistrationId;
            HasContentChanged = myCourse.HasContentChanged;
            PostCourseEvaluationFormCompleted = myCourse.PostCourseEvaluationFormCompleted;
        }

        public Guid Id { get; set; }

        public Guid CourseId { get; set; }

        public string Version { get; set; }

        public Guid UserId { get; set; }

        public MyCourseStatus Status { get; set; }

        public string ReviewStatus { get; set; }

        public double? ProgressMeasure { get; set; }

        public DateTime? LastLogin { get; set; }

        public DateTime? DisenrollUtc { get; set; }

        public DateTime? ReadDate { get; set; }

        public DateTime? ReminderSentDate { get; set; }

        public DateTime? StartDate { get; set; }

        public DateTime? EndDate { get; set; }

        public DateTime? CompletedDate { get; set; }

        public DateTime CreatedDate { get; set; }

        public Guid CreatedBy { get; set; }

        public DateTime? ChangedDate { get; set; }

        public Guid? ChangedBy { get; set; }

        public Guid? CurrentLecture { get; set; }

        public LearningCourseType CourseType { get; set; }

        public RegistrationStatus? MyRegistrationStatus { get; set; }

        public WithdrawalStatus? MyWithdrawalStatus { get; set; }

        public Guid? ResultId { get; set; }

        public DisplayStatus? DisplayStatus { get; set; }

        public Guid? RegistrationId { get; set; }

        public Guid? ClassRunId { get; set; }

        public bool HasContentChanged { get; set; }

        public bool? PostCourseEvaluationFormCompleted { get; set; }
    }
}
