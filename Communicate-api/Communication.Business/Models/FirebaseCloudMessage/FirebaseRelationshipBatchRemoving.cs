namespace Communication.Business.Models.FirebaseCloudMessage
{
    using System.Collections.Generic;
    using Newtonsoft.Json;

    public class FirebaseRelationshipBatchPayload
    {
        [JsonProperty("to")]
        public string To { get; set; }
        [JsonProperty("registration_tokens")]
        public ISet<string> RegistrationTokens { get; set; }
    }
}