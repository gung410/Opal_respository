using System;
using Newtonsoft.Json;

namespace Conexus.Toolkit.PullRequestValidator.Models
{
    public class Item
    {
        [JsonProperty("objectId")]
        public string ObjectId { get; set; }

        [JsonProperty("commitId")]
        public string CommitId { get; set; }

        [JsonProperty("path")]
        public string Path { get; set; }

        [JsonProperty("isFolder")]
        public bool IsFolder { get; set; }

        [JsonProperty("url")]
        public Uri Url { get; set; }
    }
}
