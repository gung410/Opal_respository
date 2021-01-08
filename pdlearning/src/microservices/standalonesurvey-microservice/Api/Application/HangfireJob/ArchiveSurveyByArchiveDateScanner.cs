using System;
using System.Linq;
using System.Threading.Tasks;
using System.Transactions;
using cx.datahub.scheduling.jobs.shared;
using Microservice.StandaloneSurvey.Application.Commands;
using Microservice.StandaloneSurvey.Domain.ValueObjects.Survey;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Thunder.Platform.Core.Domain.Repositories;
using Thunder.Platform.Core.Domain.UnitOfWork.Abstractions;
using Thunder.Platform.Core.Timing;
using Thunder.Platform.Cqrs;

namespace Microservice.StandaloneSurvey.Application.HangfireJob
{
    public class ArchiveSurveyByArchiveDateScanner : BaseHangfireJob, IArchiveFormScanner
    {
        private const int BatchSize = 10;
        private readonly IUnitOfWorkManager _uowManager;
        private readonly IThunderCqrs _thunderCqrs;
        private readonly IRepository<Domain.Entities.StandaloneSurvey> _formRepository;
        private readonly ILogger<ArchiveSurveyByArchiveDateScanner> _logger;

        public ArchiveSurveyByArchiveDateScanner(
            IServiceProvider serviceProvider,
            IUnitOfWorkManager unitOfWorkManager,
            IThunderCqrs thunderCqrs,
            IRepository<Domain.Entities.StandaloneSurvey> formRepository,
            ILogger<ArchiveSurveyByArchiveDateScanner> logger) : base(serviceProvider)
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
                                 && p.Status != SurveyStatus.Published
                                 && p.Status != SurveyStatus.Archived
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
                        var archiveFormCommand = new ArchiveSurveyCommand { FormId = id, ArchiveBy = Guid.Empty };

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
