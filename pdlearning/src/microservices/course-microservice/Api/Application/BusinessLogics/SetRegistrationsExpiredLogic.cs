using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microservice.Course.Application.AggregatedEntityModels;
using Microservice.Course.Application.BusinessLogics.Abstractions;
using Microservice.Course.Application.BusinessLogics.EntityCud;
using Microservice.Course.Application.Events.Todos;
using Microservice.Course.Application.SharedQueries;
using Microservice.Course.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Thunder.Platform.Core.Context;
using Thunder.Platform.Core.Domain.Repositories;
using Thunder.Platform.Core.Extensions;
using Thunder.Platform.Cqrs;

namespace Microservice.Course.Application.BusinessLogics
{
    /// <summary>
    /// This is logic service to support process registrations expiration. Also notify events for learner.
    /// </summary>
    public class SetRegistrationsExpiredLogic : BaseBusinessLogic
    {
        private readonly IReadOnlyRepository<ClassRun> _readClassrunRepository;
        private readonly IReadOnlyRepository<CourseEntity> _readCourseRepository;
        private readonly IReadOnlyRepository<Registration> _readRegistrationRepository;
        private readonly IThunderCqrs _thunderCqrs;
        private readonly GetAggregatedRegistrationSharedQuery _getAggregatedRegistrationSharedQuery;

        public SetRegistrationsExpiredLogic(
            IReadOnlyRepository<ClassRun> readClassrunRepository,
            IReadOnlyRepository<CourseEntity> readCourseRepository,
            IReadOnlyRepository<Registration> readRegistrationRepository,
            IThunderCqrs thunderCqrs,
            GetAggregatedRegistrationSharedQuery getAggregatedRegistrationSharedQuery,
            IUserContext userContext) : base(userContext)
        {
            _readClassrunRepository = readClassrunRepository;
            _readCourseRepository = readCourseRepository;
            _readRegistrationRepository = readRegistrationRepository;
            _thunderCqrs = thunderCqrs;
            _getAggregatedRegistrationSharedQuery = getAggregatedRegistrationSharedQuery;
        }

        public async Task<List<Registration>> Execute(
            IQueryable<ClassRun> classrunsQuery,
            Guid? currentUserId,
            bool forceSetExpiredAllInClass = false,
            CancellationToken cancellationToken = default)
        {
            // Filter need set expired registrations classrun without course checking to reduce amount of classruns
            var classRuns = await classrunsQuery
                .Where(ClassRun.NeedSetExpiredRegistrationsClassRunExpr(null))
                .ToListAsync(cancellationToken);
            return await Execute(classRuns, currentUserId, forceSetExpiredAllInClass, cancellationToken);
        }

        public async Task<List<Registration>> Execute(
            List<ClassRun> classruns,
            Guid? currentUserId,
            bool forceSetExpiredAllInClass = false,
            CancellationToken cancellationToken = default)
        {
            var courseIds = classruns.Select(p => p.CourseId).Distinct().ToList();

            var courseDict = await _readCourseRepository
                .GetAll()
                .Where(p => courseIds.Contains(p.Id))
                .ToDictionaryAsync(p => p.Id, p => p, cancellationToken);

            var needSetExpiredRegistrationsClassRuns = classruns
                .Where(p => p.NeedSetExpiredRegistrationsClassRun(courseDict[p.CourseId]))
                .Select(p => p.Id)
                .ToList();

            var expiredRegistrationsQuery = _readRegistrationRepository.GetAll()
                .Where(p => needSetExpiredRegistrationsClassRuns.Contains(p.ClassRunId))
                .WhereIf(!forceSetExpiredAllInClass, Registration.CanBeSetExpiredExpr());

            var setExpiredAggregatedRegistrations = (await _getAggregatedRegistrationSharedQuery
                .FullByQuery(expiredRegistrationsQuery, cancellationToken: cancellationToken))
                .SelectList(p =>
                {
                    p.Registration.IsExpired = true;
                    return p;
                });

            await SendNotifyLearnerRegistrationUnsuccessfulEvents(setExpiredAggregatedRegistrations, currentUserId, cancellationToken);

            return setExpiredAggregatedRegistrations.SelectList(p => p.Registration);
        }

        public async Task<List<Registration>> Execute(
            List<Guid> classrunIds,
            Guid? currentUserId,
            bool forceSetExpiredAllInClass = false,
            CancellationToken cancellationToken = default)
        {
            var classruns = await _readClassrunRepository
                .GetAll()
                .Where(p => classrunIds.Contains(p.Id))
                .ToListAsync(cancellationToken);

            return await Execute(classruns, currentUserId, forceSetExpiredAllInClass, cancellationToken);
        }

        private async Task SendNotifyLearnerRegistrationUnsuccessfulEvents(
            List<RegistrationAggregatedEntityModel> setExpiredAggregatedRegistrations,
            Guid? currentUserId,
            CancellationToken cancellationToken)
        {
            var notifyLearnerRegistrationUnsuccessfulEvents = setExpiredAggregatedRegistrations
                .Where(p => p.ClassRun.StartDateTime.HasValue)
                .SelectList(expiredRegistration => new NotifyLearnerRegistrationUnsuccessfulEvent(
                    currentUserId.GetValueOrDefault(),
                    new NotifyLearnerRegistrationUnsuccessfulPayload
                    {
                        CourseName = expiredRegistration.Course.CourseName,
                        ClassName = expiredRegistration.ClassRun.ClassTitle,
                        LearnerName = expiredRegistration.User.FullName(),
                        ClassStartDate = expiredRegistration.ClassRun.StartDateTime.Value.ToShortDateString()
                    },
                    new List<Guid>() { expiredRegistration.User.Id }));

            await _thunderCqrs.SendEvents(notifyLearnerRegistrationUnsuccessfulEvents, cancellationToken);
        }
    }
}
