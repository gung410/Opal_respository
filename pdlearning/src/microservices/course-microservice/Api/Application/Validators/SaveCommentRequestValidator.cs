using FluentValidation;
using Microservice.Course.Application.RequestDtos;

namespace Microservice.Course.Application.Validators
{
    public class SaveCommentRequestValidator : AbstractValidator<CreateCommentRequest>
    {
        public SaveCommentRequestValidator()
        {
            RuleFor(p => p.Content)
                .NotEmpty();
        }
    }
}
