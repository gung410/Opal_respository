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
using Thunder.Platform.Core.Domain.Entities;
using Thunder.Platform.Core.Domain.Repositories;
using Thunder.Platform.Core.Extensions;

namespace Microservice.Course.Application.SharedQueries
{
    public class GetAggregatedRegistrationSharedQuery : BaseSharedQuery
    {
        private readonly IReadOnlyRepository<Registration> _readRegistrationRepository;
        private readonly IReadOnlyRepository<ClassRun> _readClassRunRepository;
        private readonly IReadOnlyRepository<CourseEntity> _readCourseRepository;
        private readonly IReadOnlyRepository<CourseUser> _readUserRepository;

        public GetAggregatedRegistrationSharedQuery(
            IReadOnlyRepository<Registration> readRegistrationRepository,
            IReadOnlyRepository<ClassRun> readClassRunRepository,
            IReadOnlyRepository<CourseEntity> readCourseRepository,
            IReadOnlyRepository<CourseUser> readUserRepository)
        {
            _readRegistrationRepository = readRegistrationRepository;
            _readClassRunRepository = readClassRunRepository;
            _readCourseRepository = readCourseRepository;
            _readUserRepository = readUserRepository;
        }

        public Task<List<RegistrationAggregatedEntityModel>> WithClassAndCourseByRegistrationIds(
             List<Guid> registrationIds,
             CancellationToken cancellationToken = default)
        {
            if (!registrationIds.Any())
            {
                return Task.FromResult(new List<RegistrationAggregatedEntityModel>());
            }

            registrationIds = registrationIds.Distinct().ToList();

            return WithClassAndCourseByRegistrationQuery(
                _readRegistrationRepository.GetAll().Where(p => registrationIds.Contains(p.Id)),
                cancellationToken);
        }

        public async Task<List<RegistrationAggregatedEntityModel>> WithClassAndCourseByRegistrationQuery(
            IQueryable<Registration> registrationsQuery,
            CancellationToken cancellationToken = default)
        {
            var registrationToCourses = await registrationsQuery
                .Join(_readClassRunRepository.GetAll(), p => p.ClassRunId, p => p.Id, (registration, classRun) => new { registration, classRun })
                .Join(_readCourseRepository.GetAll(), p => p.classRun.CourseId, p => p.Id, (gj, course) => new { gj.registration, gj.classRun, course })
                .ToListAsync(cancellationToken);

            return registrationToCourses
                .Select(p =>
                    RegistrationAggregatedEntityModel
                        .New()
                        .WithRegistration(p.registration)
                        .WithClassRun(p.classRun)
                        .WithCourse(p.course))
                .ToList();
        }

        public async Task<RegistrationAggregatedEntityModel> ById(
            Guid registrationId,
            CancellationToken cancellationToken = default)
        {
            var result = (await FullByQuery(
                _readRegistrationRepository.GetAll().Where(p => p.Id == registrationId),
                null,
                cancellationToken))
                .FirstOrDefault();

            result = EnsureEntityFound(result);

            return result;
        }

        public async Task<List<RegistrationAggregatedEntityModel>> FullByQuery(
            IQueryable<Registration> registrationsQuery,
            Expression<Func<ClassRun, bool>> classrunPredicate = null,
            CancellationToken cancellationToken = default)
        {
            var data = await registrationsQuery
                .Join(_readClassRunRepository.GetAll().WhereIf(classrunPredicate != null, classrunPredicate), p => p.ClassRunId, p => p.Id, (registration, classRun) => new { registration, classRun })
                .Join(_readCourseRepository.GetAll(), p => p.classRun.CourseId, p => p.Id, (gj, course) => new { gj.registration, gj.classRun, course })
                .Join(_readUserRepository.GetAll(), p => p.registration.UserId, p => p.Id, (gj, user) => new { gj.registration, gj.classRun, gj.course, user })
                .ToListAsync(cancellationToken);

            return data
                    .Select(p =>
                        RegistrationAggregatedEntityModel
                            .New()
                            .WithRegistration(p.registration)
                            .WithClassRun(p.classRun)
                            .WithCourse(p.course)
                            .WithUser(p.user))
                    .ToList();
        }

