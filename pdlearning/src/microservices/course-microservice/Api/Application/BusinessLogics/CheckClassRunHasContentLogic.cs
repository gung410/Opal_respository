using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microservice.Course.Application.BusinessLogics.Abstractions;
using Microservice.Course.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Thunder.Platform.Core.Context;
using Thunder.Platform.Core.Domain.Repositories;

namespace Microservice.Course.Application.BusinessLogics
{
    public class CheckClassRunHasContentLogic : BaseBusinessLogic
    {
        private readonly IReadOnlyRepository<Section> _readSectionRepository;
        private readonly IReadOnlyRepository<Lecture> _readLectureRepository;
        private readonly IReadOnlyRepository<Assignment> _readAssignmentRepository;

        public CheckClassRunHasContentLogic(
            IReadOnlyRepository<Section> readSectionRepository,
            IReadOnlyRepository<Lecture> readLectureRepository,
            IReadOnlyRepository<Assignment> readAssignmentRepository,
            IUserContext userContext) : base(userContext)
        {
            _readSectionRepository = readSectionRepository;
            _readLectureRepository = readLectureRepository;
            _readAssignmentRepository = readAssignmentRepository;
        }

        public async Task<Dictionary<Guid, bool>> ByClassRunIds(List<Guid> classRunIds, CancellationToken cancellationToken = default)
        {
            var lectures = _readLectureRepository
                .GetAll()
                .Where(p => p.ClassRunId != null && classRunIds.Contains(p.ClassRunId.Value))
                .Select(p => p.ClassRunId.Value);

            var sections = _readSectionRepository
                .GetAll()
                .Where(p => p.ClassRunId != null && classRunIds.Contains(p.ClassRunId.Value))
                .Select(p => p.ClassRunId.Value);

            var hasContentClassIds = await _readAssignmentRepository
                .GetAll()
                .Where(p => p.ClassRunId != null && classRunIds.Contains(p.ClassRunId.Value)).Select(p => p.ClassRunId.Value)
                .Union(lectures)
                .Union(sections)
                .Distinct()
                .ToListAsync(cancellationToken);

            var hasContentClassIdsSet = hasContentClassIds.ToHashSet();

            return classRunIds.ToDictionary(p => p, p => hasContentClassIdsSet.Contains(p));
        }
    }
}
