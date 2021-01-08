using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using System.Web;
using Microservice.StandaloneSurvey.Domain.Entities;
using Microservice.StandaloneSurvey.Domain.ValueObjects.Survey;

namespace Microservice.StandaloneSurvey.Application.Models
{
    public class VersionTrackingSurveyDataModel
    {
        public VersionTrackingSurveyDataModel()
        {
            // Do not remove this constructor. It is being used by JsonSerializer.Deserialize
        }

        public VersionTrackingSurveyDataModel(Domain.Entities.StandaloneSurvey formEntity, IEnumerable<SurveyQuestion> formQuestionEntities, IEnumerable<SurveySection> formSectionEntities)
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
                var questionModel = new SurveyQuestionModel(question);
                questionModel.QuestionTitle = HttpUtility.HtmlDecode(questionModel.QuestionTitle);
                return questionModel;
            });
            FormSections = formSectionEntities.Select(section => new SurveySectionModel(section));
        }

        public Guid Id { get; set; }

        public Guid OwnerId { get; set; }

        public string Title { get; set; }

        public SurveyStatus Status { get; set; }

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

        public IEnumerable<SurveyQuestionModel> FormQuestions { get; set; }

        public IEnumerable<SurveySectionModel> FormSections { get; set; }
    }
}
