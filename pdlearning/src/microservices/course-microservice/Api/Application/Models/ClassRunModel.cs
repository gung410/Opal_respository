using System;
using System.Collections.Generic;
using Microservice.Course.Domain.Entities;
using Microservice.Course.Domain.Enums;

namespace Microservice.Course.Application.Models
{
    public class ClassRunModel
    {
        public ClassRunModel()
        {
        }

        public ClassRunModel(ClassRun entity)
        {
            Id = entity.Id;
            ClassTitle = entity.ClassTitle;
            ClassRunCode = entity.ClassRunCode;
            ApplicationEndDate = entity.ApplicationEndDate;
            ApplicationStartDate = entity.ApplicationStartDate;
            FacilitatorIds = entity.FacilitatorIds;
            CoFacilitatorIds = entity.CoFacilitatorIds;
            CourseId = entity.CourseId;
            StartDateTime = entity.StartDateTime;
            EndDateTime = entity.EndDateTime;
            PlanningStartTime = entity.PlanningStartTime;
            PlanningEndTime = entity.PlanningEndTime;
            MaxClassSize = entity.MaxClassSize;
            MinClassSize = entity.MinClassSize;
            RescheduleStartDateTime = entity.RescheduleStartDateTime;
            RescheduleEndDateTime = entity.RescheduleEndDateTime;
            Status = entity.Status;
            SubmittedContentDate = entity.SubmittedContentDate;
            ApprovalContentDate = entity.ApprovalContentDate;
            CancellationStatus = entity.CancellationStatus;
            RescheduleStatus = entity.RescheduleStatus;
            CreatedBy = entity.CreatedBy;
            CreatedDate = entity.CreatedDate;
            ContentStatus = entity.ContentStatus;
            PublishedContentDate = entity.PublishedContentDate;
            CourseCriteriaActivated = entity.CourseCriteriaActivated;
            CourseAutomateActivated = entity.CourseAutomateActivated;

            IsStarted = entity.Started();
            IsEnded = entity.Ended();
        }

        public Guid Id { get; set; }

        public Guid CourseId { get; set; }

        public string ClassTitle { get; set; }

        public string ClassRunCode { get; set; }

        public DateTime? StartDateTime { get; set; }

        public DateTime? EndDateTime { get; set; }

        public DateTime? PlanningStartTime { get; set; }

        public DateTime? PlanningEndTime { get; set; }

        public IEnumerable<Guid> FacilitatorIds { get; set; }

        public IEnumerable<Guid> CoFacilitatorIds { get; set; }

        public int MinClassSize { get; set; }

        public int MaxClassSize { get; set; }

        public DateTime? ApplicationStartDate { get; set; }

        public DateTime? ApplicationEndDate { get; set; }

        public DateTime? RegistrationStartDate { get; set; }

        public DateTime? RegistrationEndDate { get; set; }

        public DateTime? RescheduleStartDateTime { get; set; }

        public DateTime? RescheduleEndDateTime { get; set; }

        public ClassRunCancellationStatus? CancellationStatus { get; set; }

        public ClassRunRescheduleStatus? RescheduleStatus { get; set; }

        public ClassRunStatus Status { get; set; }

        public DateTime? SubmittedContentDate { get; set; }

        public DateTime? ApprovalContentDate { get; set; }

        public Guid CreatedBy { get; set; }

        public DateTime CreatedDate { get; set; }

        public ContentStatus ContentStatus { get; set; }

        public DateTime? PublishedContentDate { get; set; }

        public bool? HasContent { get; set; }

        public bool IsStarted { get; set; }

        public bool IsEnded { get; set; }

        public bool CourseCriteriaActivated { get; set; }

        public bool CourseAutomateActivated { get; set; }

        public bool? HasLearnerStarted { get; set; }

        public ClassRunModel SetHasContent(bool? value)
        {
            var cloned = (ClassRunModel)MemberwiseClone();
            cloned.HasContent = value;
            return cloned;
        }

        public ClassRunModel SetHasLearnerStarted(bool? value)
        {
            var cloned = (ClassRunModel)MemberwiseClone();
            cloned.HasLearnerStarted = value;
            return cloned;
        }
    }
}
