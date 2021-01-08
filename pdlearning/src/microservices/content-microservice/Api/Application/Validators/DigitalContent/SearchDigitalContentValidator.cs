using FluentValidation;
using Microservice.Content.Application.RequestDtos;

namespace Microservice.Content.Application.Validators
{
    public class SearchDigitalContentValidator : AbstractValidator<SearchDigitalContentRequest>
    {
        public SearchDigitalContentValidator()
        {
            RuleFor(p => p.SortField).SetValidator(new SortFieldValidator()).When(p => p.SortField != null);
        }
    }
}
