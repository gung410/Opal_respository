using FluentValidation;
using Microservice.Course.Application.RequestDtos;

namespace Microservice.Course.Application.Validators
{
    public class SaveCourseRequestValidator : AbstractValidator<SaveCourseRequest>
    {
        public SaveCourseRequestValidator()
        {
        }
    }
}
