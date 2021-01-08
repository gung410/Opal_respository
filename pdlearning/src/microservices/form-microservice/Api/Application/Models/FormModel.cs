using System;
using System.Text.Json.Serialization;
using Microservice.Form.Domain.ValueObjects.Form;
using FormEntity = Microservice.Form.Domain.Entities.Form;

namespace Microservice.Form.Application.Models
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
            PublishToCatalogue = formEntity.PublishToCatalogue;
        }

        public FormModel(FormEntity formEntity, bool canUnpublishFormStandalone)
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
            IsArchived = formEntity.IsArchived;
            FormRemindDueDate = formEntity.FormRemindDueDate;
            RemindBeforeDays = formEntity.RemindBeforeDays;
            IsSendNotification = formEntity.IsSendNotification;
            CanUnpublishFormStandalone = canUnpublishFormStandalone;
            PublishToCatalogue = formEntity.PublishToCatalogue;
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

        public bool CanUnpublishFormStandalone { get; set; } = true;

        public bool? PublishToCatalogue { get; set; } = false;

        [JsonIgnore]
        public int DepartmentId { get; set; }
    }
}
