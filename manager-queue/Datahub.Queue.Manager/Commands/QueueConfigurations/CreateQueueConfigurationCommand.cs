using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Datahub.Queue.Manager.Commands.QueueConfigurations
{
    public class CreateQueueConfigurationCommand
    {
        public string Exchange { get; set; }
        public string ExchangeType { get; set; }
        public string Queue { get; set; }
        public string RoutingKey { get; set; }
    }
}
