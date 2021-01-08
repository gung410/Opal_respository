using System.Collections.Generic;

namespace cxOrganization.Business.CandidateList
{
    public class CandidateListDto
    {
        public int PageIndex { get; set; }
        public int PageSize { get; set; }
        public bool HasMoreItem { get; set; }
        public List<CandidateListItem> Items { get; set; }
      
        public CandidateListSummary Summary { get; set; }
    }
}