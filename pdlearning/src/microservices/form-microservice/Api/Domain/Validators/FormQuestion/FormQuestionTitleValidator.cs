using FluentValidation;
using Microservice.Form.Domain.Constants;

namespace Microservice.Form.Domain.Validators.FormQuestion
{
    public class FormQuestionTitleValidator : AbstractValidator<string>
    {
        public FormQuestionTitleValidator()
        {
            RuleFor(p => p).MaximumLength(DomainConstants.DefaultStringMaxLength);
        }
    }
}
