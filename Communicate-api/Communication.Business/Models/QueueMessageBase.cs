using Communication.Business.Models.Command;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Text;

namespace Communication.Business.Models
{
    public class QueueMessageBase
    {
        public QueueMessageBase()
        {
        }
        public string Type { get; set; }
        public string Version { get; set; }
        public string Id { get; set; }
        public DateTime Created { get; set; }
        public Routing Routing { get; set; }
        public Payload Payload { get; set; }
    }

    public class Payload
    {
        public Identity Identity { get; set; }
        public References References { get; set; }
        public dynamic Body { get; set; }
    }

    public class MessageBody
    {
        public Channel Channel { get;set; }
        public string DisplayMessage { get; set; }
        public string Subject { get; set; }
        public TemplateData TemplateData { get; set; }
        public Dictionary<string, object> Data { get; set; }
    }

    public class Identity
    {
        public string ClientId { get; set; }
        public string CustomerId { get; set; }
        public string UserId { get; set; }
    }

    public class References
    {
        public string CorrelationId { get; set; }
    }

    public class Routing
    {
        public string Action { get; set; }
        public string ActionVersion { get; set; }
        public string Entity { get; set; }
        public string EntityId { get; set; }
    }
}
