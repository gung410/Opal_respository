using Datahub.Queue.Manager.Commands.QueueConfigurations;
using Datahub.Queue.Manager.Domains;
using System.Collections.Generic;

namespace Datahub.Queue.Manager.Services
{
    public interface IMappingService
    {
        QueueConfiguration ToQueueConfiguration(CreateQueueConfigurationCommand command);
        IEnumerable<QueueConfiguration> ToQueueConfigurations(IEnumerable<CreateQueueConfigurationCommand> commands);
        QueueConfiguration ToQueueConfiguration(EditQueueConfigurationCommand command);
        QueueConfiguration ToQueueConfiguration(QueueConfiguration dataPoint, EditQueueConfigurationCommand command);
    }
}