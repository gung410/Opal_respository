using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using Microservice.Course.Application.AggregatedEntityModels;
using Microservice.Course.Application.SharedQueries.Abstractions;
using Microservice.Course.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Thunder.Platform.Core.Domain.Repositories;

namespace Microservice.Course.Application.SharedQueries
{
    public class GetAggregatedClassRunSharedQuery : BaseSharedQuery
    {
        private readonly IReadOnlyRepository<CourseEntity> _readCourseRepository;
        private readonly IReadOnlyRepository<ClassRun> _readClassrunRepository;
        private readonly IReadOnlyRepository<Session> _readSessionRepository;

        public GetAggregatedClassRunSharedQuery(
            IReadOnlyRepository<CourseEntity> readCourseRepository,
            IReadOnlyRepository<ClassRun> readClassrunRepository,
            IReadOnlyRepository<Session> readSessionRepository)
        {
            _readClassrunRepository = readClassrunRepository;
            _readSessionRepository = readSessionRepository;
            _readCourseRepository = readCourseRepository;
        }

        public async Task<ClassRunAggregatedEntityModel> ByClassRunId(Guid classRunId)
        {
            var classrun = await _readClassrunRepository.GetAsync(classRunId);
            var course = await _readCourseRepository.GetAsync(classrun.CourseId);
            var sessions = await _readSessionRepository.GetAllListAsync(p => p.ClassRunId == classRunId);
            return ClassRunAggregatedEntityModel.Create(classrun, course, sessions);
        }

        public async Task<List<ClassRunAggregatedEntityModel>> ByCourseIds(List<Guid> courseIds, bool withSessions, CancellationToken cancellationToken = default)
        {
            var distinctCourseIds = courseIds.Distinct().ToList();
            var items = await _readClassrunRepository
                .GetAll()
                .Where(p => distinctCourseIds.Contains(p.CourseId))
                .Join(_readCourseRepository.GetAll(), p => p.CourseId, p => p.Id, (classRun, course) => new { course, classRun })
                .Select(p => ClassRunAggregatedEntityModel.Create(p.classRun, p.course))
                .ToListAsync(cancellationToken);

            if (withSessions)
            {
                var allClassRunIds = items.Select(p => p.ClassRun.Id);
                var itemSessions = (await _readSessionRepository
                    .GetAll()
                    .Where(p => allClassRunIds.Contains(p.ClassRunId))
                    .ToListAsync(cancellationToken))
                    .GroupBy(p => p.ClassRunId)
                    .ToDictionary(p => p.Key, p => p.ToList());
                items = items
                    .Select(p => p.WithSessions(itemSessions.GetValueOrDefault(p.ClassRun.Id, new List<Session>())))
                    .ToList();
                return items;
            }

            return items;
        }

        public Task<List<ClassRunAggregatedEntityModel>> ByClassRunIds(List<Guid> classRunIds, bool withSessions, CancellationToken cancellationToken = default)
        {
            return ByQuery(
                _readClassrunRepository.GetAll().Where(p => classRunIds.Distinct().Contains(p.Id)),
                withSessions,
                cancellationToken);
        }

        public async Task<List<ClassRunAggregatedEntityModel>> ByQuery(IQueryable<ClassRun> query, bool withSessions, CancellationToken cancellationToken = default)
        {
            var classruns = await query.ToListAsync(cancellationToken);
            var courseIds = classruns.Select(p => p.CourseId).Distinct();
            var courses = await _readCourseRepository.GetAll().Where(p => courseIds.Contains(p.Id)).ToListAsync(cancellationToken);
            var coursesDic = courses.ToDictionary(p => p.Id);

            if (withSessions)
            {
                var allClassRunIds = classruns.Select(p => p.Id);
                var itemSessions = (await _readSessionRepository
                        .GetAll()
                        .Where(p => allClassRunIds.Contains(p.ClassRunId))
                        .ToListAsync(cancellationToken))
                    .GroupBy(p => p.ClassRunId)
                    .ToDictionary(p => p.Key, p => p.ToList());
                return classruns
                    .Where(p => coursesDic.ContainsKey(p.CourseId))
                    .Select(p => ClassRunAggregatedEntityModel.Create(p, coursesDic[p.CourseId])
                        .WithSessions(itemSessions.GetValueOrDefault(p.Id, new List<Session>())))
                    .ToList();
            }

            return classruns
                .Where(p => coursesDic.ContainsKey(p.CourseId))
                .Select(p => ClassRunAggregatedEntityModel.Create(p, coursesDic[p.CourseId]))
                .ToList();
        }

        public Task<List<ClassRunAggregatedEntityModel>> All(
            Expression<Func<ClassRun, bool>> predicate,
            bool withSessions,
            CancellationToken cancellationToken = default)
        {
            return ByQuery(
                predicate != null
                    ? _readClassrunRepository.GetAll().Where(predicate)
                    : _readClassrunRepository.GetAll(),
                withSessions,
                cancellationToken);
        }
    }
}
