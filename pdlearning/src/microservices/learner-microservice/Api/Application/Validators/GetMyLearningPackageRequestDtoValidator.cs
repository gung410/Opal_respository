using FluentValidation;
using Microservice.Learner.Application.Dtos;

namespace Microservice.Learner.Application.Validators
{
    public class GetMyLearningPackageRequestDtoValidator : AbstractValidator<GetMyLearningPackageRequestDto>
    {
        public GetMyLearningPackageRequestDtoValidator()
        {
            RuleFor(p => p.MyLectureId)
                .NotEmpty()
                .When(p => !p.MyDigitalContentId.HasValue);
            RuleFor(p => p.MyLectureId)
                .Empty()
                .When(p => p.MyDigitalContentId.HasValue);
            RuleFor(p => p.MyDigitalContentId)
                .NotEmpty()
                .When(p => !p.MyLectureId.HasValue);
            RuleFor(p => p.MyDigitalContentId)
                .Empty()
                .When(p => p.MyLectureId.HasValue);
        }
    }
}
