using Conexus.Opal.Connector.RabbitMQ;
using Conexus.Opal.Connector.RabbitMQ.Core;
using Conexus.Opal.Connector.RabbitMQ.Extensions;
using Microservice.Content.Application.Models;
using Microsoft.Extensions.Options;
using Thunder.Platform.Core.Context;

namespace Microservice.Content.Application.Events.CheckHasReferenceToResource
{
    public class CheckHasReferenceToResourceEventHandler : OpalMQEventHandler<CheckHasReferenceToResourceEvent, CheckHasReferenceToResourceModel>
    {
        public CheckHasReferenceToResourceEventHandler(IOptions<RabbitMQOptions> options, IOpalMessageProducer producer, IUserContext userContext) : base(options, producer, userContext)
        {
        }

        protected override CheckHasReferenceToResourceModel GetMQMessagePayload(CheckHasReferenceToResourceEvent @event)
        {
            return @event.Model;
        }
    }
}
