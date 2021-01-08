using FluentValidation;

namespace Microservice.Form.Domain.Validators.FormAnswer
{
    public class FormAnswerScorePercentageValidator : AbstractValidator<double?>
    {
        public FormAnswerScorePercentageValidator()
        {
            RuleFor(p => p.Value)
                .InclusiveBetween(0, 100)
                .When(p => p.HasValue);
        }
    }
}
