using FluentValidation;
using Microservice.Learner.Application.Dtos;

namespace Microservice.Learner.Application.Validators
{
    public class CreateOrUpdateMyLearningPackageRequestDtoValidator : AbstractValidator<CreateOrUpdateMyLearningPackageRequestDto>
    {
        public CreateOrUpdateMyLearningPackageRequestDtoValidator()
        {
            RuleFor(p => p.Type).NotNull();
            RuleFor(p => p.MyLectureId)
                .NotEmpty()
                .When(p => !p.MyDigitalContentId.HasValue);
            RuleFor(p => p.MyDigitalContentId)
                .NotEmpty()
                .When(p => !p.MyLectureId.HasValue);
        }
    }
}
