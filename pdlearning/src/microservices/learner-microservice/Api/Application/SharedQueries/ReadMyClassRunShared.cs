using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microservice.Learner.Application.Models;
using Microservice.Learner.Application.SharedQueries.Abstractions;
using Microservice.Learner.Domain.Entities;
using Microservice.Learner.Extensions;
using Microsoft.EntityFrameworkCore;
using Thunder.Platform.Core.Domain.Repositories;
using Thunder.Platform.Core.Timing;

namespace Microservice.Learner.Application.SharedQueries
{
    public class ReadMyClassRunShared : BaseReadShared<MyClassRun>, IReadMyClassRunShared
    {
        private readonly IReadOnlyRepository<ClassRun> _readClassRunRepository;

        public ReadMyClassRunShared(
            IReadOnlyRepository<ClassRun> readClassRunRepository,
            IReadOnlyRepository<MyClassRun> readMyClassRunRepository) : base(readMyClassRunRepository)
        {
            _readClassRunRepository = readClassRunRepository;
        }

        public Task<bool> EnsureMyClassRunInserted(Guid userId, Guid courseId)
        {
            return ReadRepository
                .GetAll()
                .Where(p => p.UserId == userId)
                .Where(p => p.CourseId == courseId)
                .Where(MyClassRun.FilterInProgressExpr())
                .Where(MyClassRun.FilterFinishedLearningExpr().Not())
                .Where(p => p.ClassRunChangeStatus == null)
                .Where(p => p.WithdrawalStatus == null)
                .OrderByDescending(p => p.CreatedDate)
                .AnyAsync();
        }

        public Task<List<MyClassRun>> GetInProgressRegistrations(Guid userId, Guid courseId)
        {
            return ReadRepository
                .GetAll()
                .Where(p => p.UserId == userId)
                .Where(p => p.CourseId == courseId)
                .Where(MyClassRun.FilterInProgressExpr())
                .Where(MyClassRun.FilterFinishedLearningExpr().Not())
                .Where(MyClassRun.FilterPendingClassChangeExpr())
                .ToListAsync();
        }

        public Task<List<MyClassRunBasicInfoModel>> GetNotParticipants(Guid userId, List<Guid> courseIds)
        {
            return FilterNotParticipantQuery(userId, courseIds)
                .OrderByDescending(p => p.CreatedDate)
                .Select(p => new MyClassRunBasicInfoModel
                {
                    CourseId = p.CourseId,
                    ClassRunId = p.ClassRunId,
                    RegistrationId = p.RegistrationId,
                    WithdrawalStatus = p.WithdrawalStatus,
                    Status = p.Status,
                    ClassRunChangeStatus = p.ClassRunChangeStatus
                })
                .ToListAsync();
        }

        public Task<List<MyClassRun>> GetInProgressRegistrationsByClassRunId(Guid classRunId, bool allRegistrations = false)
        {
            var registrationQuery = ReadRepository
                .GetAll()
                .Where(p => p.ClassRunId == classRunId);

            if (!allRegistrations)
            {
                registrationQuery = registrationQuery
                    .Where(MyClassRun.FilterInProgressExpr())
                    .Where(MyClassRun.FilterFinishedLearningExpr().Not())
                    .Where(MyClassRun.FilterPendingClassChangeExpr());
            }

            return registrationQuery.ToListAsync();
        }

        public Task<Dictionary<Guid, MyClassRunBasicInfoModel>> GetExpiredRegistrations(List<Guid> registrationIds)
        {
            return ReadRepository
                .GetAll()
                .Where(p => registrationIds.Contains(p.RegistrationId))
                .Where(p => p.IsExpired)
                .Select(p => new
                {
                    p.CourseId,
                    p.ClassRunId,
                    p.RegistrationType
                })
                .ToDictionaryAsync(
                    s => s.CourseId,
                    e => new MyClassRunBasicInfoModel
                    {
                        ClassRunId = e.ClassRunId,
                        RegistrationType = e.RegistrationType
                    });
        }

        public Task<List<MyClassRun>> GetNotExpiredRegistrations(Guid userId, List<Guid> courseIds)
        {
            return ReadRepository
                .GetAll()
                .Where(p => p.UserId == userId)
                .Where(p => courseIds.Contains(p.CourseId))
                .Where(MyClassRun.FilterInProgressExpr()
                    .OrElse(MyClassRun.FilterFinishedLearningExpr()))
                .Where(MyClassRun.FilterPendingClassChangeExpr())
                .OrderByDescending(p => p.CreatedDate)
                .ToListAsync();
        }

        private IQueryable<MyClassRun> FilterNotParticipantQuery(Guid userId, List<Guid> courseIds)
        {
            // 1. Select my class run.
            var innerQuery = ReadRepository
                .GetAll()
                .Where(p => p.UserId == userId)
                .Where(p => courseIds.Contains(p.CourseId))
                .Where(MyClassRun.FilterInProgressExpr().Not()
                    .OrElse(MyClassRun.FilterClassChangeConfirmedExpr()))
                .GroupBy(p => new
                {
                    p.ClassRunId,
                    p.CourseId
                })
                .Select(p => new
                {
                    p.Key.ClassRunId,
                    p.Key.CourseId,
                    CreatedDate = p.Max(x => x.CreatedDate)
                });

            // 2. Select my class run.
            var myClassRunQuery = ReadRepository
                .GetAll()
                .Join(
                    innerQuery,
                    outer => new
                    {
                        outer.ClassRunId,
                        outer.CourseId,
                        outer.CreatedDate
                    },
                    inner => new
                    {
                        inner.ClassRunId,
                        inner.CourseId,
                        inner.CreatedDate
                    },
                    (myClassRunOuter, myClassRunInner) => myClassRunOuter);

            var now = Clock.Now;

            // 3. Select class run is in the time of application.
            var classRunQuery = _readClassRunRepository
                .GetAll()
                .Where(p => p.ApplicationStartDate.Value.Date <= now)
                .Where(p => p.ApplicationEndDate.Value.Date.AddDays(1) >= now)
                .Select(p => new
                {
                    p.Id
                });

            // 4. Final result
            return myClassRunQuery
                .Join(
                    classRunQuery,
                    mcl => mcl.ClassRunId,
                    cl => cl.Id,
                    (myClassRun, classRun) => myClassRun);
        }
    }
}
