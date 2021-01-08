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
    public class CheckCourseHasContentLogic : BaseBusinessLogic
    {
        private readonly IReadOnlyRepository<Section> _readSectionRepository;
        private readonly IReadOnlyRepository<Lecture> _readLectureRepository;
        private readonly IReadOnlyRepository<Assignment> _readAssignmentRepository;

        public CheckCourseHasContentLogic(
            IReadOnlyRepository<Section> readSectionRepository,
            IReadOnlyRepository<Lecture> readLectureRepository,
            IReadOnlyRepository<Assignment> readAssignmentRepository,
            IUserContext userContext) : base(userContext)
        {
            _readSectionRepository = readSectionRepository;
            _readLectureRepository = readLectureRepository;
            _readAssignmentRepository = readAssignmentRepository;
        }

        public async Task<Dictionary<Guid, bool>> ByCourseIds(
           List<Guid> courseIds,
           CancellationToken cancellationToken = default)
        {
            var lectures = _readLectureRepository
                .GetAll()
                .Where(p => courseIds.Contains(p.CourseId))
                .Select(p => p.CourseId);

            var sections = _readSectionRepository
                .GetAll()
                .Where(p => courseIds.Contains(p.CourseId))
                .Select(p => p.CourseId);

            var hasContentCourseIds = await _readAssignmentRepository
                .GetAll()
                .Where(p => courseIds.Contains(p.CourseId))
                .Select(p => p.CourseId)
                .Union(lectures)
                .Union(sections)
                .Distinct()
                .ToListAsync(cancellationToken);

            var hasContentCourseIdsSet = hasContentCourseIds.ToHashSet();

            return courseIds.ToDictionary(p => p, p => hasContentCourseIdsSet.Contains(p));
        }
    }
}
