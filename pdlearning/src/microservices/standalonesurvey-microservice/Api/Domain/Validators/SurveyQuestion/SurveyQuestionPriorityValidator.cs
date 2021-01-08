using FluentValidation;

namespace Microservice.StandaloneSurvey.Domain.Validators.SurveyQuestion
{
    public class SurveyQuestionPriorityValidator : AbstractValidator<int>
    {
        public SurveyQuestionPriorityValidator()
        {
            RuleFor(p => p).GreaterThanOrEqualTo(0);
        }
    }
}
