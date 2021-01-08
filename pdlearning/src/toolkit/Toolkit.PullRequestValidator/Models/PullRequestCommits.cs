using System.Collections.ObjectModel;
using Newtonsoft.Json;

namespace Conexus.Toolkit.PullRequestValidator.Models
{
    public class PullRequestCommits
    {
        [JsonProperty("count")]
        public int Count { get; set; }

        [JsonProperty("value")]
        public Collection<Commit> Value { get; set; }
    }
}
