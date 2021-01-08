using System;
using System.Threading.Tasks;
using Conexus.Opal.Connector.RabbitMQ.Core;
using Microservice.StandaloneSurvey.Application.Commands;
using Thunder.Platform.Cqrs;

namespace Microservice.StandaloneSurvey.Application.Consumers
{
    [OpalConsumer("microservice.events.standalone-survey.courses-not-referenced")]
    public class ArchiveSurveyNotReferencedConsumer : ScopedOpalMessageConsumer<ArchiveFormNotReferencedMessage>
    {
        public async Task InternalHandleAsync(
            ArchiveFormNotReferencedMessage message,
            IThunderCqrs thunderCqrs)
        {
            foreach (var formId in message.ObjectIds)
            {
                var command = new ArchiveSurveyCommand
                {
                    FormId = formId,
                    ArchiveBy = Guid.Empty
                };

                await thunderCqrs.SendCommand(command);
            }
        }
    }
}
