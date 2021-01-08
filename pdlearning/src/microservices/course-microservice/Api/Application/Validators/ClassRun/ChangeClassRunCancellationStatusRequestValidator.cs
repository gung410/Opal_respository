using FluentValidation;
using Microservice.Course.Application.RequestDtos;

namespace Microservice.Course.Application.Validators.ClassRun
{
    public class ChangeClassRunCancellationStatusRequestValidator : AbstractValidator<ChangeClassRunCancellationStatusRequest>
    {
        public ChangeClassRunCancellationStatusRequestValidator()
        {
            RuleFor(x => x.Ids).NotNull();
        }
    }
}
