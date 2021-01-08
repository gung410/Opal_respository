using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Conexus.Opal.AccessControl.Extensions;
using Conexus.Opal.AccessControl.Infrastructure;
using Microservice.Course.Application.AggregatedEntityModels;
using Microservice.Course.Application.BusinessLogics.EntityCud;
using Microservice.Course.Application.Commands.Abstracts;
using Microservice.Course.Application.Constants;
using Microservice.Course.Application.SharedQueries;
using Microservice.Course.Common.Extensions;
using Microservice.Course.Domain.Entities;
using Thunder.Platform.Core;
using Thunder.Platform.Core.Context;
using Thunder.Platform.Core.Domain.Repositories;
using Thunder.Platform.Core.Domain.UnitOfWork.Abstractions;
using Thunder.Platform.Core.Exceptions;
using Thunder.Platform.Core.Timing;
using Thunder.Platform.Core.Validation;
using Thunder.Service.Authentication;

namespace Microservice.Course.Application.Commands.CommandHandlers
{
    public class SaveSessionCommandHandler : BaseCommandHandler<SaveSessionCommand>
    {
        private readonly IReadOnlyRepository<ClassRun> _readClassRunRepository;
        private readonly IReadOnlyRepository<CourseEntity> _readCourseRepository;
        private readonly GetAggregatedSessionSharedQuery _getAggregatedSessionSharedQuery;
        private readonly SessionCudLogic _sessionCudLogic;
        private readonly GetBlockoutDateDependenciesSharedQuery _getBlockoutDateDependenciesSharedQuery;

        public SaveSessionCommandHandler(
            IUnitOfWorkManager unitOfWorkManager,
            IReadOnlyRepository<ClassRun> readClassRunRepository,
            IReadOnlyRepository<CourseEntity> readCourseRepository,
            SessionCudLogic sessionCudLogic,
            IUserContext userContext,
            IAccessControlContext<CourseUser> accessControlContext,
            GetAggregatedSessionSharedQuery getAggregatedSessionSharedQuery,
            GetBlockoutDateDependenciesSharedQuery getBlockoutDateDependenciesSharedQuery) : base(userContext, accessControlContext, unitOfWorkManager)
        {
            _readClassRunRepository = readClassRunRepository;
            _readCourseRepository = readCourseRepository;
            _sessionCudLogic = sessionCudLogic;
            _getAggregatedSessionSharedQuery = getAggregatedSessionSharedQuery;
            _getBlockoutDateDependenciesSharedQuery = getBlockoutDateDependenciesSharedQuery;
        }

        protected override async Task HandleAsync(SaveSessionCommand command, CancellationToken cancellationToken)
        {
            var classRun = await _readClassRunRepository.GetAsync(command.SessionData.ClassRunId);
            var course = await _readCourseRepository.GetAsync(classRun.CourseId);

            if (!command.UpdatePreRecordClipOnly)
            {
                EnsureValidPermission(Session.HasCreateEditPermission(CurrentUserId, CurrentUserRoles, p => HasPermissionPrefix(p)));
            }

            EnsureBusinessLogicValid(await ValidateCanSaveSession(
                command.SessionData.StartDateTime,
                course.ServiceSchemeIds.ToList(),
                cancellationToken));

            if (command.IsCreate)
            {
                await CreateNew(command, course, classRun, cancellationToken);
            }
            else
            {
                await Update(command, course, classRun, cancellationToken);
            }
        }

