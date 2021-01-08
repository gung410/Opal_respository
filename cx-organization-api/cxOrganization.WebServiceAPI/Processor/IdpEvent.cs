using cxOrganization.Domain.Dtos.DataHub;
using System;

namespace cxOrganization.WebServiceAPI.Processor.Event
{
    public class IdpEvent
    {
        public IdpEvent()
        {
        }
        public string Type { get; set; }
        public string Version { get; set; }
        public string Id { get; set; }
        public DateTime Created { get; set; }
        public Routing Routing { get; set; }
        public Payload Payload { get; set; }
    }
}
