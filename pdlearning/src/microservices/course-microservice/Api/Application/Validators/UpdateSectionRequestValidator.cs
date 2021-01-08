using FluentValidation;
using Microservice.Course.Application.RequestDtos;
using Microservice.Course.Application.Validators.SharedValidators;

namespace Microservice.Course.Application.Validators
{
    public class UpdateSectionRequestValidator : AbstractValidator<UpdateSectionRequest>
    {
        public UpdateSectionRequestValidator()
        {
            RuleFor(p => p.Title)
                .SetValidator(new SectionTitleValidator());
        }
    }
}
