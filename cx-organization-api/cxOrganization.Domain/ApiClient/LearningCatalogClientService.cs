using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using cxOrganization.Domain.HttpClients;
using cxOrganization.Domain.Settings;
using cxPlatform.Core.Cache;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;

namespace cxOrganization.Domain.ApiClient
{
    public class LearningCatalogClientService : ILearningCatalogClientService
    {
        protected static JwtSecurityToken AccessToken;

        private readonly HttpClient _httpClient;
        private readonly LearningCatalogAPISettings _learningCatalogApiSettings;
        private readonly IMemoryCacheProvider _memoryCacheProvider;
        private readonly ILogger _logger;
        private readonly IIdentityServerClientService _identityServerClientService;

        public LearningCatalogClientService(ILogger<LearningCatalogClientService> logger,
            HttpClient httpClient,
            IMemoryCacheProvider memoryCacheProvider,
            IOptions<LearningCatalogAPISettings> learningCatalogApiSettingsOtions,
            IIdentityServerClientService identityServerClientService)

        {
            _learningCatalogApiSettings = learningCatalogApiSettingsOtions.Value;
            _httpClient = httpClient;
            _memoryCacheProvider = memoryCacheProvider;
            _identityServerClientService = identityServerClientService;
            _logger = logger;

            InitHttpClient();
        }

        private void InitHttpClient()
        {
            _httpClient.BaseAddress = new Uri(_learningCatalogApiSettings.APIBaseUrl);
            _httpClient.DefaultRequestHeaders.Accept.Clear();
        }


        public async Task<List<CatalogItemDto>> GetCatalogEntries(string correlationId, string parentCode)
        {
            var resource = $"catalogentries/explorer/{parentCode}";
            var cacheKey = resource;
            var catalogEntries = _memoryCacheProvider.Get<List<CatalogItemDto>>(cacheKey);
            if (catalogEntries != null)
            {
                _logger.LogDebug($"Catalogs with  parent code {parentCode} has been retrieved from memory cache.");
                return catalogEntries;
            }
            var httpRequestMessage = new HttpRequestMessage(HttpMethod.Get, resource);
            await HandleAuthenticationRequest(httpRequestMessage);
            httpRequestMessage.Headers.Add("Correlation-Id", correlationId);
         
            var responseTask = _httpClient.SendAsync(httpRequestMessage);

            var response = await responseTask.EnsureSuccessStatusCodeAsync();

            var json = await response.ReadAsStringAsync();
            catalogEntries = JsonConvert.DeserializeObject<List<CatalogItemDto>>(json);

            _memoryCacheProvider.Set(cacheKey, catalogEntries);

            return catalogEntries;
        }

        public async Task<List<CatalogItemDto>> GetOrganizationUnitTypes(string correlationId)
        {
            return await GetCatalogEntries(correlationId, _learningCatalogApiSettings.CatalogCodes.OrgUnitType);

        }

        public async Task<List<CatalogItemDto>> GetDesignations(string correlationId)
        {
            return await GetCatalogEntries(correlationId, _learningCatalogApiSettings.CatalogCodes.Designation);

        }

        protected virtual async Task HandleAuthenticationRequest(HttpRequestMessage message)
        {
            if (AccessToken == null || AccessToken.ValidTo < DateTime.UtcNow.AddMinutes(1))
            {
                AccessToken = await GetIdentityAccessToken();
            }

            message.Headers.Authorization = new AuthenticationHeaderValue("Bearer", AccessToken.RawData);

        }

        private async Task<JwtSecurityToken> GetIdentityAccessToken()
        {
            var accessTokenString = await _identityServerClientService.GetAccessToken();
            var jwthandler = new JwtSecurityTokenHandler();
            return jwthandler.ReadJwtToken(accessTokenString);

        }
    }
}