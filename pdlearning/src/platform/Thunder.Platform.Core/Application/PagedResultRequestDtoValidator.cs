using FluentValidation;
using Thunder.Platform.Core.Application.Dtos;

namespace Thunder.Platform.Core.Application
{
    public class PagedResultRequestDtoValidator : AbstractValidator<PagedResultRequestDto>
    {
        public PagedResultRequestDtoValidator()
        {
            RuleFor(p => p.MaxResultCount)
                .InclusiveBetween(0, int.MaxValue);

            RuleFor(p => p.SkipCount)
                .InclusiveBetween(0, int.MaxValue);
        }
    }
}
