using System;
using Microservice.BrokenLink.Application.RequestDtos;
using Thunder.Platform.Cqrs;

namespace Microservice.BrokenLink.Application.Commands
{
    public class SaveBrokenLinkReportCommand : BaseThunderCommand
    {
        public CreateBrokenLinkReportRequest CreationRequest { get; set; }

        public Guid Id { get; set; }

        public Guid ReportBy { get; set; }
    }
}
