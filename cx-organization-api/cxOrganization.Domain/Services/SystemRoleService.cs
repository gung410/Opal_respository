using cxOrganization.Client.UserTypes;
using cxOrganization.Domain.Dtos.UserTypes;
using cxOrganization.Domain.HttpClients;
using cxOrganization.Domain.Settings;
using cxPlatform.Core;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace cxOrganization.Domain.Services
{
    public class SystemRoleService : ISystemRoleService
    {
        private readonly HttpClient _httpClient;
        private readonly IWorkContext _workContext;
        private readonly AppSettings _appSettings;

        public SystemRoleService(
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
            var baseUrl = _appSettings.ApiBaseUrl;
            _httpClient.BaseAddress = new Uri(baseUrl);
            _httpClient.DefaultRequestHeaders.Accept.Clear();
        }

        public async Task<List<SystemRoleSubjects>> GetSystemRolesInfoAsync(GetSystemRolesInfoRequest getSystemRolesInfoRequest, string token)
        {
            try
            {
                HandleToken(_httpClient, token);

                var response = _httpClient.GetAsync($"{_appSettings.PortalAPI}/SystemRoles/?" +
                    $"includeLocalizedData={getSystemRolesInfoRequest.includeLocalizedData}" +
                    $"&includeSystemRolePermissionSubjects={getSystemRolesInfoRequest.includeSystemRolePermissionSubjects}");

                var content = await response.EnsureSuccessStatusCodeAsync();

                var systemRolesJson = await content.ReadAsStringAsync();

                var systemRoles = JsonConvert.DeserializeObject<List<SystemRoleSubjects>>(systemRolesJson);

                return systemRoles;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<List<UserTypeDto>> GetSystemRolesConvertedToUserTypesModel(GetSystemRolesInfoRequest getSystemRolesInfoRequest, string token)
        {
            try
            {
                var systemRoles = await GetSystemRolesInfoAsync(getSystemRolesInfoRequest, token);

                return systemRoles.Select(role => role.convertToUserType(this._workContext.CurrentOwnerId, this._workContext.CurrentCustomerId)).ToList();
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
    }
}
