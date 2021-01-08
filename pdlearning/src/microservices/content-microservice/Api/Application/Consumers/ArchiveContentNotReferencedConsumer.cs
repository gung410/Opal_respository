using System;
using System.Threading.Tasks;
using Conexus.Opal.Connector.RabbitMQ.Core;
using Microservice.Content.Application.Commands;
using Thunder.Platform.Cqrs;

namespace Microservice.Content.Application.Consumers
{
    [OpalConsumer("microservice.events.content.courses-not-referenced")]
    public class ArchiveContentNotReferencedConsumer : ScopedOpalMessageConsumer<ArchiveContentNotReferencedMessage>
    {
        public async Task InternalHandleAsync(
            ArchiveContentNotReferencedMessage message,
            IThunderCqrs thunderCqrs)
        {
            foreach (var contentId in message.ObjectIds)
            {
                var command = new ArchiveDigitalContentCommand
                {
                    ContentId = contentId,
                    ArchiveBy = Guid.Empty
                };

                await thunderCqrs.SendCommand(command);
            }
        }
    }
}
