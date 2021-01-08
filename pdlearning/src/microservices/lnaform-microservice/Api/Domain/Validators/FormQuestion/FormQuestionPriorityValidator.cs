using FluentValidation;

namespace Microservice.LnaForm.Domain.Validators.FormQuestion
{
    public class FormQuestionPriorityValidator : AbstractValidator<int>
    {
        public FormQuestionPriorityValidator()
        {
            RuleFor(p => p).GreaterThanOrEqualTo(0);
        }
    }
}