        private async Task Update(SaveSessionCommand command, CourseEntity course, ClassRun classRun, CancellationToken cancellationToken)
        {
            var aggregatedSession = await _getAggregatedSessionSharedQuery.ById(command.SessionData.Id, cancellationToken);

            if (!command.UpdatePreRecordClipOnly)
            {
                EnsureValidPermission(
                    aggregatedSession.Session.HasModifyPermission(CurrentUserId, course, _readCourseRepository.GetHasAdminRightChecker(course, AccessControlContext)(course)));

                EnsureBusinessLogicValid(aggregatedSession.Session.ValidateCanEditOrDelete(classRun, course));

                SetDataForSessionEntity(aggregatedSession.Session, command);
                aggregatedSession.Session.ChangedDate = Clock.Now;
                aggregatedSession.Session.ChangedBy = CurrentUserId;

                await _sessionCudLogic.Update(aggregatedSession, cancellationToken);
            }
            else
            {
                EnsureBusinessLogicValid(aggregatedSession.Session.CanEditOrDeletePreRecordInSession(classRun, course));

                SetPreRecordDataOnlyForSessionEntity(aggregatedSession.Session, command);
                aggregatedSession.Session.ChangedDate = Clock.Now;
                aggregatedSession.Session.ChangedBy = CurrentUserId;

                await _sessionCudLogic.Update(aggregatedSession, cancellationToken);
            }
        }

        private async Task CreateNew(SaveSessionCommand command, CourseEntity course, ClassRun classRun, CancellationToken cancellationToken)
        {
            var session = new Session
            {
                Id = command.SessionData.Id
            };
            SetDataForSessionEntity(session, command);
            session.CreatedDate = Clock.Now;
            session.CreatedBy = CurrentUserIdOrDefault;

            EnsureValidPermission(
                session.HasModifyPermission(CurrentUserId, course, _readCourseRepository.GetHasAdminRightChecker(course, AccessControlContext)(course)));

            EnsureBusinessLogicValid(Session.ValidateCanCreateSession(classRun, course));

            var sessionClassRun = await _readClassRunRepository.GetAsync(session.ClassRunId);
            var sessionCourse = await _readCourseRepository.GetAsync(sessionClassRun.CourseId);
            await _sessionCudLogic.Insert(SessionAggregatedEntityModel.Create(session, sessionClassRun, sessionCourse), cancellationToken);

            // TODO: If need to support create online session today, enable this
            /*
            if (session.StartDateTime.HasValue
                && session.StartDateTime.Value >= Clock.Now.StartOfDateInSystemTimeZone().ToUniversalTime()
                && session.StartDateTime.Value <= Clock.Now.EndOfDateInSystemTimeZone().ToUniversalTime()
                && session.CanBookMeeting())
            {
                await _bookWebinarMeetingLogic.BookMeeting(session);
            }
            */
        }

        private void SetDataForSessionEntity(Session session, SaveSessionCommand command)
        {
            session.Id = command.SessionData.Id;
            session.ClassRunId = command.SessionData.ClassRunId;
            session.SessionTitle = command.SessionData.SessionTitle;
            session.EndDateTime = command.SessionData.EndDateTime;
            session.StartDateTime = command.SessionData.StartDateTime;
            session.Venue = command.SessionData.Venue;
            session.LearningMethod = command.SessionData.LearningMethod;
            session.PreRecordId = command.SessionData.PreRecordId;
            session.PreRecordPath = command.SessionData.PreRecordPath;
            session.UsePreRecordClip = command.SessionData.UsePreRecordClip;
        }

        private void SetPreRecordDataOnlyForSessionEntity(Session session, SaveSessionCommand command)
        {
            session.PreRecordId = command.SessionData.PreRecordId;
            session.PreRecordPath = command.SessionData.PreRecordPath;
            session.UsePreRecordClip = command.SessionData.UsePreRecordClip;
        }

        private async Task<Validation<DateTime?>> ValidateCanSaveSession(DateTime? sessionDate, List<string> serviceSchemes, CancellationToken cancellationToken)
        {
            var blockoutDateDependenciesModel = await _getBlockoutDateDependenciesSharedQuery.Execute(
                serviceSchemes,
                sessionDate,
                null,
                cancellationToken);

            var valid = sessionDate == null || UserRoles.IsSysAdministrator(CurrentUserRoles) || !blockoutDateDependenciesModel.MatchedBlockoutDates.Any();
            return Validation.ValidIf(sessionDate, valid, "You cannot save this session since there are block-out date(s) found at session date");
        }
    }
}
