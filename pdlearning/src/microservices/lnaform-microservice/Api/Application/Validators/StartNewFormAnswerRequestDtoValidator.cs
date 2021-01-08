using FluentValidation;
using Microservice.LnaForm.Application.RequestDtos;
using Microservice.LnaForm.Application.Services;

namespace Microservice.LnaForm.Application.Validators
{
    public class StartNewFormAnswerRequestDtoValidator : AbstractValidator<SaveFormAnswerRequestDto>
    {
        public StartNewFormAnswerRequestDtoValidator()
        {
            RuleFor(p => p.FormId).NotEmpty();
        }
    }
}
