using FluentValidation;
using Microservice.StandaloneSurvey.Domain.Constants;

namespace Microservice.StandaloneSurvey.Domain.Validators.SurveyQuestion
{
    public class SurveyQuestionTitleValidator : AbstractValidator<string>
    {
        public SurveyQuestionTitleValidator()
        {
            RuleFor(p => p).MaximumLength(DomainConstants.DefaultStringMaxLength);
        }
    }
}
