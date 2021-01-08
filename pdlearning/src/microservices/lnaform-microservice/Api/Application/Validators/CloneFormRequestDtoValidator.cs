using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentValidation;
using Microservice.LnaForm.Application.RequestDtos;
using Microservice.LnaForm.Application.Services;
using Microservice.LnaForm.Domain.Validators.Form;

namespace Microservice.LnaForm.Application.Validators
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
