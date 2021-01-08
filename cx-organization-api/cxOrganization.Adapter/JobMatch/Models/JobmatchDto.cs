using System.Collections.Generic;

namespace cxOrganization.Adapter.JobMatch.Models
{
    public class JobmatchDto
    {
        public string Code { get; set; }
        public string Title { get; set; }
        public string Riasec { get; set; }
        public List<KeyValuePair<string, string>> Texts { get; set; }
        public List<KeyValuePair<string, string>> Properties { get; set; }
        public ScoreDto Score { get; set; }
    }
}