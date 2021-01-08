using System;
using Conexus.Opal.Connector.RabbitMQ.Contract;

namespace Conexus.Opal.BrokenLinkChecker
{
    public class DequeueObjectMessage : IMQMessage
    {
        /// <summary>
        /// The id of object, where object is your content like digital content/course lecture/course section.
        /// </summary>
        public Guid ObjectId { get; set; }
    }
}
