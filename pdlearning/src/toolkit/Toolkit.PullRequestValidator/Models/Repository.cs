using System;
using Newtonsoft.Json;

namespace Conexus.Toolkit.PullRequestValidator.Models
{
    public class Repository
    {
        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("url")]
        public Uri Url { get; set; }
    }
}
