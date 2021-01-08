using FluentValidation;
using Microservice.Course.Application.RequestDtos;

namespace Microservice.Course.Application.Validators
{
    public class GetBlockoutDateDependenciesRequestValidator : AbstractValidator<GetBlockoutDateDependenciesRequest>
    {
        public GetBlockoutDateDependenciesRequestValidator()
        {
            // FromDate or ToDate, one of them must have value.
            RuleFor(p => p.FromDate).NotNull().When(p => !p.ToDate.HasValue).WithMessage("FromDate or ToDate must be not null.");
            RuleFor(p => p.ToDate).NotNull().When(p => !p.FromDate.HasValue).WithMessage("FromDate or ToDate must be not null.");

            When(p => p.FromDate.HasValue && p.ToDate.HasValue, () =>
            {
                RuleFor(p => p.ToDate).GreaterThan(p => p.FromDate).WithMessage("FromDate must be before ToDate.");
            });

            RuleFor(p => p.ServiceSchemes).NotNull().NotEmpty().WithMessage("Must be at least one ServiceScheme provided.");
        }
    }
}
