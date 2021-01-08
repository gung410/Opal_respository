using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microservice.Form.Domain.ValueObjects.Form;
using Microsoft.EntityFrameworkCore;
using Thunder.Platform.Core.Domain.Repositories;
using Thunder.Platform.Core.Timing;
using FormEntity = Microservice.Form.Domain.Entities.Form;

namespace Microservice.Form.Application.SharedQueries
{
    public class GetFormsSharedQuery
    {
        private readonly IRepository<FormEntity> _formRepository;

        public GetFormsSharedQuery(IRepository<FormEntity> formRepository)
        {
            _formRepository = formRepository;
        }

        public async Task<List<Guid>> CanArchiveBecauseReachedArchiveDate(int batchSize = 10)
        {
            var forms = await _formRepository
                        .GetAll()
                        .Where(p => p.ArchiveDate.HasValue
                                 && p.ArchiveDate.Value.Date == Clock.Now.Date
                                 && p.Status != FormStatus.PendingApproval
                                 && p.Status != FormStatus.Published
                                 && p.Status != FormStatus.ReadyToUse
                                 && p.Status != FormStatus.Archived
                                 && p.IsArchived == false)
                        .Take(batchSize)
                        .Select(form => form.Id)
                        .ToListAsync();

            return forms;
        }

        public async Task<List<Guid>> CanArchiveBecauseNoUsedForLongTime(DateTime timeNotUsed)
        {
            var forms = await _formRepository
                    .GetAll()
                    .Where(p => p.ChangedDate.Value.Date <= timeNotUsed.Date &&
                                p.Status != FormStatus.Archived &&
                                p.IsSurveyTemplate != true &&
                                p.IsArchived == false)
                    .Select(p => p.Id)
                    .ToListAsync();

            return forms;
        }
    }
}
