using System;
using System.Collections.Generic;
using Microservice.Form.Application.Commands;
using Microservice.Form.Application.Models;
using Microservice.Form.Application.Services;
using Microservice.Form.Domain.ValueObjects.Form;
using Microservice.Form.Domain.ValueObjects.Questions;

#pragma warning disable SA1402 // File may only contain a single type
namespace Microservice.Form.Application.RequestDtos
{
    public class CreateFormRequestDto
    {
        public string Title { get; set; }

        public FormType Type { get; set; }

        public FormStatus Status { get; set; }

        public int? InSecondTimeLimit { get; set; }

        public bool RandomizedQuestions { get; set; }

        public short? MaxAttempt { get; set; }

        public Guid? PrimaryApprovingOfficerId { get; set; }

        public Guid? AlternativeApprovingOfficerId { get; set; }

        public bool? IsAllowedDisplayPollResult { get; set; }

        public IEnumerable<CreateFormRequestDtoFormQuestion> FormQuestions { get; set; } = new List<CreateFormRequestDtoFormQuestion>();

        public IEnumerable<CreateFormSectionRequestDto> FormSections { get; set; } = new List<CreateFormSectionRequestDto>();

        public bool IsAutoSave { get; set; }

        public AnswerFeedbackDisplayOption? AnswerFeedbackDisplayOption { get; set; }

        public SqRatingType? SqRatingType { get; set; }

        public DateTime? StartDate { get; set; }

        public DateTime? EndDate { get; set; }

        public bool? IsStandalone { get; set; }

        public FormStandaloneMode? StandaloneMode { get; set; }

        public DateTime? FormRemindDueDate { get; set; }

        public int RemindBeforeDays { get; set; }

        public bool IsSendNotification { get; set; }

        public bool? PublishToCatalogue { get; set; } = false;

        public FormModel ToFormModel(Guid formId)
        {
            return new FormModel()
            {
                Id = formId,
                Title = Title,
                Type = Type,
                Status = Status,
                InSecondTimeLimit = InSecondTimeLimit,
                RandomizedQuestions = RandomizedQuestions,
                MaxAttempt = MaxAttempt,
                PrimaryApprovingOfficerId = PrimaryApprovingOfficerId,
                AlternativeApprovingOfficerId = AlternativeApprovingOfficerId,
                IsAllowedDisplayPollResult = IsAllowedDisplayPollResult,
                AnswerFeedbackDisplayOption = AnswerFeedbackDisplayOption,
                SqRatingType = SqRatingType,
                StartDate = StartDate,
                EndDate = EndDate,
                IsStandalone = IsStandalone,
                StandaloneMode = StandaloneMode,
                FormRemindDueDate = FormRemindDueDate,
                RemindBeforeDays = RemindBeforeDays,
                IsSendNotification = IsSendNotification,
                PublishToCatalogue = PublishToCatalogue
            };
        }
    }

    public class CreateFormRequestDtoFormQuestion
    {
        public Guid? Id { get; set; }

        public QuestionType QuestionType { get; set; }

        public string QuestionTitle { get; set; }

        public object QuestionCorrectAnswer { get; set; }

        public IEnumerable<QuestionOptionModel> QuestionOptions { get; set; } = new List<QuestionOptionModel>();

        public int Priority { get; set; }

        public int? MinorPriority { get; set; }

        public string QuestionHint { get; set; }

        public string AnswerExplanatoryNote { get; set; }

        public string FeedbackCorrectAnswer { get; set; }

        public string FeedbackWrongAnswer { get; set; }

        public int? QuestionLevel { get; set; }

        public bool? RandomizedOptions { get; set; }

        public double? Score { get; set; }

        public bool? IsSurveyTemplateQuestion { get; set; }

        public Guid? NextQuestionId { get; set; }

        public SaveFormQuestionCommand BuildSaveFormQuestionCommand(Guid formId)
        {
            return new SaveFormQuestionCommand
            {
                Id = Id,
                FormId = formId,
                QuestionTitle = QuestionTitle,
                QuestionType = QuestionType,
                QuestionCorrectAnswer = QuestionCorrectAnswer,
                QuestionOptions = QuestionOptions,
                Priority = Priority,
                MinorPriority = MinorPriority,
                QuestionHint = QuestionHint,
                AnswerExplanatoryNote = AnswerExplanatoryNote,
                QuestionLevel = QuestionLevel,
                RandomizedOptions = RandomizedOptions,
                FeedbackCorrectAnswer = FeedbackCorrectAnswer,
                FeedbackWrongAnswer = FeedbackWrongAnswer,
                Score = Score,
                IsSurveyTemplateQuestion = IsSurveyTemplateQuestion,
                NextQuestionId = NextQuestionId
            };
        }
    }
}
#pragma warning restore SA1402 // File may only contain a single type
