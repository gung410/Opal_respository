using FluentValidation;
using Microservice.Course.Application.RequestDtos;
using Microservice.Course.Application.Validators.FilterCondition;
using Microservice.Course.Domain.Entities;

namespace Microservice.Course.Application.Validators
{
    public class SearchRegistrationRequestValidator : AbstractValidator<SearchRegistrationRequest>
    {
        public SearchRegistrationRequestValidator(IValidFilterCriteria validFilterCriteria)
        {
            RuleFor(p => p.SearchText)
                .MaximumLength(100);
            RuleFor(p => p.Filter)
                    .Must(filter => filter == null || filter.ValidateWith(nameof(Registration), validFilterCriteria))
                    .WithMessage($"There are invalid property names in {nameof(SearchRegistrationRequest.Filter.ContainFilters)} " +
                                 $"or {nameof(SearchRegistrationRequest.Filter.FromToFilters)}");
        }
    }
}
