using FluentValidation;
using Microservice.LnaForm.Application.RequestDtos;
using Microservice.LnaForm.Domain.Validators.Form;
using Microservice.LnaForm.Domain.Validators.FormQuestion;
using Microservice.LnaForm.Domain.Validators.Question;

#pragma warning disable SA1402 // File may only contain a single type
namespace Microservice.LnaForm.Application.Validators
{
    public class UpdateFormRequestDtoValidator : AbstractValidator<UpdateFormRequestDto>
    {
        public UpdateFormRequestDtoValidator()
        {
            RuleFor(p => p.Title)
                .NotNull()
                .SetValidator(new FormTitleValidator())
                .When(p => p.IsAutoSave == false);
            RuleForEach(p => p.ToSaveFormQuestions)
                .NotNull()
                .SetValidator(new UpdateFormRequestDtoFormQuestionValidator())
                .When(p => p.IsAutoSave == false);
        }
    }

    public class UpdateFormRequestDtoFormQuestionValidator : AbstractValidator<UpdateFormRequestDtoFormQuestion>
    {
        public UpdateFormRequestDtoFormQuestionValidator()
        {
            RuleFor(p => p.QuestionTitle)
                .NotNull()
                .SetValidator(new QuestionTitleValidator());
            RuleFor(p => p.Priority)
                .SetValidator(new FormQuestionPriorityValidator());
        }
    }
}
#pragma warning restore SA1402 // File may only contain a single type
