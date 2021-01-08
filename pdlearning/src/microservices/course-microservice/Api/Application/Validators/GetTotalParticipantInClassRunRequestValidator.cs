using FluentValidation;
using Microservice.Course.Application.RequestDtos;

namespace Microservice.Course.Application.Validators
{
    public class GetTotalParticipantInClassRunRequestValidator : AbstractValidator<GetTotalParticipantInClassRunRequest>
    {
        public GetTotalParticipantInClassRunRequestValidator()
        {
            RuleFor(p => p.ClassRunIds).NotNull().NotEmpty().WithMessage("ClassRunIds can not be null or empty");
        }
    }
}
