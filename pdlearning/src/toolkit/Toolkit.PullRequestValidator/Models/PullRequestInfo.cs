using System;
using Newtonsoft.Json;

namespace Conexus.Toolkit.PullRequestValidator.Models
{
    public class PullRequestInfo
    {
        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("subcriptionId")]
        public string SubscriptionId { get; set; }

        [JsonProperty("resource")]
        public Resource Resource { get; set; }

        [JsonProperty("createdDate")]
        public DateTime CreatedDate { get; set; }
    }
}
