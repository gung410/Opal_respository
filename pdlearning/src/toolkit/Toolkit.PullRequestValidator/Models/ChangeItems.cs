using Newtonsoft.Json;

namespace Conexus.Toolkit.PullRequestValidator.Models
{
    public class ChangeItems
    {
        [JsonProperty("item")]
        public Item Item { get; set; }

        [JsonProperty("changeType")]
        public string ChangeType { get; set; }
    }
}
