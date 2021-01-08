using Newtonsoft.Json;

namespace LearnerApp.Models.PdCatelogue
{
    public class PdSearchCriteriaOr
    {
        public string[] CreatedBy { get; set; }

        [JsonProperty("memberships.id")]
        public string[] MembershipsId { get; set; }
    }
}
