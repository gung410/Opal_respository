using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using System.Web;
using Microservice.LnaForm.Domain.Entities;
using Microservice.LnaForm.Domain.ValueObjects.Form;
using FormEntity = Microservice.LnaForm.Domain.Entities.Form;

namespace Microservice.LnaForm.Application.Models
{
    public class VersionTrackingFormDataModel
    {
        public VersionTrackingFormDataModel()
        {
            // Do not remove this constructor. It is being used by JsonSerializer.Deserialize
        }

        public VersionTrackingFormDataModel(FormEntity formEntity, IEnumerable<FormQuestion> formQuestionEntities, IEnumerable<FormSection> formSectionEntities)
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

            FormQuestions = formQuestionEntities.Select(question =>
            {
                var questionModel = new FormQuestionModel(question);
                questionModel.QuestionTitle = HttpUtility.HtmlDecode(questionModel.QuestionTitle);
                return questionModel;
            });
            FormSections = formSectionEntities.Select(section => new FormSectionModel(section));
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

        public DateTime? ArchiveDate { get; set; }

        public Guid? ArchivedBy { get; set; }

        public DateTime? FormRemindDueDate { get; set; }

        public int RemindBeforeDays { get; set; }

        [JsonIgnore]
        public int DepartmentId { get; set; }

        public IEnumerable<FormQuestionModel> FormQuestions { get; set; }

        public IEnumerable<FormSectionModel> FormSections { get; set; }
    }
}
