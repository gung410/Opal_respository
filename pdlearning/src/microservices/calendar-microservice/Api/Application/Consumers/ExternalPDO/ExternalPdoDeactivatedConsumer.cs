using System.Threading.Tasks;
using Conexus.Opal.Connector.RabbitMQ.Core;
using Conexus.Opal.InboxPattern;
using Microservice.Calendar.Application.Commands;
using Microservice.Calendar.Application.Consumers.Messages;
using Microservice.Calendar.Domain.Entities;
using Microservice.Calendar.Domain.Enums;
using Microsoft.Extensions.Logging;
using Thunder.Platform.Core;
using Thunder.Platform.Core.Domain.Repositories;
using Thunder.Platform.Cqrs;

namespace Microservice.Calendar.Application.Consumers
{
    [OpalConsumer("cx-competence-api.pdplan.deactivate.actionitem")]
    public class ExternalPdoDeactivatedConsumer : InboxSupportConsumer<ExternalPdoDeactivatedMessage>
    {
        private readonly IThunderCqrs _thunderCqrs;
        private readonly IRepository<PersonalEvent> _personalEventRepository;
        private readonly ILogger<ExternalPdoDeactivatedConsumer> _logger;

        public ExternalPdoDeactivatedConsumer(
            IThunderCqrs thunderCqrs,
            IRepository<PersonalEvent> personalEventRepository,
            ILogger<ExternalPdoDeactivatedConsumer> logger)
        {
            _thunderCqrs = thunderCqrs;
            _personalEventRepository = personalEventRepository;
            _logger = logger;
        }

        public async Task InternalHandleAsync(ExternalPdoDeactivatedMessage message)
        {
            ValidateMessage(message);

            var sourceId = message.AdditionalInformation.CourseId;
            var eventExisted = await _personalEventRepository.FirstOrDefaultAsync(x => x.SourceId == sourceId && x.Source == CalendarEventSource.ExternalPDO);

            if (eventExisted == null)
            {
                _logger.LogWarning("[ExternalPdoDeactivatedConsumer] External PDO event with SourceId {SourceId} was not existed.", sourceId);
                return;
            }

            var deleteAttendeeExternalPdoCommand = new DeleteAttendeeExternalPdoEventCommand
            {
                ExternalPdoId = sourceId,
                AttendeeId = message.Result.ObjectiveInfo.Identity.ExtId
            };

            await _thunderCqrs.SendCommand(deleteAttendeeExternalPdoCommand);
        }

        private void ValidateMessage(ExternalPdoDeactivatedMessage message)
        {
            Guard.NotNull(message, "message");
            Guard.NotNull(message.AdditionalInformation, "message.AdditionalInformation");
            Guard.NotNull(message.AdditionalInformation.CourseId, "message.AdditionalInformation.CourseId");

            Guard.NotNull(message.Result.ObjectiveInfo, "message.Result.ObjectiveInfo");
            Guard.NotNull(message.Result.ObjectiveInfo.Identity, "message.Result.ObjectiveInfo.Identity");
            Guard.NotNull(message.Result.ObjectiveInfo.Identity.ExtId, "message.Result.ObjectiveInfo.Identity.ExtId");
        }
    }
}
