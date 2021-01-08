using Communication.Business.Exceptions;
using Communication.Business.Models.FirebaseCloudMessage;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Communication.Business.HttpClients
{
    public class GoogleAppInstanceServerHttpClient : IGoogleAppInstanceServerHttpClient
    {
        private readonly HttpClient _client;
        private readonly ILogger _logger;
        private const string BatchRemoveUrl = "iid/v1:batchRemove";
        private const string AddRemoveUrl = "iid/v1:batchAdd";
        public GoogleAppInstanceServerHttpClient(HttpClient httpClient, IOptions<FireBaseConfig> options, ILogger<GoogleAppInstanceServerHttpClient> logger)
        {
            httpClient.BaseAddress = new Uri(options.Value.BaseAppInstanceServerUrl);
            httpClient.DefaultRequestHeaders.TryAddWithoutValidation("Authorization", $"key={options.Value.FirebaseCloudMessageLegacyServerKey}");
            _client = httpClient;
            _logger = logger;
        }

        public async Task<string[]> GetAppInstanceRelationsAsync(string registrationToken)
        {
            var response = _client.GetAsync($"iid/info/{registrationToken}?details=true");
            var bodyResponse = await response.EnsureSuccessStatusCodeAsync();

            var appInstanceInformation = JsonConvert.DeserializeObject<FirebaseAppInstancesInfo>(await bodyResponse.ReadAsStringAsync());
            if (appInstanceInformation.Rel != null)
            {
                return appInstanceInformation.Rel.Topics.Properties().Select(t => t.Name).ToArray();
            }
            return await Task.FromResult(Array.Empty<string>());
        }



        public async Task RemoveRelationshipMapAsync(ISet<string> instanceIdTokens, string topicName)
        {
            var httpContent = GetBatchRelationshipsPayload(topicName, instanceIdTokens);
            var responseTask = _client.PostAsync(BatchRemoveUrl, httpContent);
            await responseTask.EnsureSuccessStatusCodeAsync();
        }

        private static StringContent GetBatchRelationshipsPayload(string topicName, ISet<string> instanceIdTokens)
        {
            var payloadObject = new FirebaseRelationshipBatchPayload()
            {
                To = string.Format("/topics/{0}", topicName),
                RegistrationTokens = instanceIdTokens
            };
            var json = JsonConvert.SerializeObject(payloadObject, new JsonSerializerSettings());
            return new StringContent(json, Encoding.UTF8, "application/json");
        }

        public async Task AddRelationshipMapAsync(ISet<string> instanceIdTokens, string topicName)
        {
            var httpContent = GetBatchRelationshipsPayload(topicName, instanceIdTokens);
            var responseTask = _client.PostAsync(AddRemoveUrl, httpContent);
            await responseTask.EnsureSuccessStatusCodeAsync();
        }

        public async Task ValidateToken(string instanceIdToken)
        {
            var response = _client.GetAsync($"iid/info/{instanceIdToken}");
            await response.EnsureSuccessStatusCodeAsync();
        }
    }
}
