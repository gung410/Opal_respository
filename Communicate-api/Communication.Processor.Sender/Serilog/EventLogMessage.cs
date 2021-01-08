using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Threading.Tasks;

namespace Communication.Processor.Sender.Serilog
{
    public class EventLogMessage
    {
        public EventLogMessage()
        {
            Type = "event";
            Version = "1.0";
            Id = Guid.NewGuid().ToString();
            Created = DateTime.Now;
            Routing = new Routing();
            Payload = new Payload();
        }
        public string Type { get; set; }
        public string Version { get; set; }
        public string Id { get; set; }
        public DateTime Created { get; set; }
        public Routing Routing { get; set; }
        public Payload Payload { get; set; }

        public string ToJson()
        {
            var settings = new JsonSerializerSettings
            {
                ContractResolver = new CamelCasePropertyNamesContractResolver(),
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore
            };
            settings.Converters.Add(new StringEnumConverter());
            return JsonConvert.SerializeObject(this, settings);
        }

    }

    public class Routing
    {
        public Routing()
        {
            ActionVersion = "1.0";
        }
        public string Action { get; set; }
        public string ActionVersion { get; set; }
        public string Entity { get; set; }
        public string EntityId { get; set; }
    }

    public class Payload
    {
        public Payload()
        {
            Identity = new Identity();
            References = new References();
            Body = new ExpandoObject();
        }
        public Identity Identity { get; set; }
        public References References { get; set; }
        public dynamic Body { get; set; }
    }

    public class Identity
    {
        public string ClientId { get; set; }
        public string CustomerId { get; set; }
        public string UserId { get; set; }

    }

    public class References
    {
        public string CorelationId { get; set; }
    }
}
