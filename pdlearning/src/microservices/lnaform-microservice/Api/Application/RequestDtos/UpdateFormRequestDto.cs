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
    public class UpdateFormRequestDto
    {
        public Guid Id { get; set; }

        public string Title { get; set; }

        public FormStatus Status { get; set; }

        public IEnumerable<UpdateFormRequestDtoFormQuestion> ToSaveFormQuestions { get; set; } = new List<UpdateFormRequestDtoFormQuestion>();

        public IEnumerable<CreateFormSectionRequestDto> FormSections { get; set; } = new List<CreateFormSectionRequestDto>();

        public IEnumerable<Guid> ToDeleteFormQuestionIds { get; set; } = new List<Guid>();

        public IEnumerable<Guid> ToDeleteFormSectionIds { get; set; } = new List<Guid>();

        public bool IsAutoSave { get; set; }

        public SqRatingType? SqRatingType { get; set; }

        public DateTime? StartDate { get; set; }

        public DateTime? EndDate { get; set; }

        public DateTime? ArchiveDate { get; set; }

        public bool? IsUpdateToNewVersion { get; set; }

        public FormModel ToFormModel()
        {
            return new FormModel
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

    public class UpdateFormRequestDtoFormQuestion
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

        public SaveFormQuestionCommand BuildSaveFormCommand(Guid formId)
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
                NextQuestionId = NextQuestionId,
                FormSectionId = FormSectionId,
            };
        }
    }
}
#pragma warning restore SA1402 // File may only contain a single type
