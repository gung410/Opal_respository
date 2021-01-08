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
using Thunder.Platform.Core.Extensions;

namespace Microservice.Course.Application.SharedQueries
{
    public class GetAggregatedParticipantAssignmentTrackSharedQuery : BaseSharedQuery
    {
        private readonly IReadOnlyRepository<Assignment> _readAssignmentRepository;
        private readonly IReadOnlyRepository<ParticipantAssignmentTrack> _readParticipantAssignmentTrackRepository;
        private readonly IReadOnlyRepository<Registration> _readRegistrationRepository;
        private readonly IReadOnlyRepository<CourseEntity> _readCourseRepository;
        private readonly IReadOnlyRepository<ClassRun> _readClassRunRepository;
        private readonly IReadOnlyRepository<CourseUser> _readCourseUserRepository;

        public GetAggregatedParticipantAssignmentTrackSharedQuery(
            IReadOnlyRepository<ParticipantAssignmentTrack> readParticipantAssignmentTrackRepository,
            IReadOnlyRepository<Assignment> readAssignmentRepository,
            IReadOnlyRepository<Registration> readRegistrationRepository,
            IReadOnlyRepository<CourseEntity> readCourseRepository,
            IReadOnlyRepository<ClassRun> readClassRunRepository,
            IReadOnlyRepository<CourseUser> readCourseUserRepository)
        {
            _readParticipantAssignmentTrackRepository = readParticipantAssignmentTrackRepository;
            _readAssignmentRepository = readAssignmentRepository;
            _readRegistrationRepository = readRegistrationRepository;
            _readCourseRepository = readCourseRepository;
            _readClassRunRepository = readClassRunRepository;
            _readCourseUserRepository = readCourseUserRepository;
        }

        /// <summary>
        /// Get assignment tracking for each participant.
        /// </summary>
        /// <param name="predicate">Predicate.</param>
        /// <param name="cancellationToken">CancellationToken.</param>
        /// <returns>The list of assignment tracking for each participant.</returns>
        public async Task<List<ParticipantAssignmentTrackAggregatedEntityModel>> All(
            Expression<Func<ParticipantAssignmentTrack, bool>> predicate = null,
            CancellationToken cancellationToken = default)
        {
            var assignmentQuery = _readAssignmentRepository
                .GetAll();

            var aggregatedData = await _readParticipantAssignmentTrackRepository
                .GetAll()
                .WhereIf(predicate != null, predicate)
                .Join(
                    assignmentQuery,
                    p => p.AssignmentId,
                    t => t.Id,
                    (participantAssignmentTrack, assignment) => new { participantAssignmentTrack, assignment })
                .Join(
                    _readCourseRepository.GetAll(),
                    p => p.assignment.CourseId,
                    p => p.Id,
                    (gj, course) => new { gj.assignment, gj.participantAssignmentTrack, course })
                .ToListAsync(cancellationToken);

            return aggregatedData
                .Select(p => ParticipantAssignmentTrackAggregatedEntityModel
                    .New()
                    .WithParticipantAssignmentTrack(p.participantAssignmentTrack)
                    .WithAssignment(p.assignment)
                    .WithCourse(p.course))
                .ToList();
        }

        /// <summary>
        /// Get assignment tracking, Course, Classrun, Learner Info  by participant track Id.
        /// </summary>
        /// <param name="participantAssignmentTrackId">The ParticipantAssignmentTrack Id.</param>
        /// <returns>The list of assignment tracking for each participant.</returns>
        public async Task<ParticipantAssignmentTrackAggregatedEntityModel> ByParticipantAssignmentTrackId(Guid participantAssignmentTrackId)
        {
            var participantAssignmentTrack = await _readParticipantAssignmentTrackRepository.GetAsync(participantAssignmentTrackId);
            var learnerId = participantAssignmentTrack.UserId;
            var registration = await _readRegistrationRepository.GetAsync(participantAssignmentTrack.RegistrationId);
            var course = await _readCourseRepository.GetAsync(registration.CourseId);
            var classRun = await _readClassRunRepository.GetAsync(registration.ClassRunId);
            var learner = await _readCourseUserRepository.GetAsync(learnerId);
            var assignment = await _readAssignmentRepository.GetAsync(participantAssignmentTrack.AssignmentId);

            return ParticipantAssignmentTrackAggregatedEntityModel
                   .New()
                   .WithParticipantAssignmentTrack(participantAssignmentTrack)
                   .WithRegistration(registration)
                   .WithCourse(course)
                   .WithClassRun(classRun)
                   .WithLearner(learner)
                   .WithAssignment(assignment);
        }

