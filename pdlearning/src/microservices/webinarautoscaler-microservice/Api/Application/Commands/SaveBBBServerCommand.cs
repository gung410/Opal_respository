using System.Collections.Generic;
using Microservice.WebinarAutoscaler.Domain.Enums;
using Thunder.Platform.Cqrs;

namespace Microservice.WebinarAutoscaler.Application.Commands
{
    public class SaveBBBServerCommand : BaseThunderCommand
    {
        public bool IsProtection { get; set; }

        public string PrivateIp { get; set; }

        public BBBServerStatus Status { get; set; }

        public string InstanceId { get; internal set; }

        public string TargetGroupArn { get; set; }

        public string RuleArn { get; set; }
    }
}
