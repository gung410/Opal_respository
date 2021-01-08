using FluentValidation;
using QuestionEntity = Microservice.StandaloneSurvey.Domain.ValueObjects.Questions.Question;

namespace Microservice.StandaloneSurvey.Domain.Validators.Question
{
    public class QuestionTitleValidator : AbstractValidator<string>
    {
        public QuestionTitleValidator()
        {
            RuleFor(p => p)
                .MaximumLength(20000)
                .NotEmpty();
        }
    }
}
