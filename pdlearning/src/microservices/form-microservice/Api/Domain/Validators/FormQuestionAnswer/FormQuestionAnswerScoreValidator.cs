using FluentValidation;

namespace Microservice.Form.Domain.Validators.FormQuestionAnswer
{
    public class FormQuestionAnswerScoreValidator : AbstractValidator<double?>
    {
        public FormQuestionAnswerScoreValidator()
        {
            RuleFor(p => p.Value)
                .GreaterThanOrEqualTo(0)
                .When(p => p.HasValue);
        }
    }
}
