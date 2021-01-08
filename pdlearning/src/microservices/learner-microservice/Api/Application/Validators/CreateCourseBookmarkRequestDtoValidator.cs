using FluentValidation;
using Microservice.Learner.Application.Dtos;

namespace Microservice.Learner.Application.Validators
{
    public class CreateCourseBookmarkRequestDtoValidator : AbstractValidator<CreateCourseBookmarkRequestDto>
    {
        public CreateCourseBookmarkRequestDtoValidator()
        {
            RuleFor(p => p.CourseId).NotEmpty();
        }
    }
}
