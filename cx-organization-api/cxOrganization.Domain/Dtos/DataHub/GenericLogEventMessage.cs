using cxOrganization.Client.UserGroups;
using System;

namespace cxOrganization.Domain.Dtos.DataHub
{
    public class GenericLogEventMessage
    {
        /// <summary>
        /// The info of the executor (who is the owner of the event).
        /// </summary>
        public ExecutorInfo Executor { get; set; }

        /// <summary>
        /// The approval group info which is the Approving Officer.
        /// This field should be available for two specific events (i.e: <see cref="AuditLogEvents.ApprovalGroupMemberAdded"/> and <see cref="AuditLogEvents.ApprovalGroupMemberRemoved"/>)
        /// </summary>
        public ApprovalGroupInfo ApprovalGroupInfo { get; set; }

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
        public dynamic Body { get; set; }
    }


    public class Identity
    {
        public string ClientId { get; set; }
        public string CustomerId { get; set; }
        public string UserId { get; set; }
        public string SourceIp { get; set; }
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

    public class ExecutorInfo : PersonInfo
    {
        public string ExtId { get; set; }
    }

    public class ApprovalGroupInfo : PersonInfo
    {
        public int ApprovalGroupId { get; set; }
        public GrouptypeEnum Type { get; set; }
    }

    public class PersonInfo
    {
        public string FullName { get; set; }
        public string AvatarUrl { get; set; }
    }
}
