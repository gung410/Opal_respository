using FluentValidation;
using Microservice.BrokenLink.Application.RequestDtos;

namespace Microservice.BrokenLink.Application.Validators
{
    public class CreateBrokenLinkReportRequestValidator : AbstractValidator<CreateBrokenLinkReportRequest>
    {
        public CreateBrokenLinkReportRequestValidator()
        {
            RuleFor(x => x.ObjectId).NotEmpty();
            RuleFor(x => x.Url).NotEmpty();
            RuleFor(x => x.ObjectDetailUrl).NotEmpty().MaximumLength(2000);
            RuleFor(x => x.ReporterName).NotEmpty();
            RuleFor(x => x.ObjectTitle).NotEmpty();
        }
    }
}
