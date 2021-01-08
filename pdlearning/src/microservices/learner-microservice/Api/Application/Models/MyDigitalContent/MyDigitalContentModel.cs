using System;
using Microservice.Learner.Domain.Entities;
using Microservice.Learner.Domain.ValueObject;

namespace Microservice.Learner.Application.Models
{
    public class MyDigitalContentModel
    {
        public MyDigitalContentModel()
        {
        }

        public MyDigitalContentModel(MyDigitalContent entity)
        {
            Id = entity.Id;
            DigitalContentId = entity.DigitalContentId;
            Version = entity.Version;
            UserId = entity.UserId;
            Status = entity.Status;
            DigitalContentType = entity.DigitalContentType;
            ReviewStatus = entity.ReviewStatus;
            ProgressMeasure = entity.ProgressMeasure;
            DisenrollUtc = entity.DisenrollUtc;
            ReadDate = entity.ReadDate;
            ReminderSentDate = entity.ReminderSentDate;
            StartDate = entity.StartDate;
            EndDate = entity.EndDate;
            CompletedDate = entity.CompletedDate;
            CreatedDate = entity.CreatedDate;
            CreatedBy = entity.CreatedBy;
            ChangedDate = entity.ChangedDate;
            ChangedBy = entity.ChangedBy;
        }

        public Guid Id { get; set; }

        public Guid UserId { get; set; }

        public Guid DigitalContentId { get; set; }

        public MyDigitalContentStatus Status { get; set; }

        public DigitalContentType DigitalContentType { get; set; }

        public string Version { get; set; }

        public string ReviewStatus { get; set; }

        public double? ProgressMeasure { get; set; }

        public bool IsDeleted { get; set; }

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
    }
}
