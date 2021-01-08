using FluentValidation;
using Microservice.Course.Domain.Constants;

namespace Microservice.Course.Application.Validators.SharedValidators
{
    public class LectureNameValidator : AbstractValidator<string>
    {
        public LectureNameValidator()
        {
            RuleFor(p => p)
                .NotEmpty()
                .MaximumLength(EntitiesConstants.LectureNameLength);
        }
    }
}
