using FluentValidation;
using Microservice.Course.Application.RequestDtos;
using Microservice.Course.Application.Validators.FilterCondition;
using Microservice.Course.Domain.Entities;

namespace Microservice.Course.Application.Validators
{
    public class SearchAnnouncementRequestValidator : AbstractValidator<SearchAnnouncementRequest>
    {
        public SearchAnnouncementRequestValidator(IValidFilterCriteria validFilterCriteria)
        {
            RuleFor(p => p.Filter)
                .Must(filter => filter == null || filter.ValidateWith(nameof(Announcement), validFilterCriteria))
                .WithMessage($"There are invalid property names in {nameof(SearchAnnouncementRequest.Filter.ContainFilters)} " +
                             $"or {nameof(SearchAnnouncementRequest.Filter.FromToFilters)}");
        }
    }
}
