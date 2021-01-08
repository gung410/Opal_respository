using FluentValidation;
using Microservice.Form.Application.RequestDtos;
using Microservice.Form.Application.Services;

namespace Microservice.Form.Application.Validators
{
    public class SearchFormQuestionsRequestDtoValidator : AbstractValidator<SearchFormQuestionsRequestDto>
    {
        public SearchFormQuestionsRequestDtoValidator()
        {
            RuleFor(p => p.FormId).NotEmpty();
        }
    }
}
