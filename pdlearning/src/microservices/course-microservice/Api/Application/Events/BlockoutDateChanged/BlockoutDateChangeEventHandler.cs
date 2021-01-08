using Conexus.Opal.Connector.RabbitMQ;
using Conexus.Opal.Connector.RabbitMQ.Core;
using Conexus.Opal.Connector.RabbitMQ.Extensions;
using Microservice.Course.Domain.Entities;
using Microsoft.Extensions.Options;
using Thunder.Platform.Core.Context;

namespace Microservice.Course.Application.Events
{
    public class BlockoutDateChangeEventHandler : OpalMQEventHandler<BlockoutDateChangeEvent, BlockoutDate>
    {
        public BlockoutDateChangeEventHandler(IOptions<RabbitMQOptions> options, IOpalMessageProducer producer, IUserContext userContext) : base(options, producer, userContext)
        {
        }

        protected override BlockoutDate GetMQMessagePayload(BlockoutDateChangeEvent @event)
        {
            return @event.BlockoutDate;
        }
    }
}
