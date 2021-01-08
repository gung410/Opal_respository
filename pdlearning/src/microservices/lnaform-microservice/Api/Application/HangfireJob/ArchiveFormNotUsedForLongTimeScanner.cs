using System;
using System.Linq;
using System.Threading.Tasks;
using System.Transactions;
using cx.datahub.scheduling.jobs.shared;
using Microservice.LnaForm.Application.Events;
using Microservice.LnaForm.Application.Models;
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
    public class ArchiveFormNotUsedForLongTimeScanner : BaseHangfireJob, ICheckReferenceForArchiveFormScanner
    {
        private const int BatchSize = 25;
        private readonly IUnitOfWorkManager _uowManager;
        private readonly IThunderCqrs _thunderCqrs;
        private readonly IRepository<FormEntity> _formRepository;
        private readonly ILogger<ArchiveFormNotUsedForLongTimeScanner> _logger;

        public ArchiveFormNotUsedForLongTimeScanner(
            IServiceProvider serviceProvider,
            IUnitOfWorkManager unitOfWorkManager,
            IThunderCqrs thunderCqrs,
            IRepository<FormEntity> formRepository,
            ILogger<ArchiveFormNotUsedForLongTimeScanner> logger) : base(serviceProvider)
        {
            _uowManager = unitOfWorkManager;
            _thunderCqrs = thunderCqrs;
            _formRepository = formRepository;
            _logger = logger;
        }

        protected override async Task InternalHandleAsync()
        {
            var oneYearBefore = Clock.Now.AddYears(-1);

            // Using only one transaction for whole the job to avoid increased DbContext.
            using (var uow = _uowManager.Begin(TransactionScopeOption.Suppress))
            {
               var itemsNeedCheckHasReference = await _formRepository
                    .GetAll()
                    .Where(p => p.ChangedDate.Value.Date <= oneYearBefore.Date &&
                                p.Status != FormStatus.Archived &&
                                p.IsArchived == false)
                    .Select(p => p.Id)
                    .ToListAsync();

               if (itemsNeedCheckHasReference.Count == 0)
               {
                    return;
               }

               for (int i = 0; i < itemsNeedCheckHasReference.Count; i += BatchSize)
               {
                    var listIds = itemsNeedCheckHasReference.Skip(i).Take(BatchSize).ToList();

                    await _thunderCqrs.SendEvent(new CheckHasReferenceToResourceEvent(
                           new CheckHasReferenceToResourceModel
                           {
                               ObjectIds = listIds,
                               ContentType = CourseReferencedContentType.Form
                           }));
               }

               _logger.LogInformation("[ArchiveFormNotUsedForLongTimeScanner] finished: {0}", Clock.Now.ToLongTimeString());
            }
        }
    }
}
