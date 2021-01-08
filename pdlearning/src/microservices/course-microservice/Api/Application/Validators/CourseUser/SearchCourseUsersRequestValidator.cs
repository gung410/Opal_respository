using FluentValidation;
using Microservice.Course.Application.RequestDtos;

namespace Microservice.Course.Application.Validators
{
    public class SearchCourseUsersRequestValidator : AbstractValidator<SearchCourseUsersRequest>
    {
        public SearchCourseUsersRequestValidator()
        {
            RuleFor(p => p.SearchText)
                .MaximumLength(100);
        }
    }
}
