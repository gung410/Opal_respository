

using System.Collections.Generic;

namespace cxOrganization.Business.CandidateList
{
    public class ProfileLetters
    {
        public string Profile { get; set; }
        public string RawLetters { get; set; }
        public List<ProfileLetterItem> Letters { get; set; }

    }
}