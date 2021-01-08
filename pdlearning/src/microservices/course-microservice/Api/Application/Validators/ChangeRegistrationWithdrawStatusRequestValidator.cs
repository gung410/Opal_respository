using FluentValidation;
using Microservice.Course.Application.RequestDtos;

namespace Microservice.Course.Application.Validators
{
    public class ChangeRegistrationWithdrawStatusRequestValidator : AbstractValidator<ChangeRegistrationWithdrawStatusRequest>
    {
        public ChangeRegistrationWithdrawStatusRequestValidator()
        {
            RuleFor(p => p.Ids)
                .NotNull();
        }
    }
}
