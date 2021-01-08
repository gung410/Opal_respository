using cxPlatform.Client.ConexusBase;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace cxOrganization.Business.CandidateList
{
    [DataContract]
    public class ContentItem
    {
        [DataMember]
        public IdentityDto Identity { get; set; }

        [DataMember]
        public IdentityDto AssessmentIdentity { get; set; }
        [DataMember]
        public int StatusTypeId { get; set; }


        [DataMember]
        public string ContentName { get; set; }
        [DataMember]
        public string ContentDisplayName { get; set; }


        [DataMember]
        public List<ContentValue> Values { get; set; }      

        [IgnoreDataMember]
        public IdentityDto UserIdenity { get; set; }
    
    }
}