using System.Collections.Generic;
using System.Linq;

namespace cxOrganization.Business.CandidateList
{
    public static class CandidateListCalculator
    {
        public static int CalculateTotalJobmatchRate(List<CandidateJobmatch> candidateJobmatches, List<string> filteredJobCodes)
        {
            if (candidateJobmatches == null || candidateJobmatches.Count == 0) return 0;
            if (filteredJobCodes == null || filteredJobCodes.Count == 0)
                return candidateJobmatches.Sum(j => (int)j.MatchRate);
            return candidateJobmatches.Where(c => filteredJobCodes.Contains(c.Code)).Sum(c => (int)c.MatchRate);
        }

        public static CandidateListSummary CalculateCandidateListSummary (List<CandidateListItem> candidateListItems, List<string> filteredJobCodes)
        {
            var summary = new CandidateListSummary()
            {
                TotalItems = candidateListItems.Count
            };

            int candidateAllGood;
            int candidateAllDescent;
            int candidateAllGoodAndDescent;

            SummarizeMatchRate(candidateListItems, filteredJobCodes, out candidateAllGood, out candidateAllDescent, out candidateAllGoodAndDescent);

            summary.AllGood = candidateAllGood;
            summary.AllDescent = candidateAllDescent;
            summary.AllGoodAndDescent = candidateAllGoodAndDescent;

            return summary;
        }

        public static void SummarizeMatchRate(
            List<CandidateListItem> candidateListItems,
            List<string> filteredJobCodes,
            out int candidateAllGood,
            out int candidateAllDescent,
            out int candidateAllGoodAndDescent)
        {
            candidateAllGood = 0;
            candidateAllDescent = 0;
            candidateAllGoodAndDescent = 0;
            //We only need to summarize when there is filter on job family

            if (filteredJobCodes != null
                && filteredJobCodes.Count > 0)
            {
                foreach (var candidateListItem in candidateListItems)
                {
                    if (candidateListItem.Jobmatches == null) continue;

                    //We only calculate on job that match with filtering job-family
                    var caculatingJobMatches = candidateListItem.Jobmatches.Where(j => filteredJobCodes.Contains(j.Code)).ToList();
                    if (!caculatingJobMatches.Any()) continue;

                    if (caculatingJobMatches.All(j => j.MatchRate == MatchRate.Good))
                    {

                        candidateAllGood++;
                        continue;

                    }

                    if (caculatingJobMatches.All(j => j.MatchRate == MatchRate.Decent))
                    {
                        candidateAllDescent++;
                        continue;
                    }
                    if (caculatingJobMatches.Any(j => j.MatchRate == MatchRate.Good)
                        && caculatingJobMatches.Any(j => j.MatchRate == MatchRate.Decent))
                    {
                        candidateAllGoodAndDescent++;
                    }
                }
            }
        }
    }
}