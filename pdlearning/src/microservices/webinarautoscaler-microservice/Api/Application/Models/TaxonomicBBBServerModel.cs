using System.Collections.Generic;

namespace Microservice.WebinarAutoscaler.Application.Models
{
    public class TaxonomicBBBServerModel
    {
        public string WebinarDns { get; set; }

        public IEnumerable<string> AvailableBBBServerIps { get; set; }

        public IEnumerable<string> RedundantBBBServerIps { get; set; }
    }
}