        public async Task<List<RegistrationAggregatedEntityModel>> FullByRegistrations(
            List<Registration> registrations,
            CancellationToken cancellationToken = default)
        {
            var classRunIds = registrations.Select(p => p.ClassRunId).Distinct();
            var classRunsDic = await _readClassRunRepository.GetAll().Where(p => classRunIds.Contains(p.Id)).ToDictionaryAsync(p => p.Id, cancellationToken);

            var courseIds = registrations.Select(p => p.CourseId).Distinct();
            var coursesDic = await _readCourseRepository.GetAll().Where(p => courseIds.Contains(p.Id)).ToDictionaryAsync(p => p.Id, cancellationToken);

            var userIds = registrations.Select(p => p.UserId).Distinct();
            var usersDic = await _readUserRepository.GetAll().Where(p => userIds.Contains(p.Id)).ToDictionaryAsync(p => p.Id, cancellationToken);

            return registrations
                .Select(p =>
                    RegistrationAggregatedEntityModel
                        .New()
                        .WithRegistration(p)
                        .WithClassRun(classRunsDic.GetValueOrDefault(p.ClassRunId))
                        .WithCourse(coursesDic.GetValueOrDefault(p.CourseId))
                        .WithUser(usersDic.GetValueOrDefault(p.UserId)))
                .ToList();
        }

        public async Task<List<RegistrationAggregatedEntityModel>> WithUserByClassRun(
            Guid courseId,
            Guid classRunId,
            Expression<Func<Registration, bool>> predicate,
            List<Registration> additionalRegistrations,
            CancellationToken cancellationToken)
        {
            var registrations = await _readRegistrationRepository.GetAll()
                .Where(p => p.CourseId == courseId && p.ClassRunId == classRunId)
                .Where(predicate)
                .ToListAsync(cancellationToken);
            if (additionalRegistrations != null)
            {
                registrations = registrations
                    .Concat(additionalRegistrations
                        .Where(p => p.CourseId == courseId && p.ClassRunId == classRunId)
                        .Where(predicate.Compile()))
                    .ToList();
            }

            var userIds = registrations.Select(p => p.UserId).Distinct().ToList();
            var users = await _readUserRepository.GetAll().Where(p => userIds.Contains(p.Id)).ToListAsync(cancellationToken);
            var usersDic = users.ToDictionary(p => p.Id);

            return registrations
                .Select(p =>
                    RegistrationAggregatedEntityModel
                        .New()
                        .WithRegistration(p)
                        .WithUser(usersDic[p.UserId]))
                .ToList();
        }

        public async Task<List<RegistrationAggregatedEntityModel>> WithUserByRegistrationsQuery(
            IQueryable<Registration> registrations,
            CancellationToken cancellationToken)
        {
            var data = await registrations
                .Join(
                    _readUserRepository.GetAll(),
                    p => p.UserId,
                    p => p.Id,
                    (registration, user) => new
                    {
                        registration,
                        user
                    })
                .ToListAsync(cancellationToken);

            return data
                .Select(p =>
                    RegistrationAggregatedEntityModel
                        .New()
                        .WithRegistration(p.registration)
                        .WithUser(p.user))
                .ToList();
        }

        public async Task<List<RegistrationAggregatedEntityModel>> WithExpiredOffer(
            DateTime expiredOfferDate,
            CancellationToken cancellationToken = default)
        {
            var registrations = await _readRegistrationRepository
                .GetAll()
                .Where(Registration.IsOfferPendingExpr())
                .Where(x => x.OfferSentDate < expiredOfferDate)
                .ToListAsync(cancellationToken);

            return registrations
                .Select(p =>
                    RegistrationAggregatedEntityModel
                        .New()
                        .WithRegistration(p))
                .ToList();
        }
    }
}
