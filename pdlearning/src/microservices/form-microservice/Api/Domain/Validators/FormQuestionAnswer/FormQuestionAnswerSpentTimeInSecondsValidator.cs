using FluentValidation;

namespace Microservice.Form.Domain.Validators.FormQuestionAnswer
{
    public class FormQuestionAnswerSpentTimeInSecondsValidator : AbstractValidator<int?>
    {
        public FormQuestionAnswerSpentTimeInSecondsValidator()
        {
            RuleFor(p => p.Value)
                .GreaterThanOrEqualTo(0)
                .When(p => p.HasValue);
        }
    }
}
