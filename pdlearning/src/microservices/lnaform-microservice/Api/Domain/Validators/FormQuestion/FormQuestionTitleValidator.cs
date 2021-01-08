using FluentValidation;
using Microservice.LnaForm.Domain.Constants;

namespace Microservice.LnaForm.Domain.Validators.FormQuestion
{
    public class FormQuestionTitleValidator : AbstractValidator<string>
    {
        public FormQuestionTitleValidator()
        {
            RuleFor(p => p).MaximumLength(DomainConstants.DefaultStringMaxLength);
        }
    }
}
