using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentValidation;
using Microservice.Form.Application.RequestDtos;
using Microservice.Form.Application.Services;
using Microservice.Form.Domain.Validators.Form;

namespace Microservice.Form.Application.Validators
{
    public class CloneFormRequestDtoValidator : AbstractValidator<CloneFormRequestDto>
    {
        public CloneFormRequestDtoValidator()
        {
            RuleFor(p => p.NewTitle)
                .NotNull()
                .SetValidator(new FormTitleValidator());
        }
    }
}
