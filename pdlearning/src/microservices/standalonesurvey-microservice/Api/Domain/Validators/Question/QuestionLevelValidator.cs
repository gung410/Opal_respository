using FluentValidation;

namespace Microservice.StandaloneSurvey.Domain.Validators.Question
{
    public class QuestionLevelValidator : AbstractValidator<int?>
    {
        public QuestionLevelValidator()
        {
            RuleFor(p => p.Value)
                .GreaterThanOrEqualTo(0)
                .When(p => p.HasValue);
        }
    }
}
