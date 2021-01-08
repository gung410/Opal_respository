using FluentValidation;

namespace Microservice.Form.Domain.Validators.FormAnswer
{
    public class FormAnswerScoreValidator : AbstractValidator<double?>
    {
        public FormAnswerScoreValidator()
        {
            RuleFor(p => p.Value)
                .GreaterThanOrEqualTo(0)
                .When(p => p.HasValue);
        }
    }
}
