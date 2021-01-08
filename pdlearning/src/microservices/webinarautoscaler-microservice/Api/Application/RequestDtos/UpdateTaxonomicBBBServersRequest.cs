using System.Collections.Generic;

namespace Microservice.WebinarAutoscaler.Application.RequestDtos
{
    public class UpdateTaxonomicBBBServersRequest
    {
        public List<string> RegisteredServerIps { get; set; }

        public List<string> UnRegisteredServerIps { get; set; }
    }
}
