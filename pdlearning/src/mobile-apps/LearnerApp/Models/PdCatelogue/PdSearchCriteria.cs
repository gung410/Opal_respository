using System.Collections.Generic;
using Newtonsoft.Json;

namespace LearnerApp.Models.PdCatelogue
{
    public class PdSearchCriteria
    {
        [JsonProperty("tags.id")]
        public List<string> Tags { get; set; }

        public List<string> ResourceType { get; set; }

        public string[] Status { get; set; } = new string[] { "contains", "published" };

        public string[] RegistrationMethod { get; set; }

        public string[] StartDate { get; set; }

        public string[] ExpiredDate { get; set; }

        public string[] IsArchived { get; set; }
    }
}
