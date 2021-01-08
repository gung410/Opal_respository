using FluentValidation;

namespace Microservice.StandaloneSurvey.Domain.Validators.Survey
{
    public class SurveyTitleValidator : AbstractValidator<string>
    {
        public SurveyTitleValidator()
        {
            RuleFor(p => p)
                .NotEmpty()
                .MaximumLength(Entities.StandaloneSurvey.MaxTitleLength);
        }
    }
}
