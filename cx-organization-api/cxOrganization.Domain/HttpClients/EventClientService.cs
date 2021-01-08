
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace cxOrganization.Domain.HttpClients
{
    public class EventClientService : IEventClientService
    {
        private static string _accessToken;
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _configuration;
        private readonly IIdentityServerClientService _identityServerClientService;

        public EventClientService(HttpClient httpClient, IConfiguration configuration, 
            IIdentityServerClientService identityServerClientService)
        {
            _configuration = configuration;
            _identityServerClientService = identityServerClientService;
            _httpClient = httpClient;

            InitialRequest();
        }

        private void InitialRequest()
        {
            var baseUrl = _configuration["EVENT_API_BASE_URL"];
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12 | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls;
            _httpClient.BaseAddress = new Uri(baseUrl);
            _httpClient.DefaultRequestHeaders.Accept.Clear();
        }


        public async Task SendEvent(dynamic eventData)
        {
            await HandleToken(_httpClient);
            var httpContent = new StringContent(JsonConvert.SerializeObject(eventData), Encoding.UTF8, "application/json");
            var response = _httpClient.PostAsync("/userevents", httpContent);
            var content = await response.EnsureSuccessStatusCodeAsync();
        }
        private async Task HandleToken(HttpClient client)
        {
            if (string.IsNullOrEmpty(_accessToken))
            {
                _accessToken = await _identityServerClientService.GetAccessToken();
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _accessToken);
            }
            else
            {
                var jwthandler = new JwtSecurityTokenHandler();
                var jwttoken = jwthandler.ReadJwtToken(_accessToken);
                var expDate = jwttoken.ValidTo;
                if (expDate < DateTime.UtcNow.AddMinutes(1) || string.IsNullOrEmpty(_accessToken))
                {
                    _accessToken = await _identityServerClientService.GetAccessToken();
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _accessToken);
                }
                else
                {
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _accessToken);
                }
            }
        }
    }
}
