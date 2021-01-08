using System;
using System.Threading.Tasks;
using Conexus.Opal.Connector.RabbitMQ.Core;
using Conexus.Opal.InboxPattern;
using Microservice.Calendar.Application.Commands;
using Microservice.Calendar.Application.Consumers.Messages;
using Microservice.Calendar.Application.Consumers.Messages.Helpers;
using Microservice.Calendar.Domain.Entities;
using Microservice.Calendar.Domain.Enums;
using Microsoft.Extensions.Logging;
using Thunder.Platform.Core;
using Thunder.Platform.Core.Domain.Repositories;
using Thunder.Platform.Cqrs;

namespace Microservice.Calendar.Application.Consumers
{
    [OpalConsumer("cx-competence-api.pdplan.create.actionitem")]
    [OpalConsumer("cx-competence-api.pdplan.update.actionitem")]
    [OpalConsumer("cx-competence-api.pdplan.migrate.idp.actionitem.external-pdo")]
    public class ExternalPdoChangedConsumer : InboxSupportConsumer<ExternalPdoChangedMessage>
    {
        private readonly IThunderCqrs _thunderCqrs;
        private readonly IRepository<PersonalEvent> _personalEventRepository;
        private readonly ILogger<ExternalPdoChangedConsumer> _logger;

        public ExternalPdoChangedConsumer(
            IThunderCqrs thunderCqrs,
            IRepository<PersonalEvent> personalEventRepository,
            ILogger<ExternalPdoChangedConsumer> logger)
        {
            _thunderCqrs = thunderCqrs;
            _personalEventRepository = personalEventRepository;
            _logger = logger;
        }

        public async Task InternalHandleAsync(ExternalPdoChangedMessage message)
        {
            ValidateMessage(message);

            if (message.Result.Answer.LearningOpportunity.Source != "custom-pdo")
            {
                _logger.LogInformation(
                    "[ExternalPdoChangedConsumer] The message in type {MessageType} with id {MessageId} was skipped because it was not an External PDO.",
                    typeof(ExternalPdoChangedMessage),
                    OriginMessage.Id);
                return;
            }

            var sourceId = message.AdditionalInformation.CourseId;
            var userPersonalEventAccepted = ExternalPdoUserEventAcceptedMapper.GetExternalPdoUserEventAccepted(message.Result.AssessmentStatusInfo.AssessmentStatusCode);

            var existedEvent = await _personalEventRepository
                .FirstOrDefaultAsync(p => p.SourceId == sourceId && p.Source == CalendarEventSource.ExternalPDO);

            if (existedEvent == null)
            {
                await _thunderCqrs.SendCommand(BuildCreateExternalPdoEventCommand(message, sourceId, userPersonalEventAccepted));
                return;
            }

            await _thunderCqrs.SendCommand(BuildUpdateExternalPdoEventCommand(message, sourceId, userPersonalEventAccepted));
        }

        private void ValidateMessage(ExternalPdoChangedMessage message)
        {
            Guard.NotNull(message, "message");
            Guard.NotNull(message.Result, "message.Result");
            Guard.NotNull(message.Result.Answer, "message.Result.Answer");
            Guard.NotNull(message.Result.Answer.LearningOpportunity, "message.Result.Answer.LearningOpportunity");
            Guard.NotNull(message.Result.Answer.LearningOpportunity.Source, "message.Result.Answer.LearningOpportunity.Source");

            Guard.NotNull(message.Result.ObjectiveInfo, "message.Result.ObjectiveInfo");
            Guard.NotNull(message.Result.ObjectiveInfo.Identity, "message.Result.ObjectiveInfo.Identity");
            Guard.NotNull(message.Result.ObjectiveInfo.Identity.ExtId, "message.Result.ObjectiveInfo.Identity.ExtId");

            Guard.NotNull(message.AdditionalInformation, "message.AdditionalInformation");
            Guard.NotNull(message.AdditionalInformation.CourseId, "message.AdditionalInformation.CourseId");

            Guard.NotNull(message.Result.AssessmentStatusInfo, "message.Result.AssessmentStatusInfo");
            Guard.NotNull(message.Result.AssessmentStatusInfo.AssessmentStatusCode, "message.Result.AssessmentStatusInfo.AssessmentStatusCode");
        }

        private CreateExternalPdoEventCommand BuildCreateExternalPdoEventCommand(ExternalPdoChangedMessage message, Guid sourceId, bool isAccepted)
        {
            return new CreateExternalPdoEventCommand
            {
                ExternalPdoId = sourceId,
                Title = message.Result.Answer.LearningOpportunity.Name,
                Description = message.Result.Answer.LearningOpportunity.Extensions.Description,
                StartAt = message.Result.Answer.LearningOpportunity.Extensions.StartDate,
                EndAt = message.Result.Answer.LearningOpportunity.Extensions.EndDate,
                IsAllDay = true,
                AttendeeId = message.Result.ObjectiveInfo.Identity.ExtId,
                IsAccepted = isAccepted
            };
        }

        private UpdateExternalPdoEventCommand BuildUpdateExternalPdoEventCommand(ExternalPdoChangedMessage message, Guid sourceId, bool isAccepted)
        {
            return new UpdateExternalPdoEventCommand
            {
                ExternalPdoId = sourceId,
                Title = message.Result.Answer.LearningOpportunity.Name,
                Description = message.Result.Answer.LearningOpportunity.Extensions.Description,
                StartAt = message.Result.Answer.LearningOpportunity.Extensions.StartDate,
                EndAt = message.Result.Answer.LearningOpportunity.Extensions.EndDate,
                AttendeeId = message.Result.ObjectiveInfo.Identity.ExtId,
                IsAccepted = isAccepted
            };
        }
    }
}
