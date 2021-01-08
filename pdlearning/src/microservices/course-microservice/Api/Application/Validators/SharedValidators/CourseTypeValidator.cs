using FluentValidation;
using Microservice.Course.Domain.Constants;

namespace Microservice.Course.Application.Validators.SharedValidators
{
    public class CourseTypeValidator : AbstractValidator<string>
    {
        public CourseTypeValidator()
        {
            RuleFor(p => p)
                .NotEmpty()
                .MaximumLength(EntitiesConstants.CourseCourseTypeLength);
        }
    }
}
