using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Conexus.Opal.AccessControl.Infrastructure;
using Microservice.Course.Application.AggregatedEntityModels;
using Microservice.Course.Application.BusinessLogics.EntityCud;
using Microservice.Course.Application.Commands.Abstracts;
using Microservice.Course.Application.Constants;
using Microservice.Course.Application.Events.Todos;
using Microservice.Course.Application.SharedQueries;
using Microservice.Course.Domain.Entities;
using Microservice.Course.Settings;
using Microsoft.EntityFrameworkCore;
using Thunder.Platform.Core.Context;
using Thunder.Platform.Core.Domain.Repositories;
using Thunder.Platform.Core.Domain.UnitOfWork.Abstractions;
using Thunder.Platform.Core.Exceptions;
using Thunder.Platform.Cqrs;

namespace Microservice.Course.Application.Commands.CommandHandlers
{
    public class ChangeLearningMethodCommandHandler : BaseCommandHandler<ChangeLearningMethodCommand>
    {
        private readonly IThunderCqrs _thunderCqrs;
        private readonly WebAppLinkBuilder _webAppLinkBuilder;
        private readonly IReadOnlyRepository<Registration> _readRegistrationRepository;
        private readonly SessionCudLogic _sessionCudLogic;
        private readonly GetAggregatedSessionSharedQuery _getAggregatedSessionSharedQuery;

        public ChangeLearningMethodCommandHandler(
            IUnitOfWorkManager unitOfWorkManager,
            IThunderCqrs thunderCqrs,
            WebAppLinkBuilder webAppLinkBuilder,
            IReadOnlyRepository<Registration> readRegistrationRepository,
            GetAggregatedSessionSharedQuery getAggregatedSessionSharedQuery,
            SessionCudLogic sessionCudLogic,
            IUserContext userContext,
            IAccessControlContext<CourseUser> accessControlContext) : base(userContext, accessControlContext, unitOfWorkManager)
        {
            _thunderCqrs = thunderCqrs;
            _webAppLinkBuilder = webAppLinkBuilder;
            _readRegistrationRepository = readRegistrationRepository;
            _getAggregatedSessionSharedQuery = getAggregatedSessionSharedQuery;
            _sessionCudLogic = sessionCudLogic;
        }

        protected override async Task HandleAsync(ChangeLearningMethodCommand command, CancellationToken cancellationToken)
        {
            var aggregatedSession = await _getAggregatedSessionSharedQuery.ById(command.Id, cancellationToken);

            EnsureBusinessLogicValid(aggregatedSession, p => p.Session.ValidateCanChangeLearningMethod(p.Course));

            aggregatedSession.Session.LearningMethod = command.LearningMethod;

            await _sessionCudLogic.Update(aggregatedSession, cancellationToken);
            await SendNotificationWhenChangeLearningMethod(aggregatedSession, cancellationToken);
        }

        private async Task SendNotificationWhenChangeLearningMethod(SessionAggregatedEntityModel aggregatedSession, CancellationToken cancellationToken)
        {
            await SendToLearners(aggregatedSession, cancellationToken);
            await SendToManagementInvoledUsers(aggregatedSession, cancellationToken);
        }

