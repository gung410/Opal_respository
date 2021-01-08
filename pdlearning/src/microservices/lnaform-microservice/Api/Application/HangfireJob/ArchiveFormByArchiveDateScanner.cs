using System;
using System.Linq;
using System.Threading.Tasks;
using System.Transactions;
using cx.datahub.scheduling.jobs.shared;
using Microservice.LnaForm.Application.Commands;
using Microservice.LnaForm.Domain.ValueObjects.Form;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Thunder.Platform.Core.Domain.Repositories;
using Thunder.Platform.Core.Domain.UnitOfWork.Abstractions;
using Thunder.Platform.Core.Timing;
using Thunder.Platform.Cqrs;
using FormEntity = Microservice.LnaForm.Domain.Entities.Form;

namespace Microservice.LnaForm.Application.HangfireJob
{
    public class ArchiveFormByArchiveDateScanner : BaseHangfireJob, IArchiveFormScanner
    {
        private const int BatchSize = 10;
        private readonly IUnitOfWorkManager _uowManager;
        private readonly IThunderCqrs _thunderCqrs;
        private readonly IRepository<FormEntity> _formRepository;
        private readonly ILogger<ArchiveFormByArchiveDateScanner> _logger;

        public ArchiveFormByArchiveDateScanner(
            IServiceProvider serviceProvider,
            IUnitOfWorkManager unitOfWorkManager,
            IThunderCqrs thunderCqrs,
            IRepository<FormEntity> formRepository,
            ILogger<ArchiveFormByArchiveDateScanner> logger) : base(serviceProvider)
        {
            _uowManager = unitOfWorkManager;
            _thunderCqrs = thunderCqrs;
            _formRepository = formRepository;
            _logger = logger;
        }

        protected override async Task InternalHandleAsync()
        {
            bool continueToScan = true;

            // Using only one transaction for whole the job to avoid increased DbContext.
            using (var uow = _uowManager.Begin(TransactionScopeOption.Suppress))
            {
                while (continueToScan)
                {
                    var formsToArchive = await _formRepository
                        .GetAll()
                        .Where(p => p.ArchiveDate.HasValue
                                 && p.ArchiveDate.Value.Date == Clock.Now.Date
                                 && p.Status != FormStatus.Published
                                 && p.Status != FormStatus.Archived
                                 && p.IsArchived == false)
                        .Take(BatchSize)
                        .Select(p => p.Id)
                        .ToListAsync();

                    if (formsToArchive.Count == 0)
                    {
                        break;
                    }

                    foreach (var id in formsToArchive)
                    {
                        var archiveFormCommand = new ArchiveFormCommand { FormId = id, ArchiveBy = Guid.Empty };

                        await _thunderCqrs.SendCommand(archiveFormCommand);
                    }

                    continueToScan = formsToArchive.Count == BatchSize;
                }

                _logger.LogInformation("[ArchiveFormByArchiveDateScanner] finished: {0}", Clock.Now.ToLongTimeString());

                // No need to complete due to read only.
                // await uow.CompleteAsync();
            }
        }
    }
}
