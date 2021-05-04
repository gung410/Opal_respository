using cxOrganization.Domain.Dtos.Auth;
using cxOrganization.Domain.HttpClients;
using cxOrganization.Domain.Settings;
using cxPlatform.Core;
using cxPlatform.Core.Cache;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace cxOrganization.Domain.Services
{
    public class PortalApiClient : IPortalApiClient
    {
        private readonly IInternalHttpClientRequestService _internalHttpClientRequestService;
        private IOptions<AppSettings> _appSettingOption;
        private readonly IMemoryCacheProvider _memoryCacheProvider;
        private const int cachingTimeSeconds = 10;
        private readonly ILogger<PortalApiClient> _logger;

        public PortalApiClient(
            IOptions<AppSettings> appSettingOption,
            IInternalHttpClientRequestService internalHttpClientRequestService,
            IMemoryCacheProvider memoryCacheProvider,
             ILogger<PortalApiClient> logger
            )
        {
            _appSettingOption = appSettingOption;
            _memoryCacheProvider = memoryCacheProvider;
            _internalHttpClientRequestService = internalHttpClientRequestService;
            _logger = logger;
        }

        public async Task<IList<string>> GetPermissionKeys(string token, string cxToken)
        {
            var cacheKey = $"User-Permissions-{token}";

            var permissionKeys = _memoryCacheProvider.Get<IList<string>>(cacheKey);
            if (permissionKeys == null)
            {
                try
                {
                    permissionKeys = await GetFreshPermissionKeys(token, cxToken);
                    _memoryCacheProvider.GetOrCreate(cacheKey, (cache) => permissionKeys);
                }
                catch (Exception exception)
                {
                    _logger.LogError(exception, $"Could not retrieve permissions for user holding the token {token}.");
                }
            }

            return permissionKeys;
        }

        private async Task<IList<string>> GetFreshPermissionKeys(string token, string cxToken)
        {
            string myAccessRightsUrl = _appSettingOption.Value.PortalAPI + "/me/accessrights";

            var accessRightDtos = await _internalHttpClientRequestService.GetAsync<List<AccessRightDto>>(token,
                                                                                   cxToken,
                                                                                   myAccessRightsUrl,
                                                                                   ("modules", new List<string>() { "OrganizationSpa", "CompetenceSpa" }));

            return accessRightDtos != null
                ? accessRightDtos.Where(p => !string.IsNullOrEmpty(p.Action)).Select(p => p.Action).ToList()
                : new List<string>();
        }

    }
}
