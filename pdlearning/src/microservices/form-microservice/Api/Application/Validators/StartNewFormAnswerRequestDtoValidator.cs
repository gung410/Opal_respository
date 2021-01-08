using FluentValidation;
using Microservice.Form.Application.RequestDtos;
using Microservice.Form.Application.Services;

namespace Microservice.Form.Application.Validators
{
    public class StartNewFormAnswerRequestDtoValidator : AbstractValidator<SaveFormAnswerRequestDto>
    {
        public StartNewFormAnswerRequestDtoValidator()
        {
            RuleFor(p => p.FormId).NotEmpty();
        }
    }
}
