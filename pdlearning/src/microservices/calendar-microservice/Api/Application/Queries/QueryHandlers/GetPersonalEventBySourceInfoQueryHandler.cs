using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microservice.Calendar.Application.Models;
using Microservice.Calendar.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Thunder.Platform.Core.Domain.Repositories;
using Thunder.Platform.Cqrs;

namespace Microservice.Calendar.Application.Queries
{
    public class GetPersonalEventBySourceInfoQueryHandler : BaseThunderQueryHandler<GetPersonalEventBySourceInfoQuery, PersonalEventModel>
    {
        private readonly IRepository<PersonalEvent> _personalEventRepository;

        public GetPersonalEventBySourceInfoQueryHandler(
            IRepository<PersonalEvent> personalEventRepository)
        {
            _personalEventRepository = personalEventRepository;
        }

        protected override Task<PersonalEventModel> HandleAsync(GetPersonalEventBySourceInfoQuery query, CancellationToken cancellationToken)
        {
            return _personalEventRepository
                .GetAll()
                .Where(p => p.Source == query.Source && p.SourceId == query.SourceId)
                .Select(p => new PersonalEventModel
                {
                    Id = p.Id,
                    Title = p.Title,
                    CreatedBy = p.CreatedBy,
                    StartAt = p.StartAt,
                    EndAt = p.EndAt,
                    IsAllDay = p.IsAllDay,
                    Source = p.Source,
                    SourceId = p.SourceId,
                    RepeatFrequency = p.RepeatFrequency,
                    RepeatUntil = p.RepeatUntil
                })
                .FirstOrDefaultAsync(cancellationToken);
        }
    }
}
