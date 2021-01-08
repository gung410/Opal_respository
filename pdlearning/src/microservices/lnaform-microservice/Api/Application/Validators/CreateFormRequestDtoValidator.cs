using FluentValidation;
using Microservice.LnaForm.Application.RequestDtos;
using Microservice.LnaForm.Application.Services;
using Microservice.LnaForm.Domain.Validators.Form;
using Microservice.LnaForm.Domain.Validators.FormQuestion;
using Microservice.LnaForm.Domain.Validators.Question;

#pragma warning disable SA1402 // File may only contain a single type
namespace Microservice.LnaForm.Application.Validators
{
    public class CreateFormRequestDtoValidator : AbstractValidator<CreateFormRequestDto>
    {
        public CreateFormRequestDtoValidator()
        {
            RuleFor(p => p.Title)
                .NotNull()
                .SetValidator(new FormTitleValidator())
                .When(p => p.IsAutoSave == false);
            RuleForEach(p => p.FormQuestions)
                .NotNull()
                .SetValidator(new CreateFormRequestDtoFormQuestionValidator())
                .When(p => p.IsAutoSave == false);
        }
    }

    public class CreateFormRequestDtoFormQuestionValidator : AbstractValidator<CreateFormRequestDtoFormQuestion>
    {
        public CreateFormRequestDtoFormQuestionValidator()
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
