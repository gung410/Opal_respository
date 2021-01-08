using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Thunder.Platform.Core.Json;

namespace Conexus.Opal.Microservice.Infrastructure
{
    public class HttpService
    {
        private readonly IHttpClientFactory _httpClientFactory;

        public HttpService(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        public Task<T> GetAsync<T>(
            string requestUrl,
            List<KeyValuePair<string, string>> queryParams = null,
            string bearerToken = null,
            List<KeyValuePair<string, string>> requestHeaders = null)
        {
            var urlQueryParamsPart = queryParams != null
                ? string.Join("&", queryParams.Select(p => $"{p.Key}={WebUtility.UrlEncode(p.Value)}"))
                : string.Empty;
            var requestUrlWithQueryParams = !string.IsNullOrEmpty(urlQueryParamsPart)
                ? (!requestUrl.Contains("?") ? $"{requestUrl}?{urlQueryParamsPart}" : $"{requestUrl}&{urlQueryParamsPart}")
                : requestUrl;
            return SendRequestAsync<T>(client => client.GetAsync(requestUrlWithQueryParams), bearerToken, requestHeaders);
        }

        public Task<T> PostAsync<T, T1>(string requestUrl, T1 content, string bearerToken = null, List<KeyValuePair<string, string>> requestHeaders = null) =>
            SendRequestAsync<T>(
                client => client.PostAsync(requestUrl, CreateHttpContent(content)), bearerToken, requestHeaders);

        private HttpContent CreateHttpContent<T>(T content) =>
            new StringContent(JsonSerializer.Serialize(content, ThunderJsonSerializerOptions.SharedJsonSerializerOptions), Encoding.UTF8, "application/json");

        private Task<T> SendRequestAsync<T>(Func<HttpClient, Task<HttpResponseMessage>> sendRequestFn, string token = null, List<KeyValuePair<string, string>> requestHeaders = null) =>
            Task.Run(async () =>
            {
                using (var client = _httpClientFactory.CreateClient())
                {
                    if (token != null)
                    {
                        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                    }

                    requestHeaders?.ForEach(p => client.DefaultRequestHeaders.Add(p.Key, p.Value));

                    var response = await sendRequestFn(client);
                    response.EnsureSuccessStatusCode();
                    var data = await response.Content.ReadAsStringAsync();
                    return JsonSerializer.Deserialize<T>(data, ThunderJsonSerializerOptions.SharedJsonSerializerOptions);
                }
            });
    }
}
