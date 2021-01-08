using System.Collections.Generic;

namespace Microservice.Learner.Application
{
    public class TrackingEventOptions
    {
        public const string TrackingEvent = "TrackingEventOptions";

        public string[] AllowEvents { get; set; }

        public Dictionary<string, string> EventMapSubRouting { get; set; }
    }
}
