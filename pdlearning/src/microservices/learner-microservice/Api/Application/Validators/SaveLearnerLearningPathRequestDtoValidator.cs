using System.Linq;
using FluentValidation;
using Microservice.Learner.Application.Dtos;

namespace Microservice.Learner.Application.Validators
{
    public class SaveLearnerLearningPathRequestDtoValidator : AbstractValidator<SaveLearnerLearningPathRequestDto>
    {
        public SaveLearnerLearningPathRequestDtoValidator()
        {
            RuleFor(p => p.Title).NotEmpty();
            RuleFor(p => p.Title).MaximumLength(2000);
            RuleFor(p => p.Courses).Must(c => c.Count > 0);
            RuleFor(p => p).Must(p => AreCoursesDuplicated(p) == false);
        }

        private bool AreCoursesDuplicated(SaveLearnerLearningPathRequestDto dto)
        {
            return dto.Courses.GroupBy(c => c.CourseId).Any(c => c.Count() > 1);
        }
    }
}
