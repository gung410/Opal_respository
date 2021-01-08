using FluentValidation;
using Microservice.Learner.Domain.Entities;

namespace Microservice.Learner.Domain.Validators
{
    public class MyCourseVersionValidator : AbstractValidator<string>
    {
        public MyCourseVersionValidator()
        {
            RuleFor(p => p).MaximumLength(MyCourse.MaxVersionLength);
        }
    }
}
