using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Microsoft.AspNetCore.WebUtilities;
using Communication.Business.Exceptions;

namespace Communication.Business.HttpClients
{
    public class IdmClientService : IIdentityServerClientService
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _configuration;
        private string _accessToken;
        public IdmClientService(IConfiguration configuration, HttpClient httpClient)
        {
            _configuration = configuration;
            _httpClient = httpClient;
            InitialRequest();
        }

        private void InitialRequest()
        {
            var baseUrl = _configuration["IDM_BASEURL"];
            _httpClient.BaseAddress = new Uri(baseUrl);
            _httpClient.DefaultRequestHeaders.Accept.Clear();
        }

        public async Task<string> GetAccessToken()
        {
            var grantType = _configuration["IDM_GRANT_TYPE"];
            var clientId = _configuration["IDM_CLIENT_ID"];
            var clientSecret = _configuration["IDM_CLIENT_SECRET"];
            var scope = _configuration["IDM_SCOPE"];

            var formVariables = new List<KeyValuePair<string, string>>();
            formVariables.Add(new KeyValuePair<string, string>("grant_type", grantType));
            formVariables.Add(new KeyValuePair<string, string>("client_id", clientId));
            formVariables.Add(new KeyValuePair<string, string>("client_secret", clientSecret));
            formVariables.Add(new KeyValuePair<string, string>("scope", scope));
            var formContent = new FormUrlEncodedContent(formVariables);
            formContent.Headers.ContentType.CharSet = "UTF-8";
            var response = _httpClient.PostAsync("connect/token", formContent);
            var content = await response.EnsureSuccessStatusCodeAsync();
            var jsonString = await content.ReadAsStringAsync();
            var tokenData = JObject.Parse(jsonString);
            return await Task.FromResult(tokenData.GetValue("access_token").Value<string>());

        }

        public async Task<UserIdmResponseInfo> GetUsers(string orgUnit = "", string role = "",int pageIndex = 1, int pageSize = 100)
        {
            if (string.IsNullOrEmpty(_accessToken))
            {
                _accessToken = await GetAccessToken();
                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _accessToken);
            }
            else
            {
                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _accessToken);
            }
            string queryString = "";
            if (!string.IsNullOrEmpty(orgUnit))
                queryString = QueryHelpers.AddQueryString(queryString, "orgUnitIds", orgUnit);
            if (!string.IsNullOrEmpty(role))
                queryString = QueryHelpers.AddQueryString(queryString, "roles", role);
            queryString = QueryHelpers.AddQueryString(queryString, "pageSize", pageSize.ToString());
            queryString = QueryHelpers.AddQueryString(queryString, "pageIndex", pageIndex.ToString());
            var response = _httpClient.GetAsync($"/users{queryString}");
            var content = await response.EnsureSuccessStatusCodeAsync();
            var json = await content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<UserIdmResponseInfo>(json);

        }
    }
}
