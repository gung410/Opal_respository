using System;
using Microservice.WebinarAutoscaler.Domain.Enums;
using Thunder.Platform.Core.Domain.Auditing;

namespace Microservice.WebinarAutoscaler.Domain.Entities
{
    public class BBBServer : AuditedEntity
    {
        public BBBServer()
        {
            this.Id = Guid.NewGuid();
        }

        /// <summary>
        ///  IsProtection: Used to turn on/off termination protection Auto scale group.
        /// </summary>
        public bool IsProtection { get; set; }

        public string PrivateIp { get; set; }

        /// <summary>
        ///  InstanceId: BBB server's AWS id.
        /// </summary>
        public string InstanceId { get; internal set; }

        /// <summary>
        ///  TargetGroupArn: being id of targetGroup.
        /// </summary>
        public string TargetGroupArn { get; set; }

        /// <summary>
        ///  RuleArn: being id of rule which is belong to ALB listener.
        /// </summary>
        public string RuleArn { get; set; }

        /// <summary>
        ///  Status: Used to check BBB server's status.
        /// </summary>
        public BBBServerStatus Status { get; set; }
    }
}
