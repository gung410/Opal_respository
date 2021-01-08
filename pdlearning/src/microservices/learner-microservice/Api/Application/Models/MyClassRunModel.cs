using System;
using Microservice.Learner.Domain.ValueObject;

namespace Microservice.Learner.Application.Models
{
    public class MyClassRunModel
    {
        public MyClassRunModel(Domain.Entities.MyClassRun entity)
        {
            Id = entity.Id;
            UserId = entity.UserId;
            CourseId = entity.CourseId;
            ClassRunId = entity.ClassRunId;
            Status = entity.Status;
            RegistrationId = entity.RegistrationId;
            WithdrawalStatus = entity.WithdrawalStatus;
            RegistrationType = entity.RegistrationType;
            ChangedBy = entity.ChangedBy;
            AdministratedBy = entity.AdministratedBy;
            ChangedDate = entity.ChangedDate;
            ClassRunChangeStatus = entity.ClassRunChangeStatus;
            ClassRunChangeRequestedDate = entity.ClassRunChangeRequestedDate;
            ClassRunChangeId = entity.ClassRunChangeId;
            LearningStatus = entity.LearningStatus;
            PostCourseEvaluationFormCompleted = entity.PostCourseEvaluationFormCompleted;
            IsExpired = entity.IsExpired;
        }

        public Guid Id { get; set; }

        public Guid UserId { get; set; }

        public Guid CourseId { get; set; }

        public Guid ClassRunId { get; set; }

        public RegistrationStatus Status { get; set; }

        public Guid RegistrationId { get; set; }

        public WithdrawalStatus? WithdrawalStatus { get; set; }

        public RegistrationType RegistrationType { get; set; }

        public Guid? ChangedBy { get; set; }

        public Guid? AdministratedBy { get; set; }

        public DateTime? ChangedDate { get; set; }

        public ClassRunChangeStatus? ClassRunChangeStatus { get; set; }

        public DateTime? ClassRunChangeRequestedDate { get; set; }

        public Guid? ClassRunChangeId { get; set; }

        public LearningStatus? LearningStatus { get; set; }

        public bool PostCourseEvaluationFormCompleted { get; set; }

        public bool IsExpired { get; set; }
    }
}
