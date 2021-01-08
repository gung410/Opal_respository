using FluentValidation;
using Microservice.Course.Domain.Constants;

namespace Microservice.Course.Application.Validators.SharedValidators
{
    public class SectionTitleValidator : AbstractValidator<string>
    {
        public SectionTitleValidator()
        {
            RuleFor(p => p)
                .NotEmpty()
                .MaximumLength(EntitiesConstants.CourseNameLength);
        }
    }
}