        /// <summary>
        /// Get assignment tracking, Course, Classrun, Learner Info  by participant track query.
        /// </summary>
        /// <param name="participantAssignmentTracksQuery">Participant Assignment Track Query.</param>
        /// <param name="cancellationToken">CancellationToken.</param>
        /// <returns>The list of assignment tracking for each participant.</returns>
        public async Task<List<ParticipantAssignmentTrackAggregatedEntityModel>> FullByQuery(
            IQueryable<ParticipantAssignmentTrack> participantAssignmentTracksQuery,
            CancellationToken cancellationToken = default)
        {
            var participantAssignmentTrackToCourses = await participantAssignmentTracksQuery
                .Join(
                    _readRegistrationRepository.GetAll(),
                    p => p.RegistrationId,
                    p => p.Id,
                    (participantAssignmentTrack, registration) => new { participantAssignmentTrack, registration })
                .Join(
                    _readClassRunRepository.GetAll(),
                    p => p.registration.ClassRunId,
                    p => p.Id,
                    (gj, classRun) => new { gj.registration, gj.participantAssignmentTrack, classRun })
                .Join(
                    _readCourseRepository.GetAll(),
                    p => p.classRun.CourseId,
                    p => p.Id,
                    (gj, course) => new { gj.participantAssignmentTrack, gj.registration, gj.classRun, course })
                .Join(
                    _readCourseUserRepository.GetAll(),
                    p => p.participantAssignmentTrack.UserId,
                    p => p.Id,
                    (gj, learner) => new { gj.course, gj.classRun, gj.participantAssignmentTrack, gj.registration, learner })
                .Join(
                    _readAssignmentRepository.GetAll(),
                    p => p.participantAssignmentTrack.AssignmentId,
                    p => p.Id,
                    (gj, assignment) => new { gj.course, gj.classRun, gj.participantAssignmentTrack, gj.registration, gj.learner, assignment })
                .ToListAsync(cancellationToken);

            return participantAssignmentTrackToCourses.SelectList(
                p =>
                    ParticipantAssignmentTrackAggregatedEntityModel
                        .New()
                        .WithParticipantAssignmentTrack(p.participantAssignmentTrack)
                        .WithRegistration(p.registration)
                        .WithCourse(p.course)
                        .WithClassRun(p.classRun)
                        .WithLearner(p.learner)
                        .WithAssignment(p.assignment));
        }

        /// <summary>
        /// Get participant assignment tracks with registrations.
        /// </summary>
        /// <param name="participantAssignmentTracksQuery">Participant Assignment Track Query.</param>
        /// <param name="cancellationToken">CancellationToken.</param>
        /// <returns>The list of assignment tracks with registrations.</returns>
        public async Task<List<ParticipantAssignmentTrackAggregatedEntityModel>> WithRegistrationByQuery(
            IQueryable<ParticipantAssignmentTrack> participantAssignmentTracksQuery,
            CancellationToken cancellationToken = default)
        {
            var aggregatedQueryData = await participantAssignmentTracksQuery
                .Join(_readRegistrationRepository.GetAll(), p => p.RegistrationId, p => p.Id, (participantAssignmentTrack, registration) => new { participantAssignmentTrack, registration })
                .ToListAsync(cancellationToken);

            return aggregatedQueryData
                .Select(p => ParticipantAssignmentTrackAggregatedEntityModel
                    .New()
                    .WithParticipantAssignmentTrack(p.participantAssignmentTrack)
                    .WithRegistration(p.registration))
                .ToList();
        }
    }
}
