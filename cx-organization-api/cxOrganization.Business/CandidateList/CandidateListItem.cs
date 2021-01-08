using System;
using System.Collections.Generic;
using cxPlatform.Client.ConexusBase;

namespace cxOrganization.Business.CandidateList
{
    public class CandidateListItem
    {
        public IdentityDto Identity { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public int? MobileCountryCode { get; set; }
        public string MobileNumber { get; set; }
        public string EmailAddress { get; set; }
        public short? Gender { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public List<ProfileLetters> ProfileLetters { get; set; }
        public List<CandidateJobmatch> Jobmatches { get; set; }
        public List<ContentItem> ContentItems { get; set; }
        public string FullName
        {
            get
            {
                return string.Format("{0} {1}", FirstName, LastName);
            }
        }

        public int JobmatchTotalRate { get; set; }//TODO: for term
        public DateTime? Connected { get; set; }

        public int? CvCompleteness { get; set; }
    }
}