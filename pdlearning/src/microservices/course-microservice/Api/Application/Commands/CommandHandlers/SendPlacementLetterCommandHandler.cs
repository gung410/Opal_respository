using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Conexus.Opal.AccessControl.Infrastructure;
using Microservice.Course.Application.BusinessLogics;
using Microservice.Course.Application.Commands.Abstracts;
using Microservice.Course.Application.Constants;
using Microservice.Course.Application.Events.Todos;
using Microservice.Course.Application.SharedQueries;
using Microservice.Course.Domain.Entities;
using Microservice.Course.Settings;
using Thunder.Platform.Core.Context;
using Thunder.Platform.Core.Domain.Repositories;
using Thunder.Platform.Core.Domain.UnitOfWork.Abstractions;
using Thunder.Platform.Core.Exceptions;
using Thunder.Platform.Cqrs;

namespace Microservice.Course.Application.Commands.CommandHandlers
{
    public class SendPlacementLetterCommandHandler : BaseCommandHandler<SendPlacementLetterCommand>
    {
        private readonly IReadOnlyRepository<Registration> _readRegistrationRepository;
        private readonly IReadOnlyRepository<CourseUser> _readCourseUserRepository;
        private readonly IReadOnlyRepository<Session> _readSessionRepository;
        private readonly GetAggregatedRegistrationSharedQuery _getAggregatedRegistrationSharedQuery;
        private readonly IThunderCqrs _thunderCqrs;
        private readonly WebAppLinkBuilder _webAppLinkBuilder;
        private readonly SendPlacementLetterLogic _sendPlacementLetterLogic;

        public SendPlacementLetterCommandHandler(
            IUnitOfWorkManager unitOfWorkManager,
            IReadOnlyRepository<Registration> readRegistrationRepository,
            IReadOnlyRepository<CourseUser> readCourseUserRepository,
            IReadOnlyRepository<Session> readSessionRepository,
            IUserContext userContext,
            IThunderCqrs thunderCqrs,
            IAccessControlContext<CourseUser> accessControlContext,
            GetAggregatedRegistrationSharedQuery getAggregatedRegistrationSharedQuery,
            WebAppLinkBuilder webAppLinkBuilder,
            SendPlacementLetterLogic sendPlacementLetterLogic) : base(userContext, accessControlContext, unitOfWorkManager)
        {
            _readRegistrationRepository = readRegistrationRepository;
            _readCourseUserRepository = readCourseUserRepository;
            _readSessionRepository = readSessionRepository;
            _getAggregatedRegistrationSharedQuery = getAggregatedRegistrationSharedQuery;
            _thunderCqrs = thunderCqrs;
            _webAppLinkBuilder = webAppLinkBuilder;
            _sendPlacementLetterLogic = sendPlacementLetterLogic;
        }

        protected override async Task HandleAsync(SendPlacementLetterCommand command, CancellationToken cancellationToken)
        {
            var aggregatedRegistrations = await _getAggregatedRegistrationSharedQuery.FullByQuery(
                _readRegistrationRepository
                    .GetAll()
                    .Where(Registration.IsParticipantExpr())
                    .Where(a => command.Ids.Contains(a.Id)),
                null,
                cancellationToken);

            EnsureBusinessLogicValid(aggregatedRegistrations, p => p.Course.ValidateNotArchived());

            // Get list sessions information
            var classRunIds = aggregatedRegistrations.Select(x => x.ClassRun.Id);
            var sessions = await _readSessionRepository.GetAllListAsync(x => classRunIds.Contains(x.ClassRunId));
            var classRunIdToSessionsDic = sessions.GroupBy(s => s.ClassRunId).ToDictionary(x => x.Key, x => x.ToList());

            var currentUserInfo = await _readCourseUserRepository.GetAsync(CurrentUserIdOrDefault);

            if (!string.IsNullOrEmpty(command.Base64Message))
            {
                await _thunderCqrs.SendEvents(
                    aggregatedRegistrations.Select(p =>
                    {
                        var detailUrl = _webAppLinkBuilder.GetCourseDetailLearnerLinkForCAMModule(p.Course.Id);
                        return new SendPlacementLetterUsingEditorNotifyLearnerEvent(
                            CurrentUserIdOrDefault,
                            new SendPlacementLetterUsingEditorNotifyLearnerPayload
                            {
                                CourseName = p.Course.CourseName,
                                CourseCode = p.Course.CourseCode,
                                ActionUrl = detailUrl
                            },
                            new List<Guid> { p.Registration.UserId },
                            command.GetReplacedTagsMessage(
                                p.Course.CourseName,
                                p.User.FullName(),
                                p.Course.CourseCode,
                                currentUserInfo.FullName(),
                                currentUserInfo.Email,
                                _sendPlacementLetterLogic.RenderSessionTable(
                                    classRunIdToSessionsDic.GetValueOrDefault(p.ClassRun.Id, new List<Session>())),
                                detailUrl));
                    }),
                    cancellationToken);
            }
        }
    }
}
