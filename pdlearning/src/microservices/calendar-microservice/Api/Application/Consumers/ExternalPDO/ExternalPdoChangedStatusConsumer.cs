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
    [OpalConsumer("cx-competence-api.pdplan.change.status_type")]
    public class ExternalPdoChangedStatusConsumer : InboxSupportConsumer<ExternalPdoChangedStatusMessage>
    {
        private const string _pdPlanType = "Idp";
        private const string _pdPlanActivity = "ActionItem";
        private readonly IThunderCqrs _thunderCqrs;
        private readonly IRepository<PersonalEvent> _personalEventRepository;
        private readonly ILogger<ExternalPdoChangedStatusConsumer> _logger;

        public ExternalPdoChangedStatusConsumer(
            IThunderCqrs thunderCqrs,
            IRepository<PersonalEvent> personalEventRepository,
            ILogger<ExternalPdoChangedStatusConsumer> logger)
        {
            _thunderCqrs = thunderCqrs;
            _personalEventRepository = personalEventRepository;
            _logger = logger;
        }

        public async Task InternalHandleAsync(ExternalPdoChangedStatusMessage message)
        {
            ValidateMessage(message);
            if (message.Result.PdPlanType != _pdPlanType || message.Result.PdPlanActivity != _pdPlanActivity)
            {
                return;
            }

            var sourceId = message.AdditionalInformation.CourseId;
            var existedEvent = await _personalEventRepository
                .FirstOrDefaultAsync(p => p.SourceId == sourceId && p.Source == CalendarEventSource.ExternalPDO);

            if (existedEvent == null)
            {
                _logger.LogWarning("[ExternalPdoChangedStatusConsumer] ExternalPdo with Id {ResultExtId} was not existed.", sourceId);
                return;
            }

            var userPersonalEventAccepted = ExternalPdoUserEventAcceptedMapper.GetExternalPdoUserEventAccepted(message.Result.TargetStatusType.StatusTypeCode);
            var updateExternalPdoCommand = new UpdateExternalPdoEventCommand
            {
                ExternalPdoId = sourceId,
                Title = existedEvent.Title,
                Description = existedEvent.Description,
                StartAt = existedEvent.StartAt,
                EndAt = existedEvent.EndAt,
                AttendeeId = Guid.Parse(message.Result.ObjectiveInfo.Identity.ExtId),
                IsAccepted = userPersonalEventAccepted
            };

            await _thunderCqrs.SendCommand(updateExternalPdoCommand);
        }

        private void ValidateMessage(ExternalPdoChangedStatusMessage message)
        {
            Guard.NotNull(message, "message");
            Guard.NotNull(message.Result, "message.Result");
            Guard.NotNull(message.Result.TargetStatusType, "message.Result.TargetStatusType");
            Guard.NotNull(message.Result.TargetStatusType.StatusTypeCode, "message.Result.TargetStatusType.StatusTypeCode");

            Guard.NotNull(message.Result.ObjectiveInfo, "message.Result.ObjectiveInfo");
            Guard.NotNull(message.Result.ObjectiveInfo.Identity, "message.Result.ObjectiveInfo.Identity");
            Guard.NotNull(message.Result.ObjectiveInfo.Identity.ExtId, "message.Result.ObjectiveInfo.Identity.ExtId");

            Guard.NotNull(message.AdditionalInformation, "message.AdditionalInformation");
            Guard.NotNull(message.AdditionalInformation.CourseId, "message.AdditionalInformation.CourseId");
        }
    }
}
