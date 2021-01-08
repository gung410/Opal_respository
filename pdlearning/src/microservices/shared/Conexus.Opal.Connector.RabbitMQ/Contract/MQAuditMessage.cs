using System;

namespace Conexus.Opal.Connector.RabbitMQ.Contract
{
    public enum AuditLogActionType
    {
        Unknown,
        Created,
        Deleted,
        Updated
    }

    public class MQAuditMessage<TBody> : IMQMessage
    {
        public const string DefaultRoutingKey = "auditlog";

        /// <summary>
        /// Message Id.
        /// </summary>
        public Guid Id { get; set; } = Guid.NewGuid();

        /// <summary>
        /// Friendly name of an message (human readable, e.g. Course Created).
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// The type of action to help auditor easily find the message.
        /// </summary>
        public AuditLogActionType ActionType { get; set; }

        /// <summary>
        /// The origin of the sender.
        /// This could be a module name such as LMM, CAM, Learner.
        /// </summary>
        public string ModuleName { get; set; }

        /// <summary>
        /// The service name could course-service, learner-service, etc.
        /// </summary>
        public string ServiceName { get; set; }

        /// <summary>
        /// The origin IP.
        /// </summary>
        public string OriginIp { get; set; }

        public string UserId { get; set; }

        public TBody Body { get; set; }

        public DateTime Created { get; set; } = DateTime.UtcNow;
    }
}
