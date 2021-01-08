using FluentValidation;
using Microservice.Form.Application.RequestDtos;
using Microservice.Form.Application.Services;
using Microservice.Form.Domain.Validators.Form;
using Microservice.Form.Domain.Validators.FormQuestion;
using Microservice.Form.Domain.Validators.Question;
using Microservice.Form.Domain.ValueObjects.Form;

#pragma warning disable SA1402 // File may only contain a single type
namespace Microservice.Form.Application.Validators
{
    public class UpdateFormRequestDtoValidator : AbstractValidator<UpdateFormRequestDto>
    {
        public UpdateFormRequestDtoValidator()
        {
            RuleFor(p => p.Title)
                .NotNull()
                .SetValidator(new FormTitleValidator())
                .When(p => p.IsAutoSave == false);
            RuleFor(p => p.InSecondTimeLimit)
                .SetValidator(new FormInSecondTimeLimitValidator())
                .When(p => p.IsAutoSave == false);
            RuleFor(p => p.MaxAttempt)
                .SetValidator(new FormMaxAttemptValidator())
                .When(p => p.IsAutoSave == false);
            RuleForEach(p => p.ToSaveFormQuestions)
                .NotNull()
                .SetValidator(new UpdateFormRequestDtoFormQuestionValidator())
                .When(p => p.IsAutoSave == false && p.Type != FormType.Poll);
        }
    }

    public class UpdateFormRequestDtoFormQuestionValidator : AbstractValidator<UpdateFormRequestDtoFormQuestion>
    {
        public UpdateFormRequestDtoFormQuestionValidator()
        {
            RuleFor(p => p.QuestionTitle)
                .NotNull()
                .SetValidator(new QuestionTitleValidator());
            RuleFor(p => p.Score)
                .SetValidator(new FormQuestionScoreValidator());
            RuleFor(p => p.Priority)
                .SetValidator(new FormQuestionPriorityValidator());
        }
    }
}
#pragma warning restore SA1402 // File may only contain a single type
