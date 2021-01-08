using System;
using Microservice.WebinarAutoscaler.Domain.Enums;

namespace Microservice.WebinarAutoscaler.Application.Models
{
    public class BBBServerModel
    {
        public Guid Id { get; set; }

        public bool IsProtection { get; set; }

        public string PrivateIp { get; set; }

        public string InstanceId { get; internal set; }

        public string TargetGroupArn { get; set; }

        public int ParticipantCount { get; set; }

        public string RuleArn { get; set; }

        public BBBServerStatus Status { get; set; }
    }
}
