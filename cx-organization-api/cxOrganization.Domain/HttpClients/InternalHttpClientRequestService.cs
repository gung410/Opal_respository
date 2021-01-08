using cxOrganization.Domain.Extensions;
using cxOrganization.Domain.Settings;
using cxPlatform.Core;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace cxOrganization.Domain.HttpClients
{
    public class InternalHttpClientRequestService : IInternalHttpClientRequestService
    {
        private readonly HttpClient _httpClient;
        private readonly IWorkContext _workContext;
        private readonly AppSettings _appSettings;

        public InternalHttpClientRequestService(
            HttpClient httpClient,
            IWorkContext workContext,
            IOptions<AppSettings> appSettingsOptions)
        {
            _httpClient = httpClient;
            _workContext = workContext;
            _appSettings = appSettingsOptions.Value;
            InitialRequest();
        }

        private void InitialRequest()
        {
            var apiBaseUrl = _appSettings.ApiBaseUrl;
            _httpClient.BaseAddress = new Uri(apiBaseUrl);
            _httpClient.DefaultRequestHeaders.Accept.Clear();
        }

        public async Task<T> GetAsync<T>(string token, string baseUrl, params (string, List<string>)[] payloads)
        {
            try
            {
                HandleToken(_httpClient, token);

                var urlQuery = getUrlQuery(baseUrl, payloads);

                var response = _httpClient.GetAsync(urlQuery);

                var content = await response.EnsureSuccessStatusCodeAsync();

                var json = await content.ReadAsStringAsync();

                var data = JsonConvert.DeserializeObject<T>(json);

                return data;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private void HandleToken(HttpClient client, string token)
        {
            try
            {
                var _accessToken = token.Replace("Bearer ", "");
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _accessToken);
                client.DefaultRequestHeaders.Add("cxToken", $"{_workContext.CurrentOwnerId}:{_workContext.CurrentCustomerId}");
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private string getUrlQuery(string url, params (string, List<string>)[] payloads)
        {
            var urlResult = $"{url}?";

            foreach (var payload in payloads)
            {
                urlResult += extractPayload(payload.Item1, payload.Item2);
            }

            return urlResult.RemoveTheLastChar();
        }

        private string extractPayload(string key, List<string> payloadItems)
        {
            var extractedPayload = "";
            foreach (var item in payloadItems)
            {
                extractedPayload += $"{key}={item}&";
            }

            return extractedPayload;
        }
    }
}
