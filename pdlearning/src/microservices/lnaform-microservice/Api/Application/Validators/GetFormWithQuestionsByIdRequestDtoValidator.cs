using FluentValidation;
using Microservice.LnaForm.Application.RequestDtos;
using Microservice.LnaForm.Application.Services;

namespace Microservice.LnaForm.Application.Validators
{
    public class GetFormWithQuestionsByIdRequestDtoValidator : AbstractValidator<GetFormWithQuestionsByIdRequestDto>
    {
        public GetFormWithQuestionsByIdRequestDtoValidator()
        {
            RuleFor(p => p.FormId).NotEmpty();
        }
    }
}
