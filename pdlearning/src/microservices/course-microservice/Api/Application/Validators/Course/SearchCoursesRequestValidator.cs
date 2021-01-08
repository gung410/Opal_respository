using FluentValidation;
using Microservice.Course.Application.RequestDtos;
using Microservice.Course.Application.Validators.FilterCondition;
using Microservice.Course.Domain.Entities;

namespace Microservice.Course.Application.Validators
{
    public class SearchCoursesRequestValidator : AbstractValidator<SearchCoursesRequest>
    {
        public SearchCoursesRequestValidator(IValidFilterCriteria validFilterCriteria)
        {
            RuleFor(p => p.SearchText)
                .MaximumLength(100);
            RuleFor(p => p.Filter)
                .Must(filter => filter == null || filter.ValidateWith(nameof(CourseEntity), validFilterCriteria))
                .WithMessage($"There are invalid property names in {nameof(SearchCoursesRequest.Filter.ContainFilters)} " +
                             $"or {nameof(SearchCoursesRequest.Filter.FromToFilters)}");
        }
    }
}
