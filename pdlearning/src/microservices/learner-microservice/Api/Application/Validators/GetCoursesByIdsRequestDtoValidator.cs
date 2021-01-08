using FluentValidation;
using Microservice.Learner.Application.Dtos;

namespace Microservice.Learner.Application.Validators
{
    public class GetCoursesByIdsRequestDtoValidator : AbstractValidator<GetCoursesByIdsRequestDto>
    {
        public GetCoursesByIdsRequestDtoValidator()
        {
            RuleFor(p => p.CourseIds).NotNull();
        }
    }
}
