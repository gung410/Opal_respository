using FluentValidation;

namespace Microservice.Learner.Domain.Validators
{
    public class MyCourseProgressMeasureValidator : AbstractValidator<double?>
    {
        public MyCourseProgressMeasureValidator()
        {
            RuleFor(p => p.Value)
                .InclusiveBetween(0, 100)
                .When(p => p.HasValue);
        }
    }
}
