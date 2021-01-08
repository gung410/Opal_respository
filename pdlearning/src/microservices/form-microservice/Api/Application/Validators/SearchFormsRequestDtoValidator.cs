using FluentValidation;
using Microservice.Form.Application.RequestDtos;
using Microservice.Form.Application.Services;
using FormEntity = Microservice.Form.Domain.Entities.Form;

namespace Microservice.Form.Application.Validators
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
