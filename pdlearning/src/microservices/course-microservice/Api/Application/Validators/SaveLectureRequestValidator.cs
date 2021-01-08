using FluentValidation;
using Microservice.Course.Application.RequestDtos;
using Microservice.Course.Application.Validators.SharedValidators;

namespace Microservice.Course.Application.Validators
{
    public class SaveLectureRequestValidator : AbstractValidator<SaveLectureRequest>
    {
        public SaveLectureRequestValidator()
        {
            RuleFor(p => p.LectureName)
                .SetValidator(new LectureNameValidator());

            RuleFor(p => p.Type)
                .IsInEnum();

            RuleFor(p => p.MimeType)
                .SetValidator(new LectureMimeTypeValidator());
        }
    }
}
