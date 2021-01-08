using System;
using System.Threading.Tasks;
using Conexus.Opal.Connector.RabbitMQ.Core;
using Conexus.Opal.InboxPattern;
using Microservice.Calendar.Application.Commands;
using Microservice.Calendar.Application.Consumers.Messages;
using Microservice.Calendar.Domain.Entities;
using Microservice.Calendar.Domain.Enums;
using Thunder.Platform.Core;
using Thunder.Platform.Core.Domain.Repositories;
using Thunder.Platform.Cqrs;

namespace Microservice.Calendar.Application.Consumers
{
    [OpalConsumer("cx-competence-api.pdplan.migrate.idp.learningneed")]
    [OpalConsumer("cx-competence-api.pdplan.create.learningneed")]
    [OpalConsumer("cx-competence-api.pdplan.update.learningneed")]
    public class LearningNeedsAnalysisChangedConsumer : InboxSupportConsumer<LearningNeedsAnalysisChangedMessage>
    {
        private readonly IThunderCqrs _thunderCqrs;
        private readonly IRepository<PersonalEvent> _personalEventRepository;

        public LearningNeedsAnalysisChangedConsumer(IThunderCqrs thunderCqrs, IRepository<PersonalEvent> personalEventRepository)
        {
            _thunderCqrs = thunderCqrs;
            _personalEventRepository = personalEventRepository;
        }

        public async Task InternalHandleAsync(LearningNeedsAnalysisChangedMessage message)
        {
            ValidateMessage(message);

            var sourceId = message.Result.ResultIdentity.ExtId;
            var existedEvent = await _personalEventRepository.FirstOrDefaultAsync(x => x.SourceId == sourceId && x.Source == CalendarEventSource.LNA);

            if (existedEvent == null)
            {
                await _thunderCqrs.SendCommand(BuildCreateLearningNeedsAnalysisEventCommand(message, sourceId));
                return;
            }

            await _thunderCqrs.SendCommand(BuildUpdateLearningNeedsAnalysisEventCommand(message, sourceId));
        }

        private void ValidateMessage(LearningNeedsAnalysisChangedMessage message)
        {
            Guard.NotNull(message, "message");
            Guard.NotNull(message.Result, "message.Result");
            Guard.NotNull(message.Result.ResultIdentity, "message.Result.ResultIdentity");
            Guard.NotNull(message.Result.ResultIdentity.ExtId, "message.Result.ResultIdentity.ExtId");

            Guard.NotNull(message.Result.ObjectiveInfo, "message.Result.ObjectiveInfo");
            Guard.NotNull(message.Result.ObjectiveInfo.Identity, "message.Result.ObjectiveInfo.Identity");
            Guard.NotNull(message.Result.ObjectiveInfo.Identity.ExtId, "message.Result.ObjectiveInfo.Identity.ExtId");
        }

        private CreateLearningNeedsAnalysisEventCommand BuildCreateLearningNeedsAnalysisEventCommand(LearningNeedsAnalysisChangedMessage message, Guid sourceId)
        {
            return new CreateLearningNeedsAnalysisEventCommand
            {
                LearningNeedsAnalysisId = sourceId,
                Title = CalendarEventSource.LNA.ToString(),
                Description = message.PdPlanURL,
                StartAt = message.Result.Created,
                EndAt = message.Result.DueDate,
                IsAllDay = true,
                AttendeeId = message.Result.ObjectiveInfo.Identity.ExtId
            };
        }

        private UpdateLearningNeedsAnalysisEventCommand BuildUpdateLearningNeedsAnalysisEventCommand(LearningNeedsAnalysisChangedMessage message, Guid sourceId)
        {
            return new UpdateLearningNeedsAnalysisEventCommand
            {
                LearningNeedsAnalysisId = sourceId,
                EndAt = message.Result.DueDate
            };
        }
    }
}
