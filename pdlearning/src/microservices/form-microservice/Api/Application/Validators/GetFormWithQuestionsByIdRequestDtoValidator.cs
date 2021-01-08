using FluentValidation;
using Microservice.Form.Application.RequestDtos;
using Microservice.Form.Application.Services;

namespace Microservice.Form.Application.Validators
{
    public class GetFormWithQuestionsByIdRequestDtoValidator : AbstractValidator<GetFormWithQuestionsByIdRequestDto>
    {
        public GetFormWithQuestionsByIdRequestDtoValidator()
        {
            RuleFor(p => p.FormId).NotEmpty();
        }
    }
}