        private async Task SendToLearners(SessionAggregatedEntityModel aggregatedSession, CancellationToken cancellationToken)
        {
            var listLearnerIds = await _readRegistrationRepository.GetAll()
                .Where(p => p.ClassRunId == aggregatedSession.Session.ClassRunId)
                .Where(Registration.IsParticipantExpr())
                .Select(x => x.UserId).ToListAsync(cancellationToken);

            if (aggregatedSession.Session.IsLearningOnline())
            {
                var notifyLearnerEvent = ChangeLearningMethodNotifyInvolvedUserEvent.CreateForChangeLearningMethodToOnline(
                    CurrentUserIdOrDefault,
                    new ChangeLearningMethodNotifyInvolvedUserPayload
                    {
                        CourseName = aggregatedSession.Course.CourseName,
                        ClassRunName = aggregatedSession.ClassRun.ClassTitle,
                        SessionTitle = aggregatedSession.Session.SessionTitle,
                        SessionVenue = "WEBINAR",
                        MessageIfIsLearningOnline = "Please find the button to join the Webinar session in the class run of your course in Learner Module",
                        ChangeLearningMethodText = "offline to online",
                        ActionUrl = _webAppLinkBuilder.GetCourseDetailLearnerLinkForCAMModule(aggregatedSession.Course.Id)
                    },
                    listLearnerIds);
                await _thunderCqrs.SendEvent(notifyLearnerEvent, cancellationToken);
            }
            else
            {
                var notifyLearnerEvent = ChangeLearningMethodNotifyInvolvedUserEvent.CreateForChangeLearningMethodToOffline(
                   CurrentUserIdOrDefault,
                   new ChangeLearningMethodNotifyInvolvedUserPayload
                   {
                       CourseName = aggregatedSession.Course.CourseName,
                       ClassRunName = aggregatedSession.ClassRun.ClassTitle,
                       SessionTitle = aggregatedSession.Session.SessionTitle,
                       SessionVenue = aggregatedSession.Session.Venue,
                       ChangeLearningMethodText = "online to offline",
                       ActionUrl = _webAppLinkBuilder.GetCourseDetailLearnerLinkForCAMModule(aggregatedSession.Course.Id)
                   },
                   listLearnerIds);
                await _thunderCqrs.SendEvent(notifyLearnerEvent, cancellationToken);
            }
        }

        private async Task SendToManagementInvoledUsers(SessionAggregatedEntityModel aggregatedSession, CancellationToken cancellationToken)
        {
            var listInvoledUserIds = aggregatedSession.Course.GetManagementInvoledUserIds().Distinct().ToList();
            var sessionDetailLinkForLMMModule = _webAppLinkBuilder.GetSessionDetailLinkForLMMModule(
                LMMTabConfigurationConstant.CoursesTab,
                LMMTabConfigurationConstant.CourseInfoTab,
                LMMTabConfigurationConstant.AllClassRunsTab,
                CourseDetailModeConstant.View,
                LMMTabConfigurationConstant.ClassRunInfoTab,
                ClassRunDetailModeConstant.View,
                LMMTabConfigurationConstant.SessionInfoTab,
                AssignmentDetailModeConstant.View,
                aggregatedSession.Course.Id,
                aggregatedSession.ClassRun.Id,
                aggregatedSession.Session.Id);

            if (aggregatedSession.Session.IsLearningOnline())
            {
                var notifyManagementInvoledUserEvent = ChangeLearningMethodNotifyInvolvedUserEvent.CreateForChangeLearningMethodToOnline(
                CurrentUserIdOrDefault,
                new ChangeLearningMethodNotifyInvolvedUserPayload
                {
                    CourseName = aggregatedSession.Course.CourseName,
                    ClassRunName = aggregatedSession.ClassRun.ClassTitle,
                    SessionTitle = aggregatedSession.Session.SessionTitle,
                    SessionVenue = "WEBINAR",
                    MessageIfIsLearningOnline = "Please find the button to join the Webinar session in the class run of your course in Learning Management",
                    ChangeLearningMethodText = "offline to online",
                    ActionUrl = sessionDetailLinkForLMMModule
                },
                listInvoledUserIds);
                await _thunderCqrs.SendEvent(notifyManagementInvoledUserEvent, cancellationToken);
            }
            else
            {
                var notifyManagementInvoledUserEvent = ChangeLearningMethodNotifyInvolvedUserEvent.CreateForChangeLearningMethodToOffline(
              CurrentUserIdOrDefault,
              new ChangeLearningMethodNotifyInvolvedUserPayload
              {
                  CourseName = aggregatedSession.Course.CourseName,
                  ClassRunName = aggregatedSession.ClassRun.ClassTitle,
                  SessionTitle = aggregatedSession.Session.SessionTitle,
                  SessionVenue = aggregatedSession.Session.Venue,
                  ChangeLearningMethodText = "online to offline",
                  ActionUrl = sessionDetailLinkForLMMModule
              },
              listInvoledUserIds);
                await _thunderCqrs.SendEvent(notifyManagementInvoledUserEvent, cancellationToken);
            }
        }
    }
}
