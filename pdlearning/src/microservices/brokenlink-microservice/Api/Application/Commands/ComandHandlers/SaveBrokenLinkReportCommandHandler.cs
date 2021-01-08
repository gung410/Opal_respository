using System;
using System.Threading;
using System.Threading.Tasks;
using Microservice.BrokenLink.Application.Services;
using Microservice.BrokenLink.Domain.Entities;
using Thunder.Platform.Core.Context;
using Thunder.Platform.Core.Domain.Repositories;
using Thunder.Platform.Core.Domain.UnitOfWork.Abstractions;
using Thunder.Platform.Cqrs;

namespace Microservice.BrokenLink.Application.Commands.CommandHandlers
{
    public class SaveBrokenLinkReportCommandHandler : BaseCommandHandler<SaveBrokenLinkReportCommand>
    {
        private readonly IRepository<BrokenLinkReport> _brokenLinkRepository;
        private readonly IBrokenLinkNotifier _brokenLinkNotifier;

        public SaveBrokenLinkReportCommandHandler(
            IRepository<BrokenLinkReport> brokenLinkRepository,
            IBrokenLinkNotifier brokenLinkNotifier,
            IUnitOfWorkManager unitOfWorkManager,
            IUserContext userContext) : base(unitOfWorkManager, userContext)
        {
            _brokenLinkRepository = brokenLinkRepository;
            _brokenLinkNotifier = brokenLinkNotifier;
        }

        protected override async Task HandleAsync(SaveBrokenLinkReportCommand command, CancellationToken cancellationToken)
        {
            var report = new BrokenLinkReport
            {
                Id = command.Id,
                ReportBy = command.ReportBy,
                ReporterName = command.CreationRequest.ReporterName,
                ObjectId = command.CreationRequest.ObjectId,
                OriginalObjectId = command.CreationRequest.OriginalObjectId,
                Url = command.CreationRequest.Url,
                Module = command.CreationRequest.Module,
                ObjectDetailUrl = command.CreationRequest.ObjectDetailUrl,
                ObjectOwnerId = command.CreationRequest.ObjectOwnerId,
                ObjectTitle = command.CreationRequest.ObjectTitle,
                ObjectOwnerName = command.CreationRequest.ObjectOwnerName,
                Description = command.CreationRequest.Description,
                IsSystemReport = false,
                ContentType = command.CreationRequest.ContentType
            };

            await _brokenLinkRepository.InsertAsync(report);

            await _brokenLinkNotifier.NotifyLearnerReport(new Models.BrokenLinkReportModel(report));
        }
    }
}
