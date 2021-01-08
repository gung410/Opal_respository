namespace cxOrganization.Business.CandidateList
{
    public class CandidateJobmatch
    {
        public string Code { get; set; }
        public string Riasec { get; set; }
        public string MatchRiasec { get; set; }
        public MatchRate MatchRate { get; set; }
        public JobmatchScoreDto Score { get; set; }

    }
}