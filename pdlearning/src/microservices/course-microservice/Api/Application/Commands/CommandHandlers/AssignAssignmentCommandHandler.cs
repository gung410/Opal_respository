using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Conexus.Opal.AccessControl.Infrastructure;
using Microservice.Course.Application.BusinessLogics.EntityCud;
using Microservice.Course.Application.Commands.Abstracts;
using Microservice.Course.Application.Constants;
using Microservice.Course.Application.Events.Todos;
using Microservice.Course.Application.SharedQueries;
using Microservice.Course.Domain.Entities;
using Microservice.Course.Domain.Enums;
using Microservice.Course.Infrastructure;
using Microservice.Course.Settings;
using Microsoft.EntityFrameworkCore;
using Thunder.Platform.Core.Context;
using Thunder.Platform.Core.Domain.Repositories;
using Thunder.Platform.Core.Domain.UnitOfWork.Abstractions;
using Thunder.Platform.Core.Timing;
using Thunder.Platform.Cqrs;

namespace Microservice.Course.Application.Commands.CommandHandlers
{
    public class AssignAssignmentCommandHandler : BaseCommandHandler<AssignAssignmentCommand>
    {
        private readonly IThunderCqrs _thunderCqrs;
        private readonly IReadOnlyRepository<ParticipantAssignmentTrack> _readParticipantAssignmentTrackRepository;
        private readonly IReadOnlyRepository<CourseEntity> _readCourseRepository;
        private readonly IReadOnlyRepository<Assignment> _readAssignmentRepository;
        private readonly ParticipantAssignmentTrackCudLogic _participantAssignmentTrackCudLogic;
        private readonly WebAppLinkBuilder _webAppLinkBuilder;
        private readonly GetAggregatedRegistrationSharedQuery _getAggregatedRegistrationSharedQuery;

        public AssignAssignmentCommandHandler(
            IThunderCqrs thunderCqrs,
            IUnitOfWorkManager unitOfWorkManager,
            IReadOnlyRepository<ParticipantAssignmentTrack> readParticipantAssignmentTrackRepository,
            IReadOnlyRepository<CourseEntity> readCourseRepository,
            IReadOnlyRepository<Assignment> readAssignmentRepository,
            ParticipantAssignmentTrackCudLogic participantAssignmentTrackCudLogic,
            IUserContext userContext,
            IAccessControlContext<CourseUser> accessControlContext,
            WebAppLinkBuilder webAppLinkBuilder,
            GetAggregatedRegistrationSharedQuery getAggregatedRegistrationSharedQuery) : base(userContext, accessControlContext, unitOfWorkManager)
        {
            _thunderCqrs = thunderCqrs;
            _readParticipantAssignmentTrackRepository = readParticipantAssignmentTrackRepository;
            _readCourseRepository = readCourseRepository;
            _readAssignmentRepository = readAssignmentRepository;
            _participantAssignmentTrackCudLogic = participantAssignmentTrackCudLogic;
            _webAppLinkBuilder = webAppLinkBuilder;
            _getAggregatedRegistrationSharedQuery = getAggregatedRegistrationSharedQuery;
        }

