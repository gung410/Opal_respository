using System.Linq;
using FluentValidation;
using Microservice.Learner.Application.Dtos;

namespace Microservice.Learner.Application.Validators
{
    public class EnrollCourseRequestDtoValidator : AbstractValidator<EnrollCourseRequestDto>
    {
        public EnrollCourseRequestDtoValidator()
        {
            RuleFor(p => p.CourseId).NotEmpty();
        }
    }
}
