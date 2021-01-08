using cxPlatform.Client.ConexusBase;

namespace cxOrganization.Business.Connection
{
    /// <summary>
    /// Definition of event type name for connection when changing status
    /// </summary>
    public class ConnectionEventTypeConfig
    {
        /// <summary>
        /// The entity status which connection changed from. Null it means add new connection
        /// </summary>
        public EntityStatusEnum? FromStatus { get; set; }
        /// <summary>
        /// The entity status which connection changed to. 
        /// </summary> 
        public EntityStatusEnum ToStatus { get; set; }

        /// <summary>
        /// The event type name when connection changed status from FromStatus to ToStatus
        /// </summary> 
        public string EventTypeName { get; set; }
    }
}