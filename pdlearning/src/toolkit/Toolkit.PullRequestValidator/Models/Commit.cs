using Newtonsoft.Json;

namespace Conexus.Toolkit.PullRequestValidator.Models
{
    public class Commit
    {
        [JsonProperty("commitId")]
        public string CommitId { get; set; }

        [JsonProperty("url")]
        public string Url { get; set; }
    }
}
