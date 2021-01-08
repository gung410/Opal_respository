using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Text;

namespace Communication.Business.Models.Event
{

    public class CommunicationEvent
    {
        public CommunicationEvent()
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
        public Body Body { get; set; }
    }

    public class Body
    {
        public string UserId { get; set; }
        public string RegistrationToken { get; set; }
        public string DeviceId { get; set; }
        public string Platform { get; set; }
        public string NotificationId { get; set; }
        public string ItemId { get; set; }
    }

    

    public enum Channel
    {
        Default,
        Email,
        SMS,
        System
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
