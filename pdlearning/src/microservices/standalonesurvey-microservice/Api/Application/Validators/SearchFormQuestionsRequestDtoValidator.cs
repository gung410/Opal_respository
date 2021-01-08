using FluentValidation;
using Microservice.StandaloneSurvey.Application.RequestDtos;

namespace Microservice.StandaloneSurvey.Application.Validators
{
    public class SearchFormQuestionsRequestDtoValidator : AbstractValidator<SearchSurveyQuestionsRequestDto>
    {
        public SearchFormQuestionsRequestDtoValidator()
        {
            RuleFor(p => p.FormId).NotEmpty();
        }
    }
}
