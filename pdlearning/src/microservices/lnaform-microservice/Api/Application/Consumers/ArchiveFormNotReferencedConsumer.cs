using System;
using System.Threading.Tasks;
using Conexus.Opal.Connector.RabbitMQ.Core;
using Microservice.LnaForm.Application.Commands;
using Thunder.Platform.Cqrs;

namespace Microservice.LnaForm.Application.Consumers
{
    [OpalConsumer("microservice.events.form.courses-not-referenced")]
    public class ArchiveFormNotReferencedConsumer : ScopedOpalMessageConsumer<ArchiveFormNotReferencedMessage>
    {
        public async Task InternalHandleAsync(
            ArchiveFormNotReferencedMessage message,
            IThunderCqrs thunderCqrs)
        {
            foreach (var formId in message.ObjectIds)
            {
                var command = new ArchiveFormCommand
                {
                    FormId = formId,
                    ArchiveBy = Guid.Empty
                };

                await thunderCqrs.SendCommand(command);
            }
        }
    }
}
