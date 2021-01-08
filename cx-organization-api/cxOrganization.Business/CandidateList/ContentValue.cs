using cxPlatform.Client.ConexusBase;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace cxOrganization.Business.CandidateList
{
    [DataContract]
    public class ContentValue
    {
      
        [DataMember]
        public long AnswerId { get; set; }

        [DataMember]
        public long QuestionId { get; set; }

        [DataMember]
        public long AlternativeId { get; set; }
        [DataMember]
        public string AlternativeExtId { get; set; }

        [DataMember]
        public string Value { get; set; }


   
    
    }
}