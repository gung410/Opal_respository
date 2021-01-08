using cxPlatform.Client.ConexusBase;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace cxOrganization.Business.Connection
{
    public class ConnectionTypeConfig
    {
        public ConnectionTypeConfig()
        {
            EventTypeConfigs = new ReadOnlyCollection<ConnectionEventTypeConfig>(new List<ConnectionEventTypeConfig>());
        }

        public ConnectionType? ConnectionType { get; set; }
        public IReadOnlyCollection<ConnectionEventTypeConfig> EventTypeConfigs { get; set; }
    }
    public class ConnectionConfig
    {
        public ConnectionConfig()
        {
            ConnectionTypeConfigs = new ReadOnlyCollection<ConnectionTypeConfig>(new List<ConnectionTypeConfig>());
            AcceptedStatusesForEditting=new List<EntityStatusEnum>();
            AcceptedStatusesForGetting = new List<EntityStatusEnum>();
        }

        public IReadOnlyCollection<ConnectionTypeConfig> ConnectionTypeConfigs { get; set; }

        public List<EntityStatusEnum> AcceptedStatusesForGetting { get; set; }
        public List<EntityStatusEnum> AcceptedStatusesForEditting { get; set; }

        public bool GetMemberInGetTimeAsDefault { get; set; }
    }
}