using System;
using System.Collections.Generic;
using Microservice.LnaForm.Application.Commands;
using Microservice.LnaForm.Application.Models;
using Microservice.LnaForm.Application.Services;
using Microservice.LnaForm.Domain.ValueObjects.Form;
using Microservice.LnaForm.Domain.ValueObjects.Questions;

#pragma warning disable SA1402 // File may only contain a single type
namespace Microservice.LnaForm.Application.RequestDtos
{
    public class CreateFormRequestDto
    {
        public string Title { get; set; }

        public FormStatus Status { get; set; }

        public IEnumerable<CreateFormRequestDtoFormQuestion> FormQuestions { get; set; } = new List<CreateFormRequestDtoFormQuestion>();

        public IEnumerable<CreateFormSectionRequestDto> FormSections { get; set; } = new List<CreateFormSectionRequestDto>();

        public bool IsAutoSave { get; set; }

        public SqRatingType? SqRatingType { get; set; }

        public DateTime? StartDate { get; set; }

        public DateTime? EndDate { get; set; }

        public FormModel ToFormModel(Guid formId)
        {
            return new FormModel
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

    public class CreateFormRequestDtoFormQuestion
    {
        public QuestionType QuestionType { get; set; }

        public string QuestionTitle { get; set; }

        public object QuestionCorrectAnswer { get; set; }

        public IEnumerable<QuestionOptionModel> QuestionOptions { get; set; } = new List<QuestionOptionModel>();

        public int Priority { get; set; }

        public int? MinorPriority { get; set; }

        public Guid? NextQuestionId { get; set; }

        public SaveFormQuestionCommand BuildSaveFormQuestionCommand(Guid formId)
        {
            return new SaveFormQuestionCommand
            {
                FormId = formId,
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
