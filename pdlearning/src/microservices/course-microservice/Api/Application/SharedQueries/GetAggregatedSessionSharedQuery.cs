using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microservice.Course.Application.AggregatedEntityModels;
using Microservice.Course.Application.SharedQueries.Abstractions;
using Microservice.Course.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Thunder.Platform.Core.Domain.Entities;
using Thunder.Platform.Core.Domain.Repositories;

namespace Microservice.Course.Application.SharedQueries
{
    public class GetAggregatedSessionSharedQuery : BaseSharedQuery
    {
        private readonly IReadOnlyRepository<ClassRun> _readClassrunRepository;
        private readonly IReadOnlyRepository<CourseEntity> _readCourseRepository;
        private readonly IReadOnlyRepository<Session> _readSessionRepository;

        public GetAggregatedSessionSharedQuery(
            IReadOnlyRepository<ClassRun> readClassrunRepository,
            IReadOnlyRepository<CourseEntity> readCourseRepository,
            IReadOnlyRepository<Session> readSessionRepository)
        {
            _readClassrunRepository = readClassrunRepository;
            _readCourseRepository = readCourseRepository;
            _readSessionRepository = readSessionRepository;
        }

        public async Task<SessionAggregatedEntityModel> ById(
           Guid id,
           CancellationToken cancellationToken = default)
        {
            var sessionAggregatedEntityModel = (await ByQuery(_readSessionRepository.GetAll().Where(p => p.Id == id), cancellationToken)).FirstOrDefault();

            sessionAggregatedEntityModel = EnsureEntityFound(sessionAggregatedEntityModel);

            return sessionAggregatedEntityModel;
        }

        public Task<List<SessionAggregatedEntityModel>> ByIds(
            List<Guid> ids,
            CancellationToken cancellationToken = default)
        {
            return ByQuery(_readSessionRepository.GetAll().Where(p => ids.Contains(p.Id)), cancellationToken);
        }

        public async Task<List<SessionAggregatedEntityModel>> ByQuery(
            IQueryable<Session> sessionsQuery,
            CancellationToken cancellationToken = default)
        {
            var classRunQuery = _readClassrunRepository
                .GetAll()
                .Join(sessionsQuery, p => p.Id, p => p.ClassRunId, (classrunQuery, sessionQuery) => classrunQuery)
                .Distinct();

            var sessions = await sessionsQuery.ToListAsync(cancellationToken);
            var classruns = (await classRunQuery.ToListAsync(cancellationToken)).ToDictionary(p => p.Id);
            var courses = (await _readCourseRepository
                .GetAll()
                .Join(classRunQuery, p => p.Id, p => p.CourseId, (courseQuery, classrunQuery) => courseQuery)
                .Distinct()
                .ToListAsync(cancellationToken))
                .ToDictionary(p => p.Id);

            return sessions
                .Select(p =>
                    SessionAggregatedEntityModel.Create(p, classruns[p.ClassRunId], courses[classruns[p.ClassRunId].CourseId]))
                .ToList();
        }

        public Task<List<SessionAggregatedEntityModel>> ByClassrunQuery(
            IQueryable<ClassRun> classRunsQuery,
            CancellationToken cancellationToken = default)
        {
            return ByQuery(
                _readSessionRepository
                    .GetAll()
                    .Join(classRunsQuery, p => p.ClassRunId, p => p.Id, (session, classRun) => session),
                cancellationToken);
        }
    }
}
