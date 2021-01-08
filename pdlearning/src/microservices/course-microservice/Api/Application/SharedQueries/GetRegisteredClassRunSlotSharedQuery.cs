using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microservice.Course.Application.SharedQueries.Abstractions;
using Microservice.Course.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Thunder.Platform.Core.Domain.Repositories;
using Thunder.Platform.Core.Extensions;

namespace Microservice.Course.Application.SharedQueries
{
    public class GetRegisteredClassRunSlotSharedQuery : BaseSharedQuery
    {
        private readonly IReadOnlyRepository<Registration> _readRegistrationRepository;
        private readonly IReadOnlyRepository<ClassRun> _readClassrunRepository;

        public GetRegisteredClassRunSlotSharedQuery(
            IReadOnlyRepository<Registration> readRegistrationRepository,
            IReadOnlyRepository<ClassRun> readClassrunRepository)
        {
            _readRegistrationRepository = readRegistrationRepository;
            _readClassrunRepository = readClassrunRepository;
        }

        public async Task<Dictionary<Guid, int>> CountByClassRunIds(
            List<Guid> classRunIds,
            List<Registration> willBeAddedAsParticipants = null,
            bool countParticipantOnly = false,
            CancellationToken cancellationToken = default)
        {
            return await CountByClassRunQuery(
                _readClassrunRepository.GetAll().Where(p => classRunIds.Contains(p.Id)),
                willBeAddedAsParticipants,
                countParticipantOnly,
                cancellationToken);
        }

        public async Task<Dictionary<Guid, int>> CountByClassRunQuery(
            IQueryable<ClassRun> classRunQuery,
            List<Registration> willBeAddedAsParticipants = null,
            bool countParticipantOnly = false,
            CancellationToken cancellationToken = default)
        {
            var registeredCountByClassRunList = await _readRegistrationRepository
                .GetAll()
                .Join(classRunQuery, p => p.ClassRunId, p => p.Id, (registration, classRun) => registration)
                .Where(Registration.IsSlotTakingExpr())
                .WhereIf(countParticipantOnly, Registration.IsParticipantExpr())
                .GroupBy(p => p.ClassRunId)
                .Select(x => new { ClassRunId = x.Key, Count = x.Count() })
                .ToListAsync(cancellationToken);

            var registeredCountByClassRunDic = registeredCountByClassRunList.ToDictionary(p => p.ClassRunId, p => p.Count);
            if (willBeAddedAsParticipants != null)
            {
                var willBeAddedAsParticipantsCountByClassRun = willBeAddedAsParticipants
                    .GroupBy(p => p.ClassRunId)
                    .ToDictionary(p => p.Key, p => p.Count());
                registeredCountByClassRunDic = registeredCountByClassRunDic.ToDictionary(p => p.Key, p =>
                {
                    var willBeAddedAsParticipantsCount = willBeAddedAsParticipantsCountByClassRun.ContainsKey(p.Key) ? willBeAddedAsParticipantsCountByClassRun[p.Key] : 0;
                    return p.Value + willBeAddedAsParticipantsCount;
                });
            }

            return await classRunQuery
                .Select(p => p.Id)
                .ToDictionaryAsync(
                    p => p,
                    classrunId => registeredCountByClassRunDic.ContainsKey(classrunId) ? registeredCountByClassRunDic[classrunId] : 0);
        }
    }
}
