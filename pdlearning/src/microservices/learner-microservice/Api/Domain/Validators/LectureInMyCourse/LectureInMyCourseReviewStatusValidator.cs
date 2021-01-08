using FluentValidation;

namespace Microservice.Learner.Domain.Validators
{
    public class LectureInMyCourseReviewStatusValidator : AbstractValidator<string>
    {
        public LectureInMyCourseReviewStatusValidator()
        {
            RuleFor(p => p).MaximumLength(1000);
        }
    }
}
