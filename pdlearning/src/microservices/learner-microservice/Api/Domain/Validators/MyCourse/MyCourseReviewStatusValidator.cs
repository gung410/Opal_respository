using FluentValidation;
using Microservice.Learner.Domain.Entities;

namespace Microservice.Learner.Domain.Validators
{
    public class MyCourseReviewStatusValidator : AbstractValidator<string>
    {
        public MyCourseReviewStatusValidator()
        {
            RuleFor(p => p).MaximumLength(MyCourse.MaxReviewStatusLength);
        }
    }
}
