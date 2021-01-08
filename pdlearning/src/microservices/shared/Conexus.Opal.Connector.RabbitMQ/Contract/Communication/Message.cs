using System;
using System.Collections.Generic;

namespace Conexus.Opal.Connector.RabbitMQ.Contract
{
    public class Message
    {
        public string Subject { get; set; }

        public string DisplayMessage { get; set; }
    }
}
