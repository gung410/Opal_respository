using Newtonsoft.Json;

namespace Microservice.Learner.Application.Models
{
    public class ScormStateModel
    {
        [JsonProperty("completion_status")]
        public bool CompletionStatus { get; set; }

        [JsonProperty("success_status")]
        public bool SuccessStatus { get; set; }

        [JsonProperty("total_time")]
        public int TotalTime { get; set; }
    }
}
