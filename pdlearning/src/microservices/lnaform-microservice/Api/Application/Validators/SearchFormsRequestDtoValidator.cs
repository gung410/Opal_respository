using FluentValidation;
using Microservice.LnaForm.Application.RequestDtos;
using Microservice.LnaForm.Application.Services;
using FormEntity = Microservice.LnaForm.Domain.Entities.Form;

namespace Microservice.LnaForm.Application.Validators
{
    public class SearchFormsRequestDtoValidator : AbstractValidator<SearchFormsRequestDto>
    {
        public SearchFormsRequestDtoValidator()
        {
            RuleFor(p => p.SearchFormTitle)
                .MaximumLength(FormEntity.MaxTitleLength)
                .When(p => p.SearchFormTitle != null);
        }
    }
}
