using FluentValidation;
using Microservice.Course.Application.RequestDtos;
using Microservice.Course.Application.Validators.FilterCondition;
using Microservice.Course.Domain.Entities;

namespace Microservice.Course.Application.Validators
{
    public class SearchBlockoutDatesRequestValidator : AbstractValidator<SearchBlockoutDatesRequest>
    {
        public SearchBlockoutDatesRequestValidator(IValidFilterCriteria validFilterCriteria)
        {
            RuleFor(p => p.SearchText)
                .MaximumLength(100);
            RuleFor(p => p.Filter)
                .Must(filter => filter == null || filter.ValidateWith(nameof(BlockoutDate), validFilterCriteria))
                .WithMessage($"There are invalid property names in {nameof(SearchBlockoutDatesRequest.Filter.ContainFilters)} " +
                             $"or {nameof(SearchBlockoutDatesRequest.Filter.FromToFilters)}");
        }
    }
}
