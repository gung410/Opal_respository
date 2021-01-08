using FluentValidation;
using FormEntity = Microservice.LnaForm.Domain.Entities.Form;

namespace Microservice.LnaForm.Domain.Validators.Form
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
