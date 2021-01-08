using System;
using System.Collections.Generic;
using Conexus.Opal.Connector.RabbitMQ.Contract;

namespace Conexus.Opal.BrokenLinkChecker
{
    public class EnqueueUrlMessage : IMQMessage
    {
        /// <summary>
        /// The id of object, where object is your content like digital content/course lecture/course section.
        /// </summary>
        public Guid ObjectId { get; set; }

        /// <summary>
        /// Optional property. Used to support multiple version features of the object.
        /// </summary>
        public Guid? OriginalObjectId { get; set; }

        /// <summary>
        /// Used to support CAM get report from class run.
        /// </summary>
        public Guid? ParentId { get; set; }

        public List<string> Urls { get; set; }

        public ModuleIdentifier Module { get; set; }

        public string ObjectDetailUrl { get; set; }

        public string ObjectTitle { get; set; }

        public Guid ObjectOwnerId { get; set; }

        public string ObjectOwnerName { get; set; }

        public string EmailActionUrl { get; set; }

        public string EmailActionName { get; set; }

        public BrokenLinkContentType ContentType { get; set; }
    }
}
