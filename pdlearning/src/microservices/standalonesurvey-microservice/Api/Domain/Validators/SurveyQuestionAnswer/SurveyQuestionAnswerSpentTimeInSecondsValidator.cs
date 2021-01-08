using FluentValidation;

namespace Microservice.StandaloneSurvey.Domain.Validators.SurveyQuestionAnswer
{
    public class SurveyQuestionAnswerSpentTimeInSecondsValidator : AbstractValidator<int?>
    {
        public SurveyQuestionAnswerSpentTimeInSecondsValidator()
        {
            RuleFor(p => p.Value)
                .GreaterThanOrEqualTo(0)
                .When(p => p.HasValue);
        }
    }
}
