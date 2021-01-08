using System.Threading.Tasks;
using Conexus.Opal.OutboxPattern;
using Microservice.Content.Application.Commands;
using Microservice.Content.Application.Events;
using Microservice.Content.Application.Models;
using Microservice.Content.Domain.Entities;
using Microservice.Content.Infrastructure;
using Microservice.Content.Versioning.Application.Commands;
using Microservice.Content.Versioning.Core;
using Microservice.Content.Versioning.Entities;
using Thunder.Platform.Core.Context;
using Thunder.Platform.Cqrs;

namespace Microservice.Content.Domain.Versioning
{
    public class DigitalContentVersionResolver : SchemaVersionResolver<DigitalContentModel, DigitalContent>
    {
        private readonly IOutboxQueue _outboxQueue;
        private readonly IUserContext _userContext;
        private readonly IThunderCqrs _thunderCqrs;

        public DigitalContentVersionResolver(IThunderCqrs thunderCqrs, IOutboxQueue outboxQueue, IUserContext userContext)
        {
            _thunderCqrs = thunderCqrs;
            _outboxQueue = outboxQueue;
            _userContext = userContext;
        }

        public override async Task DoLogicCheckoutVersion(
            RevertVersionCommand command,
            VersionTracking versionTrackingData,
            DigitalContentModel contentModel)
        {
            // Mark current active record as archived version
            MarkDigitalContentAsArchivedCommand markDigitalContentAsArchivedCommand = new MarkDigitalContentAsArchivedCommand
            {
                Id = command.Request.CurrentActiveId,
                UserId = command.UserId
            };

            await _thunderCqrs.SendCommand(markDigitalContentAsArchivedCommand);

            // CLone backup record to active version
            var rollbackCommand = new RollbackDigitalContentCommand
            {
                RevertFromRecordId = command.Request.RevertFromRecordId,
                RevertToRecordId = command.NewActiveId,
                CurrentActiveId = command.Request.CurrentActiveId,
                UserId = command.UserId,
            };
            await _thunderCqrs.SendCommand(rollbackCommand);

            await _outboxQueue.QueueMessageAsync(
                new CloneMetadataBody
                {
                    CloneFromResouceId = rollbackCommand.RevertFromRecordId,
                    CloneToResouceId = rollbackCommand.RevertToRecordId,
                    UserId = command.UserId
                },
                _userContext);
        }
    }
}
