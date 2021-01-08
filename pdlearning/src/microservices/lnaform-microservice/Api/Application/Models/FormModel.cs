using System;
using System.Text.Json.Serialization;
using Microservice.LnaForm.Domain.ValueObjects.Form;
using FormEntity = Microservice.LnaForm.Domain.Entities.Form;

namespace Microservice.LnaForm.Application.Models
{
    public class FormModel
    {
        public FormModel()
        {
            // Do not remove this constructor. It is being used by JsonSerializer.Deserialize
        }

        public FormModel(FormEntity formEntity)
        {
            Id = formEntity.Id;
            OwnerId = formEntity.OwnerId;
            Title = formEntity.Title;
            Status = formEntity.Status;
            CreatedDate = formEntity.CreatedDate;
            ChangedDate = formEntity.ChangedDate;
            ParentId = formEntity.ParentId;
            OriginalObjectId = formEntity.OriginalObjectId;
            DepartmentId = formEntity.DepartmentId;
            SqRatingType = formEntity.SqRatingType;
            StartDate = formEntity.StartDate;
            EndDate = formEntity.EndDate;
            ArchiveDate = formEntity.ArchiveDate;
            ArchivedBy = formEntity.ArchivedBy;
            FormRemindDueDate = formEntity.FormRemindDueDate;
            RemindBeforeDays = formEntity.RemindBeforeDays;
        }

        public FormModel(FormEntity formEntity, bool canUnpublishFormStandalone)
        {
            Id = formEntity.Id;
            OwnerId = formEntity.OwnerId;
            Title = formEntity.Title;
            Status = formEntity.Status;
            CreatedDate = formEntity.CreatedDate;
            ChangedDate = formEntity.ChangedDate;
            ParentId = formEntity.ParentId;
            OriginalObjectId = formEntity.OriginalObjectId;
            DepartmentId = formEntity.DepartmentId;
            SqRatingType = formEntity.SqRatingType;
            StartDate = formEntity.StartDate;
            EndDate = formEntity.EndDate;
            ArchiveDate = formEntity.ArchiveDate;
            ArchivedBy = formEntity.ArchivedBy;
            IsArchived = formEntity.IsArchived;
            FormRemindDueDate = formEntity.FormRemindDueDate;
            RemindBeforeDays = formEntity.RemindBeforeDays;
            CanUnpublishFormStandalone = canUnpublishFormStandalone;
        }

        public Guid Id { get; set; }

        public Guid OwnerId { get; set; }

        public string Title { get; set; }

        public FormStatus Status { get; set; }

        public DateTime CreatedDate { get; set; }

        public DateTime? ChangedDate { get; set; }

        public DateTime? StartDate { get; set; }

        public DateTime? EndDate { get; set; }

        public Guid ParentId { get; set; }

        public Guid OriginalObjectId { get; set; }

        public SqRatingType? SqRatingType { get; set; }

        public bool? IsArchived { get; set; }

        public DateTime? ArchiveDate { get; set; }

        public Guid? ArchivedBy { get; set; }

        public DateTime? FormRemindDueDate { get; set; }

        public int RemindBeforeDays { get; set; }

        public bool CanUnpublishFormStandalone { get; set; } = true;

        [JsonIgnore]
        public int DepartmentId { get; set; }
    }
}
