using FluentValidation;

namespace Microservice.LnaForm.Domain.Validators.FormQuestionAnswer
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
