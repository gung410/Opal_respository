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
    public class UpdateSurveyRequestDto : HasSubModuleInfoBase
    {
        public Guid Id { get; set; }

        public string Title { get; set; }

        public SurveyStatus Status { get; set; }

        public IEnumerable<UpdateSurveyRequestDtoFormQuestion> ToSaveFormQuestions { get; set; } = new List<UpdateSurveyRequestDtoFormQuestion>();

        public IEnumerable<CreateSurveySectionRequestDto> FormSections { get; set; } = new List<CreateSurveySectionRequestDto>();

        public IEnumerable<Guid> ToDeleteFormQuestionIds { get; set; } = new List<Guid>();

        public IEnumerable<Guid> ToDeleteFormSectionIds { get; set; } = new List<Guid>();

        public bool IsAutoSave { get; set; }

        public SqRatingType? SqRatingType { get; set; }

        public DateTime? StartDate { get; set; }

        public DateTime? EndDate { get; set; }

        public DateTime? ArchiveDate { get; set; }

        public bool? IsUpdateToNewVersion { get; set; }

        public StandaloneSurveyModel ToStandaloneSurveyModel()
        {
            return new StandaloneSurveyModel
            {
                Id = Id,
                Title = Title,
                Status = Status,
                SqRatingType = SqRatingType,
                StartDate = StartDate,
                EndDate = EndDate,
                ArchiveDate = ArchiveDate
            };
        }
    }

    public class UpdateSurveyRequestDtoFormQuestion
    {
        public Guid? Id { get; set; }

        public QuestionType QuestionType { get; set; }

        public string QuestionTitle { get; set; }

        public object QuestionCorrectAnswer { get; set; }

        public IEnumerable<QuestionOptionModel> QuestionOptions { get; set; } = new List<QuestionOptionModel>();

        public int Priority { get; set; }

        public int? MinorPriority { get; set; }

        public Guid? NextQuestionId { get; set; }

        public Guid? FormSectionId { get; set; }

        public SaveSurveyQuestionCommand BuildSaveSurveyCommand(Guid formId)
        {
            return new SaveSurveyQuestionCommand
            {
                Id = Id,
                SurveyId = formId,
                QuestionTitle = QuestionTitle,
                QuestionType = QuestionType,
                QuestionCorrectAnswer = QuestionCorrectAnswer,
                QuestionOptions = QuestionOptions,
                Priority = Priority,
                MinorPriority = MinorPriority,
                NextQuestionId = NextQuestionId,
                SurveySectionId = FormSectionId,
            };
        }
    }
}
#pragma warning restore SA1402 // File may only contain a single type
