using System;
using System.Collections.Generic;
using Microservice.StandaloneSurvey.Application.Commands;
using Microservice.StandaloneSurvey.Application.Models;
using Microservice.StandaloneSurvey.Application.Services;
using Microservice.StandaloneSurvey.Domain.ValueObjects.Questions;
using Microservice.StandaloneSurvey.Domain.ValueObjects.Survey;

#pragma warning disable SA1402 // File may only contain a single type
namespace Microservice.StandaloneSurvey.Application.RequestDtos
{
    public class CreateSurveyRequestDto : HasSubModuleInfoBase
    {
        public string Title { get; set; }

        public SurveyStatus Status { get; set; }

        public IEnumerable<CreateSurveyRequestDtoFormQuestion> FormQuestions { get; set; } = new List<CreateSurveyRequestDtoFormQuestion>();

        public IEnumerable<CreateSurveySectionRequestDto> FormSections { get; set; } = new List<CreateSurveySectionRequestDto>();

        public bool IsAutoSave { get; set; }

        public SqRatingType? SqRatingType { get; set; }

        public DateTime? StartDate { get; set; }

        public DateTime? EndDate { get; set; }

        public StandaloneSurveyModel ToStandaloneSurveyModel(Guid formId)
        {
            return new StandaloneSurveyModel
            {
                Id = formId,
                Title = Title,
                Status = Status,
                SqRatingType = SqRatingType,
                StartDate = StartDate,
                EndDate = EndDate
            };
        }
    }

    public class CreateSurveyRequestDtoFormQuestion
    {
        public QuestionType QuestionType { get; set; }

        public string QuestionTitle { get; set; }

        public object QuestionCorrectAnswer { get; set; }

        public IEnumerable<QuestionOptionModel> QuestionOptions { get; set; } = new List<QuestionOptionModel>();

        public int Priority { get; set; }

        public int? MinorPriority { get; set; }

        public Guid? NextQuestionId { get; set; }

        public SaveSurveyQuestionCommand BuildSaveFormQuestionCommand(Guid formId)
        {
            return new SaveSurveyQuestionCommand
            {
                SurveyId = formId,
                QuestionTitle = QuestionTitle,
                QuestionType = QuestionType,
                QuestionCorrectAnswer = QuestionCorrectAnswer,
                QuestionOptions = QuestionOptions,
                Priority = Priority,
                MinorPriority = MinorPriority,
                NextQuestionId = NextQuestionId
            };
        }
    }
}
#pragma warning restore SA1402 // File may only contain a single type
