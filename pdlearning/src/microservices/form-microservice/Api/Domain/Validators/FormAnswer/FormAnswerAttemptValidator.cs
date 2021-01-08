using FluentValidation;

namespace Microservice.Form.Domain.Validators.FormAnswer
{
    public class FormAnswerAttemptValidator : AbstractValidator<short>
    {
        public FormAnswerAttemptValidator()
        {
            RuleFor(p => p).GreaterThanOrEqualTo((short)1);
        }
    }
}
