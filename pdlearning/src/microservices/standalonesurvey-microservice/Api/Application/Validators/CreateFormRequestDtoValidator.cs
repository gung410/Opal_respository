using FluentValidation;
using Microservice.StandaloneSurvey.Application.RequestDtos;
using Microservice.StandaloneSurvey.Domain.Validators.Question;
using Microservice.StandaloneSurvey.Domain.Validators.Survey;
using Microservice.StandaloneSurvey.Domain.Validators.SurveyQuestion;

#pragma warning disable SA1402 // File may only contain a single type
namespace Microservice.StandaloneSurvey.Application.Validators
{
    public class CreateFormRequestDtoValidator : AbstractValidator<CreateSurveyRequestDto>
    {
        public CreateFormRequestDtoValidator()
        {
            RuleFor(p => p.Title)
                .NotNull()
                .SetValidator(new SurveyTitleValidator())
                .When(p => p.IsAutoSave == false);
            RuleForEach(p => p.FormQuestions)
                .NotNull()
                .SetValidator(new CreateFormRequestDtoFormQuestionValidator())
                .When(p => p.IsAutoSave == false);
        }
    }

    public class CreateFormRequestDtoFormQuestionValidator : AbstractValidator<CreateSurveyRequestDtoFormQuestion>
    {
        public CreateFormRequestDtoFormQuestionValidator()
        {
            RuleFor(p => p.QuestionTitle)
                .NotNull()
                .SetValidator(new QuestionTitleValidator());
            RuleFor(p => p.Priority)
                .SetValidator(new SurveyQuestionPriorityValidator());
        }
    }
}
#pragma warning restore SA1402 // File may only contain a single type
