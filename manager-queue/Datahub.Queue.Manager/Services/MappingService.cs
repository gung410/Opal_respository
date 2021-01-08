using System.Collections.Generic;
using Datahub.Queue.Manager.Commands.QueueConfigurations;
using Datahub.Queue.Manager.Domains;

namespace Datahub.Queue.Manager.Services
{
    public class MappingService : IMappingService
    {
        public QueueConfiguration ToQueueConfiguration(CreateQueueConfigurationCommand command)
        {
            return new QueueConfiguration()
            {
                Exchange = command.Exchange,
                ExchangeType = command.ExchangeType,
                Queue = command.Queue,
                RoutingKey = command.RoutingKey
            };
        }

        public QueueConfiguration ToQueueConfiguration(EditQueueConfigurationCommand command)
        {
            return new QueueConfiguration()
            {
                Exchange = command.Exchange,
                ExchangeType = command.ExchangeType,
                Queue = command.Queue,
                RoutingKey = command.RoutingKey
            };
        }

        public QueueConfiguration ToQueueConfiguration(QueueConfiguration queueConfiguration, EditQueueConfigurationCommand command)
        {
            queueConfiguration.Exchange = command.Exchange;
            queueConfiguration.ExchangeType = command.ExchangeType;
            queueConfiguration.Queue = command.Queue;
            queueConfiguration.RoutingKey = command.RoutingKey;

            return queueConfiguration;
        }

        public IEnumerable<QueueConfiguration> ToQueueConfigurations(IEnumerable<CreateQueueConfigurationCommand> commands)
        {
            List<QueueConfiguration> result = new List<QueueConfiguration>();
            foreach (var command in commands)
            {
                result.Add(new QueueConfiguration()
                {
                    Exchange = command.Exchange,
                    ExchangeType = command.ExchangeType,
                    Queue = command.Queue,
                    RoutingKey = command.RoutingKey
                });
            }

            return result;
        }
    }
}
