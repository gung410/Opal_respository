using Newtonsoft.Json;
namespace Communication.Business.Models.FirebaseCloudMessage
{
  
    public class FirebaseNotificationMessage 
    {
        [JsonProperty("to")]
        public string To { get; set; }

        [JsonProperty("condition")]
        public string Condition { get; set; }

        [JsonProperty("registration_ids")]
        public string RegistrationIds { get; set; }

        [JsonProperty("priority")]
        public string Priority { get; set; }

        [JsonProperty("content_available")]
        public bool ContentAvailable { get; set; }

        [JsonProperty("notification")] 
        public NotificationPayload Notification { get; set; }
         [JsonProperty("data")] 
        public object Data { get; set; }
    }
    public class NotificationPayload
    {
        [JsonProperty("title")]
        public string Title { get; set; }

        [JsonProperty("body")]
        public string Body { get; set; }

        [JsonProperty("sound")]
        public string Sound { get; set; }

        [JsonProperty("badge")]
        public string Badge { get; set; }

        [JsonProperty("click_action")]
        public string ClickAction { get; set; }

        [JsonProperty("subtitle")]
        public string Subtitle { get; set; }

        [JsonProperty("icon")]
        public string Icon { get; set; }
    }
}