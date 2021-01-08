using FluentValidation;
using Microservice.LnaForm.Application.RequestDtos;
using Microservice.LnaForm.Application.Services;

namespace Microservice.LnaForm.Application.Validators
{
    public class SearchFormQuestionsRequestDtoValidator : AbstractValidator<SearchFormQuestionsRequestDto>
    {
        public SearchFormQuestionsRequestDtoValidator()
        {
            RuleFor(p => p.FormId).NotEmpty();
        }
    }
}
