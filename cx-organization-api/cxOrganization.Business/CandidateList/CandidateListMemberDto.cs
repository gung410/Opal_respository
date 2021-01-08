
using cxOrganization.Business.Connection;

namespace cxOrganization.Business.CandidateList
{
    public class CandidateListMemberDto
    {
        public ConnectionMemberDto ConnectionMember { get; set; }
        public int CvCompleteness { get; set; }
    }
}
