using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microservice.Course.Application.SharedQueries.Abstractions;
using Microservice.Course.Common.Helpers;
using Microservice.Course.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Thunder.Platform.Core.Domain.Repositories;
using Thunder.Platform.Core.Extensions;

namespace Microservice.Course.Application.SharedQueries
{
    public class GetRemainingClassRunSlotSharedQuery : BaseSharedQuery
    {
        private readonly IReadOnlyRepository<ClassRun> _readClassRunRepository;
        private readonly IReadOnlyRepository<Registration> _readRegistrationRepository;

        public GetRemainingClassRunSlotSharedQuery(IReadOnlyRepository<ClassRun> readClassRunRepository, IReadOnlyRepository<Registration> readRegistrationRepository)
        {
            _readClassRunRepository = readClassRunRepository;
            _readRegistrationRepository = readRegistrationRepository;
        }

        public async Task<Dictionary<Guid, int>> ByClassRunIds(
            IEnumerable<Guid> classRunIds,
            CancellationToken cancellationToken = default)
        {
            return await ByClassRunIds(classRunIds, null, cancellationToken);
        }

        public async Task<Dictionary<Guid, int>> ByClassRunIds(
            IEnumerable<Guid> classRunIds,
            List<Registration> willBeRemovedParticipants,
            CancellationToken cancellationToken = default)
        {
            classRunIds = classRunIds.Distinct().ToList();
            var classRunMaxSizes = await _readClassRunRepository
                .GetAll()
                .Where(x => classRunIds.Contains(x.Id))
                .Select(x => new { ClassRunId = x.Id, x.MaxClassSize })
                .ToListAsync(cancellationToken);

            var totalParticipants = await _readRegistrationRepository
                .GetAll()
                .Where(x => classRunIds.Contains(x.ClassRunId))
                .Where(Registration.IsSlotTakingExpr())
                .Select(x => new { x.ClassRunId, x.UserId })
                .Distinct()
                .GroupBy(x => x.ClassRunId)
                .Select(x => new { ClassRunId = x.Key, TotalParticipant = x.Count() })
                .ToListAsync(cancellationToken);

            var remainingSlotDict =
                (from x in classRunMaxSizes
                 join y in totalParticipants on x.ClassRunId equals y.ClassRunId into total
                 from m in total.DefaultIfEmpty()
                 select new
                 {
                     x.ClassRunId,
                     RemainingSlot = x.MaxClassSize - (m?.TotalParticipant ?? 0)
                 }).ToDictionary(x => x.ClassRunId, x => x.RemainingSlot);

            // NOTE: Update remainingSlotDict when we have removed participants.
            var classRunToWillBeRemovedParticipantsCountDict = willBeRemovedParticipants?.GroupBy(x => x.ClassRunId).ToDictionary(p => p.Key, p => p.Count());
            if (classRunToWillBeRemovedParticipantsCountDict != null)
            {
                remainingSlotDict = remainingSlotDict.ToDictionary(
                    p => p.Key,
                    p =>
                    {
                        var willBeRemovedParticipantsCount = classRunToWillBeRemovedParticipantsCountDict.ContainsKey(p.Key) ? classRunToWillBeRemovedParticipantsCountDict[p.Key] : 0;
                        return p.Value + willBeRemovedParticipantsCount;
                    });
            }

            return remainingSlotDict;
        }

        public async Task<int> ByClassRunId(
            Guid classRunId,
            CancellationToken cancellationToken = default)
        {
            var remainSlotDict = await ByClassRunIds(F.List(classRunId), cancellationToken);
            return remainSlotDict.GetOrDefault(classRunId);
        }
    }
}