        protected override async Task HandleAsync(AssignAssignmentCommand command, CancellationToken cancellationToken)
        {
            var registrationIds = command.Registrations.Select(x => x.RegistrationId).ToList();
            var aggregatedRegistrations =
                await _getAggregatedRegistrationSharedQuery.WithClassAndCourseByRegistrationIds(
                    registrationIds,
                    cancellationToken);

            EnsureValidPermission(ParticipantAssignmentTrack.HasAssignAssignmentPermission(CurrentUserId, CurrentUserRoles, p => HasPermissionPrefix(p)));

            EnsureBusinessLogicValid(aggregatedRegistrations, p => ParticipantAssignmentTrack.ValidateCanAssignAssignment(p.Course, p.ClassRun));

            var assignedParticipants = _readParticipantAssignmentTrackRepository
                .GetAll()
                .Where(x => registrationIds.Contains(x.RegistrationId) && x.AssignmentId == command.AssignmentId)
                .Select(x => x.RegistrationId)
                .ToList();

            var newParticipantAssignmentTracks = command.Registrations
                .Where(r => !assignedParticipants.Contains(r.RegistrationId))
                .Select(r => new ParticipantAssignmentTrack
                {
                    Id = Guid.NewGuid(),
                    RegistrationId = r.RegistrationId,
                    UserId = r.UserId,
                    AssignmentId = command.AssignmentId,
                    StartDate = command.StartDate,
                    EndDate = command.EndDate,
                    AssignedDate = Clock.Now,
                    Status = ParticipantAssignmentTrackStatus.NotStarted,
                    CreatedBy = CurrentUserIdOrDefault
                })
                .ToList();

            await _participantAssignmentTrackCudLogic.InsertMany(newParticipantAssignmentTracks, cancellationToken);

            await SendNotification(newParticipantAssignmentTracks, cancellationToken);
        }

        private async Task SendNotification(List<ParticipantAssignmentTrack> newParticipantAssignmentTracks, CancellationToken cancellationToken)
        {
            var assignmentIds = newParticipantAssignmentTracks.Select(p => p.AssignmentId).ToList();
            var assignments = await _readAssignmentRepository.GetAllListAsync(p => assignmentIds.Contains(p.Id));

            var courseDict = await GetAssignmentsCoursesDic(assignments);
            var assignmentDict = assignments.ToDictionary(x => x.Id, x => x);

            var events = BuildAssignedAssignmentNotifyLearnerEvents(newParticipantAssignmentTracks, assignmentDict, courseDict);

            await _thunderCqrs.SendEvents(events, cancellationToken);
        }

        private IEnumerable<AssignedAssignmentNotifyLearnerEvent> BuildAssignedAssignmentNotifyLearnerEvents(
            List<ParticipantAssignmentTrack> newParticipantAssignmentTracks,
            Dictionary<Guid, Assignment> assignmentDict,
            Dictionary<Guid, CourseEntity> courseDict)
        {
            var events = newParticipantAssignmentTracks
                .Where(x => assignmentDict.ContainsKey(x.AssignmentId)
                            && courseDict.ContainsKey(assignmentDict[x.AssignmentId].CourseId))
                .ToList()
                .Select(p =>
                {
                    var assignment = assignmentDict[p.AssignmentId];
                    var course = courseDict[assignment.CourseId];
                    return new AssignedAssignmentNotifyLearnerEvent(
                        CurrentUserIdOrDefault,
                        new AssignedAssignmentNotifyLearnerPayload
                        {
                            CourseTitle = course.CourseName,
                            StartDate = TimeHelper.ConvertTimeFromUtc(p.StartDate).ToString(DateTimeFormatConstant.OnlyDate),
                            EndDate = TimeHelper.ConvertTimeFromUtc(p.EndDate).ToString(DateTimeFormatConstant.OnlyDate),
                            AssignmentName = assignment.Title,

                            // [TODO] replace this link by learner assignment detail deep link when it is ready
                            ActionUrl = _webAppLinkBuilder.GetCourseDetailLearnerLinkForCAMModule(assignment.CourseId)
                        },
                        new List<Guid> { p.UserId });
                });
            return events;
        }

        private async Task<Dictionary<Guid, CourseEntity>> GetAssignmentsCoursesDic(List<Assignment> assignments)
        {
            var courseIds = assignments.Select(p => p.CourseId);
            var courses = await _readCourseRepository.GetAllListAsync(p => courseIds.Contains(p.Id));
            var courseDict = courses
                .Select(x => new { x.Id, Course = x })
                .ToDictionary(x => x.Id, x => x.Course);
            return courseDict;
        }
    }
}
