using FluentValidation;

namespace Microservice.Form.Domain.Validators.Form
{
    public class FormInSecondTimeLimitValidator : AbstractValidator<int?>
    {
        public FormInSecondTimeLimitValidator()
        {
            RuleFor(p => p.Value)
                .GreaterThanOrEqualTo(1)
                .LessThanOrEqualTo(int.MaxValue)
                .When(p => p.HasValue);
        }
    }
}
