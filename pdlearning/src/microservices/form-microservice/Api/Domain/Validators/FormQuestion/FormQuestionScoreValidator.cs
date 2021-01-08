using FluentValidation;

namespace Microservice.Form.Domain.Validators.FormQuestion
{
    public class FormQuestionScoreValidator : AbstractValidator<double?>
    {
        public FormQuestionScoreValidator()
        {
            RuleFor(p => p.Value)
                .GreaterThanOrEqualTo(0)
                .When(p => p.HasValue);
        }
    }
}
