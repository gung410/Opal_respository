using System;
using System.Threading.Tasks;
using Microservice.BrokenLink.Application.Commands;
using Microservice.BrokenLink.Application.Models;
using Microservice.BrokenLink.Application.Queries;
using Microservice.BrokenLink.Application.RequestDtos;
using Thunder.Platform.Core.Application;
using Thunder.Platform.Core.Application.Dtos;
using Thunder.Platform.Cqrs;

namespace Microservice.BrokenLink.Application.Services
{
    public class BrokenLinkReporter : ApplicationService, IBrokenLinkReporter
    {
        private readonly IThunderCqrs _thunderCqrs;

        public BrokenLinkReporter(IThunderCqrs thunderCqrs)
        {
            _thunderCqrs = thunderCqrs;
        }

        public Task ReportBrokenLink(CreateBrokenLinkReportRequest request, Guid userId)
        {
            var saveCommand = new SaveBrokenLinkReportCommand
            {
                CreationRequest = request,
                ReportBy = userId,
                Id = Guid.NewGuid()
            };

            return _thunderCqrs.SendCommand(saveCommand);
        }

        public Task<PagedResultDto<BrokenLinkReportModel>> SearchBrokenLinkReport(SearchBrokenLinkReportRequest dto)
        {
            var searchQuery = new SearchBrokenLinkReportQuery
            {
                Request = dto
            };

            return _thunderCqrs.SendQuery(searchQuery);
        }
    }
}
