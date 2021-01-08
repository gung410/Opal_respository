using System.Collections.Generic;

namespace Microservice.WebinarAutoscaler.Application.RequestDtos
{
    public class UpdateBBBProtectionStateRequest
    {
        public bool IsProtection { get; set; }

        public string InstanceId { get; set; }
    }
}
