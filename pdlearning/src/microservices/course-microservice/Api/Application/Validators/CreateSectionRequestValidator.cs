using FluentValidation;
using Microservice.Course.Application.RequestDtos;
using Microservice.Course.Application.Validators.SharedValidators;

namespace Microservice.Course.Application.Validators
{
    public class CreateSectionRequestValidator : AbstractValidator<CreateOrUpdateSectionRequest>
    {
        public CreateSectionRequestValidator()
        {
            RuleFor(p => p.Data.Title)
                .SetValidator(new SectionTitleValidator());
        }
    }
}
