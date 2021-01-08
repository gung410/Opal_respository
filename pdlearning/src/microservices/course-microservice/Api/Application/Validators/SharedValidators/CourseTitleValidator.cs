using FluentValidation;
using Microservice.Course.Domain.Constants;

namespace Microservice.Course.Application.Validators.SharedValidators
{
    public class CourseTitleValidator : AbstractValidator<string>
    {
        public CourseTitleValidator()
        {
            RuleFor(p => p)
                .NotEmpty()
                .MaximumLength(EntitiesConstants.CourseNameLength);
        }
    }
}
