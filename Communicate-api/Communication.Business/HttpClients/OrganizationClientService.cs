using Communication.Business.Exceptions;
using Communication.Business.Models;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace Communication.Business.HttpClients
{
    public class OrganizationClientService : IOrganizationClientService
    {
        private string _accessToken;
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _configuration;
        private readonly IIdentityServerClientService _identityServerClientService;

        public OrganizationClientService(IIdentityServerClientService identityServerClientService, IConfiguration configuration, HttpClient httpClient)
        {
            _identityServerClientService = identityServerClientService;
            _configuration = configuration;
            _httpClient = httpClient;
            InitialRequest();
        }

        private void InitialRequest()
        {
            var baseUrl = _configuration["ORG_DOMAIN_API"];
            _httpClient.BaseAddress = new Uri(baseUrl);
            _httpClient.DefaultRequestHeaders.Accept.Clear();
        }

        public async Task<OrgnanizationResponseDto> GetUsers(string cxToken,
            ISet<string> userIds = null, ISet<string> emails = null,
            ISet<string> departmentIds = null,
            ISet<string> usertypes = null,
            ISet<string> userGroupIds = null,
            int pageIndex = 1, int pageSize = 100,
            bool? forHrmsUsers = null, bool? forExternalUsers = null)
        {
            _httpClient.DefaultRequestHeaders.Accept.Clear();
            await HandleToken(_httpClient);
            string queryString = "";
            if (departmentIds != null)
            {
                foreach (var item in departmentIds)
                {
                    queryString = QueryHelpers.AddQueryString(queryString, "parentDepartmentId", item.Trim());
                }
            }
            if (userIds != null)
            {
                foreach (var item in userIds)
                {
                    queryString = QueryHelpers.AddQueryString(queryString, "loginServiceClaims", item.Trim());
                }
            }
            if (userGroupIds != null)
            {
                foreach (var item in userGroupIds)
                {
                    queryString = QueryHelpers.AddQueryString(queryString, "userGroupIds", item.Trim());
                }
            }
            if (emails != null)
            {
                foreach (var item in emails)
                {
                    queryString = QueryHelpers.AddQueryString(queryString, "emails", item.Trim());
                }
            }
            if (usertypes != null)
            {
                foreach (var item in usertypes)
                {
                    queryString = QueryHelpers.AddQueryString(queryString, "usertypeIds", item.TrimEnd().TrimStart());
                }
            }
            if (userIds != null)
            {
                foreach (var item in userIds)
                {
                    queryString = QueryHelpers.AddQueryString(queryString, "loginServiceClaims", item.Trim());
                }
            }
            if (forHrmsUsers.HasValue && forHrmsUsers.Value)
            {
                queryString = QueryHelpers.AddQueryString(queryString, "externallyMastered", "true");
            }
            if (forExternalUsers.HasValue && forExternalUsers.Value)
            {
                queryString = QueryHelpers.AddQueryString(queryString, "externallyMastered", "false");
            }
            queryString = QueryHelpers.AddQueryString(queryString, "pageSize", pageSize.ToString());
            queryString = QueryHelpers.AddQueryString(queryString, "pageIndex", pageIndex.ToString());
            _httpClient.DefaultRequestHeaders.TryAddWithoutValidation("cxToken", cxToken);

            var response = _httpClient.GetAsync($"usermanagement/users{queryString}");
            var content = await response.EnsureSuccessStatusCodeAsync();
            var json = await content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<OrgnanizationResponseDto>(json);
        }

        private async Task HandleToken(HttpClient client)
        {
            if (string.IsNullOrEmpty(_accessToken))
            {
                client.DefaultRequestHeaders.Clear();
                _accessToken = await _identityServerClientService.GetAccessToken();
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _accessToken);
            }
            else
            {
                _httpClient.DefaultRequestHeaders.Clear();
                var jwthandler = new JwtSecurityTokenHandler();
                var jwttoken = jwthandler.ReadJwtToken(_accessToken);
                var expDate = jwttoken.ValidTo;
                if (expDate < DateTime.UtcNow.AddMinutes(1))
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