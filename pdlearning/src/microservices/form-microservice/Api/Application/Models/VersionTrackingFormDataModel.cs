using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using System.Web;
using Microservice.Form.Domain.Entities;
using Microservice.Form.Domain.ValueObjects.Form;
using FormEntity = Microservice.Form.Domain.Entities.Form;

namespace Microservice.Form.Application.Models
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
            Type = formEntity.Type;
            SurveyType = formEntity.SurveyType;
            IsSurveyTemplate = formEntity.IsSurveyTemplate;
            Status = formEntity.Status;
            InSecondTimeLimit = formEntity.InSecondTimeLimit;
            RandomizedQuestions = formEntity.RandomizedQuestions;
            MaxAttempt = formEntity.MaxAttempt;
            PassingMarkPercentage = formEntity.PassingMarkPercentage;
            PassingMarkScore = formEntity.PassingMarkScore;
            CreatedDate = formEntity.CreatedDate;
            ChangedDate = formEntity.ChangedDate;
            PrimaryApprovingOfficerId = formEntity.PrimaryApprovingOfficerId;
            AlternativeApprovingOfficerId = formEntity.AlternativeApprovingOfficerId;
            IsAllowedDisplayPollResult = formEntity.IsAllowedDisplayPollResult;
            ParentId = formEntity.ParentId;
            OriginalObjectId = formEntity.OriginalObjectId;
            DepartmentId = formEntity.DepartmentId;
            IsShowFreeTextQuestionInPoll = formEntity.IsShowFreeTextQuestionInPoll;
            AnswerFeedbackDisplayOption = formEntity.AnswerFeedbackDisplayOption;
            AttemptToShowFeedback = formEntity.AttemptToShowFeedback;
            SqRatingType = formEntity.SqRatingType;
            StartDate = formEntity.StartDate;
            EndDate = formEntity.EndDate;
            ArchiveDate = formEntity.ArchiveDate;
            ArchivedBy = formEntity.ArchivedBy;
            IsStandalone = formEntity.IsStandalone;
            StandaloneMode = formEntity.StandaloneMode;
            FormRemindDueDate = formEntity.FormRemindDueDate;
            RemindBeforeDays = formEntity.RemindBeforeDays;
            IsSendNotification = formEntity.IsSendNotification;

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

        public FormType Type { get; set; }

        public FormSurveyType? SurveyType { get; set; }

        public bool? IsSurveyTemplate { get; set; }

        public FormStatus Status { get; set; }

        public int? InSecondTimeLimit { get; set; }

        public bool RandomizedQuestions { get; set; }

        public short? MaxAttempt { get; set; }

        public short? PassingMarkPercentage { get; set; }

        public int? PassingMarkScore { get; set; }

        public DateTime CreatedDate { get; set; }

        public DateTime? ChangedDate { get; set; }

        public DateTime? StartDate { get; set; }

        public DateTime? EndDate { get; set; }

        public Guid? PrimaryApprovingOfficerId { get; set; }

        public Guid? AlternativeApprovingOfficerId { get; set; }

        public bool? IsAllowedDisplayPollResult { get; set; }

        public Guid ParentId { get; set; }

        public Guid OriginalObjectId { get; set; }

        public bool? IsShowFreeTextQuestionInPoll { get; set; }

        public AnswerFeedbackDisplayOption? AnswerFeedbackDisplayOption { get; set; }

        public SqRatingType? SqRatingType { get; set; }

        public short? AttemptToShowFeedback { get; set; }

        public bool? IsArchived { get; set; }

        public DateTime? ArchiveDate { get; set; }

        public Guid? ArchivedBy { get; set; }

        public bool? IsStandalone { get; set; }

        public FormStandaloneMode? StandaloneMode { get; set; }

        public DateTime? FormRemindDueDate { get; set; }

        public int RemindBeforeDays { get; set; }

        public bool IsSendNotification { get; set; }

        [JsonIgnore]
        public int DepartmentId { get; set; }

        public IEnumerable<FormQuestionModel> FormQuestions { get; set; }

        public IEnumerable<FormSectionModel> FormSections { get; set; }
    }
}
