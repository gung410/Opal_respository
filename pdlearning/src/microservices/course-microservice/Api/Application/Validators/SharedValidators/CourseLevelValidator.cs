using FluentValidation;
using Microservice.Course.Domain.Constants;

namespace Microservice.Course.Application.Validators.SharedValidators
{
    public class CourseLevelValidator : AbstractValidator<string>
    {
        public CourseLevelValidator()
        {
            RuleFor(p => p)
                .NotEmpty()
                .MaximumLength(EntitiesConstants.CourseLevelLength);
        }
    }
}
