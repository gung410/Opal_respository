using System;
using Microservice.Course.Domain.Entities;
using Microservice.Course.Domain.Enums;
using Microservice.Course.Domain.ValueObjects;

namespace Microservice.Course.Application.Models
{
    public class RegistrationModel
    {
        public RegistrationModel()
        {
        }

        public RegistrationModel(Registration entity)
        {
            Id = entity.Id;
            UserId = entity.UserId;
            CourseId = entity.CourseId;
            ClassRunId = entity.ClassRunId;
            RegistrationType = entity.RegistrationType;
            RegistrationDate = entity.RegistrationDate;
            CourseId = entity.CourseId;
            Status = entity.Status;
            LastStatusChangedDate = entity.LastStatusChangedDate;
            WithdrawalStatus = entity.WithdrawalStatus;
            WithdrawalRequestDate = entity.WithdrawalRequestDate;
            ClassRunChangeStatus = entity.ClassRunChangeStatus;
            ClassRunChangeRequestedDate = entity.ClassRunChangeRequestedDate;
            ClassRunChangeId = entity.ClassRunChangeId;
            ApprovingOfficer = entity.ApprovingOfficer;
            ApprovingDate = entity.ApprovingDate;
            AdministratedBy = entity.AdministratedBy;
            AdministrationDate = entity.AdministrationDate;
            Status = entity.Status;
            LearningStatus = entity.LearningStatus;
            LearningContentProgress = entity.LearningContentProgress;
            PostCourseEvaluationFormCompleted = entity.PostCourseEvaluationFormCompleted;
            CreatedDate = entity.CreatedDate;
            CourseCriteriaOverrided = entity.CourseCriteriaOverrided;
            CourseCriteriaViolation = entity.CourseCriteriaViolation;
            IsExpired = entity.IsExpired;
            LearningCompletedDate = entity.LearningCompletedDate;
        }

        public RegistrationModel(Registration entity, CourseUser user) : this(entity)
        {
            User = user != null ? new UserModel(user) : null;
        }

        public RegistrationModel(Registration entity, CourseUser user, CourseEntity course, ClassRun classRun) : this(entity, user)
        {
            Course = course != null ? new CourseModel(course) : null;
            ClassRun = classRun != null ? new ClassRunModel(classRun) : null;
        }

        public Guid Id { get; set; }

        public Guid UserId { get; set; }

        public Guid CourseId { get; set; }

        public Guid? ClassRunId { get; set; }

        public RegistrationType? RegistrationType { get; set; }

        public DateTime? RegistrationDate { get; set; }

        public RegistrationStatus? Status { get; set; }

        public DateTime CreatedDate { get; set; }

        public DateTime? LastStatusChangedDate { get; set; }

        public WithdrawalStatus? WithdrawalStatus { get; set; }

        public DateTime? WithdrawalRequestDate { get; set; }

        public ClassRunChangeStatus? ClassRunChangeStatus { get; set; }

        public DateTime? ClassRunChangeRequestedDate { get; set; }

        public Guid? ClassRunChangeId { get; set; }

        public Guid? ApprovingOfficer { get; set; }

        public DateTime? ApprovingDate { get; set; }

        public Guid? AdministratedBy { get; set; }

        public DateTime? AdministrationDate { get; set; }

        public double? LearningContentProgress { get; set; }

        public LearningStatus LearningStatus { get; set; }

        public bool PostCourseEvaluationFormCompleted { get; set; }

        public bool? CourseCriteriaOverrided { get; set; }

        public DateTime? LearningCompletedDate { get; set; }

        public CourseCriteriaLearnerViolation CourseCriteriaViolation { get; set; }

        /// <summary>
        /// This property is optional load, it could be null.
        /// </summary>
        public UserModel User { get; set; }

        /// <summary>
        /// This property is optional load, it could be null.
        /// </summary>
        public CourseModel Course { get; set; }

        /// <summary>
        /// This property is optional load, it could be null.
        /// </summary>
        public ClassRunModel ClassRun { get; set; }

        public bool IsExpired { get; set; }
    }
}
