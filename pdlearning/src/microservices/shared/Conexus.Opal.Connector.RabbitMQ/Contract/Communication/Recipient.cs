using System;
using System.Collections.Generic;

namespace Conexus.Opal.Connector.RabbitMQ.Contract
{
    public class Recipient
    {
        public HashSet<string> UserIds { get; set; }

        #pragma warning disable SA1300 // Element should begin with upper-case letter
        public string cxToken { get; set; }
        #pragma warning restore SA1300 // Element should begin with upper-case letter
    }
}
