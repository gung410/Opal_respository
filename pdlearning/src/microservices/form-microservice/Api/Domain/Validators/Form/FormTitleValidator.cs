using FluentValidation;
using FormEntity = Microservice.Form.Domain.Entities.Form;

namespace Microservice.Form.Domain.Validators.Form
{
    public class FormTitleValidator : AbstractValidator<string>
    {
        public FormTitleValidator()
        {
            RuleFor(p => p)
                .NotEmpty()
                .MaximumLength(FormEntity.MaxTitleLength);
        }
    }
}
