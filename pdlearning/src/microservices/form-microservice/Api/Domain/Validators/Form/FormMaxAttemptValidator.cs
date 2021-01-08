using FluentValidation;

namespace Microservice.Form.Domain.Validators.Form
{
    public class FormMaxAttemptValidator : AbstractValidator<short?>
    {
        public FormMaxAttemptValidator()
        {
            RuleFor(p => p.Value)
                .GreaterThanOrEqualTo((short)1)
                .When(p => p.HasValue);
        }
    }
}
