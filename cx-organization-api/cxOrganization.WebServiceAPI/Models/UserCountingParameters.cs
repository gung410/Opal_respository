using cxPlatform.Client.ConexusBase;
using System.Collections.Generic;

namespace cxOrganization.WebServiceAPI.Models
{
    public class UserCountingParameters
    {
        public List<int> UserGroupIds { get; set; }
        public List<int> UserIds { get; set; }
        public List<ArchetypeEnum> UserTypeArchetypes { get; set; }
    }
}
