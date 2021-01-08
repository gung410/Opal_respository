using Communication.Business.Exceptions;
using Communication.Business.Models.FirebaseCloudMessage;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Communication.Business.HttpClients
{
    public class FirebaseCloudMessageHttpClient : IFirebaseCloudMessageHttpClient
    {
        private readonly HttpClient _client;
        private readonly ILogger _logger;
        private const string SendingMessageUrl = "fcm/send";
        private readonly FireBaseConfig _config;
        public FirebaseCloudMessageHttpClient(HttpClient httpClient, IOptions<FireBaseConfig> options, ILogger<GoogleAppInstanceServerHttpClient> logger)
        {
            httpClient.BaseAddress = new Uri(options.Value.BaseFirebaseClouldMessageUrl);
            httpClient.DefaultRequestHeaders.TryAddWithoutValidation("Authorization", $"key={options.Value.FirebaseCloudMessageLegacyServerKey}");
            _client = httpClient;
            _config = options.Value;
            _logger = logger;
        }

        public async Task SendNotificationAsync(dynamic message)
        {
            var httpContent = GetNotificationPayload(message);
            Task<HttpResponseMessage> responseTask = _client.PostAsync(SendingMessageUrl, httpContent);
            await responseTask.EnsureSuccessStatusCodeAsync();
        }
        private static StringContent GetNotificationPayload(FirebaseNotificationMessage message)
        {
            var json = JsonConvert.SerializeObject(message,
                                                   Formatting.None,
                                                   new JsonSerializerSettings
                                                   {
                                                       NullValueHandling = NullValueHandling.Ignore
                                                   });
            return new StringContent(json, Encoding.UTF8, "application/json");
        }



    }
}
