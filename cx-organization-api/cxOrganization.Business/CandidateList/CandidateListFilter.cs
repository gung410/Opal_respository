using System.Collections.Generic;
using System.Linq;

namespace cxOrganization.Business.CandidateList
{
    public static class CandidateListFilter
    {
       public static List<CandidateListItem> FilterCandidateListItemsByJobFamilies(List<string> filteredJobCodes, bool includeJobmatch, List<CandidateListItem> candidateListItems)
        {
            if (includeJobmatch && filteredJobCodes != null && filteredJobCodes.Count > 0)
            {
                candidateListItems = candidateListItems.Where(c => c.Jobmatches != null && c.Jobmatches.Any(j =>
                                                                       (j.MatchRate == MatchRate.Good
                                                                        || j.MatchRate == MatchRate.Decent) &&
                                                                       filteredJobCodes.Contains(j.Code))).ToList();
            }
            return candidateListItems;
        }
    }
}