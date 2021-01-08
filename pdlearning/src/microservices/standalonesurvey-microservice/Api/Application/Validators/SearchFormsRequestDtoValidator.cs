using FluentValidation;
using Microservice.StandaloneSurvey.Application.RequestDtos;

namespace Microservice.StandaloneSurvey.Application.Validators
{
    public class SearchFormsRequestDtoValidator : AbstractValidator<SearchSurveysRequestDto>
    {
        public SearchFormsRequestDtoValidator()
        {
            RuleFor(p => p.SearchFormTitle)
                .MaximumLength(Domain.Entities.StandaloneSurvey.MaxTitleLength)
                .When(p => p.SearchFormTitle != null);
        }
    }
}
