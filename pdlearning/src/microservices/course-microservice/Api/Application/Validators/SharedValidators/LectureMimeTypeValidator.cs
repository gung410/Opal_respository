using FluentValidation;
using Microservice.Course.Domain.Constants;

namespace Microservice.Course.Application.Validators.SharedValidators
{
    public class LectureMimeTypeValidator : AbstractValidator<string>
    {
        public LectureMimeTypeValidator()
        {
            RuleFor(p => p)
                .NotEmpty()
                .MaximumLength(EntitiesConstants.LectureMimeTypeLength);
        }
    }
}
