using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text.Json;
using System.Threading.Tasks;
using Conexus.Opal.AccessControl.Domain.Constants;
using Conexus.Opal.AccessControl.Domain.Models;
using Conexus.Opal.Microservice.Infrastructure;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Thunder.Platform.Core.Context;
using Thunder.Platform.Core.Timing;

namespace Thunder.Service.Authentication
{
    public class ThunderAuthenticationMiddleware
    {
        /// <summary>
        /// Permissions cache time in minutes configuration. Should be moved into AppSettings when needed.
        /// </summary>
        public static int PermissionsCacheTimeInMinutes = 2;

        private const string ThunderSecretKeyName = "ThunderSecretKey";
        private readonly string _secretKey;
        private readonly RequestDelegate _next;
        private readonly IDistributedCache _distributedCache;
        private readonly IConfiguration _configuration;
        private readonly ILogger<ThunderAuthenticationMiddleware> _logger;
        private readonly HttpService _httpService;

        public ThunderAuthenticationMiddleware(
            RequestDelegate next,
            IAuthenticationSchemeProvider schemes,
            IConfiguration configuration,
            IDistributedCache distributedCache,
            ILogger<ThunderAuthenticationMiddleware> logger,
            HttpService httpService)
        {
            _configuration = configuration;
            _secretKey = configuration.GetValue<string>(ThunderSecretKeyName);

            _next = next ?? throw new ArgumentNullException(nameof(next));
            Schemes = schemes ?? throw new ArgumentNullException(nameof(schemes));
            _distributedCache = distributedCache;
            _logger = logger;
            _httpService = httpService;
        }

        public IAuthenticationSchemeProvider Schemes { get; set; }

        public async Task Invoke(HttpContext context, IUserContext userContext)
        {
            context.Features.Set<IAuthenticationFeature>(new AuthenticationFeature
            {
                OriginalPath = context.Request.Path,
                OriginalPathBase = context.Request.PathBase
            });

            // Give any IAuthenticationRequestHandler schemes a chance to handle the request
            var handlers = context.RequestServices.GetRequiredService<IAuthenticationHandlerProvider>();
            foreach (var scheme in await Schemes.GetRequestHandlerSchemesAsync())
            {
                if (await handlers.GetHandlerAsync(context, scheme.Name) is IAuthenticationRequestHandler handler
                    && await handler.HandleRequestAsync())
                {
                    return;
                }
            }

            // Support passing an access token as a form parameter.
            // This case is usually used for the POST method.
            if (context.Request.HasFormContentType)
            {
                var accessTokenQuery = context.Request.Form["AccessToken"].ToString();
                if (!string.IsNullOrEmpty(accessTokenQuery))
                {
                    context.Request.Headers.Add("Authorization", $"Bearer {accessTokenQuery}");
                }
            }

            // This case is for Azure AD authentication with JWT.
            var defaultAuthenticate = await Schemes.GetDefaultAuthenticateSchemeAsync();
            if (defaultAuthenticate != null)
            {
                var result = await context.AuthenticateAsync(defaultAuthenticate.Name);
                if (result.Succeeded)
                {
                    var principal = result.Principal;

                    // https://stackoverflow.com/questions/57998262/why-is-claimtypes-nameidentifier-not-mapping-to-sub.
                    // ISSUE with Identity Server 4
                    /*var userId = principal.FindFirstValue(ClaimTypes.NameIdentifier);
                    var userFullName = principal.FindFirstValue(ClaimTypes.GivenName);
                    var roles = principal
                        .FindAll(c => c.Type == ClaimTypes.Role)
                        .Select(c => c.Value)
                        .ToList();*/

                    var userId = principal.FindFirstValue("sub");
                    var userFullName = principal.FindFirstValue("given_name");
                    var roles = principal
                        .FindAll(c => c.Type == "role")
                        .Select(c => c.Value)
                        .ToList();
                    var permissions = await GetPermissions(context, userId);

                    context.User = principal;

                    userContext.SetValue(roles, CommonUserContextKeys.UserRoles);
                    userContext.SetValue(userId, CommonUserContextKeys.UserId);
                    userContext.SetValue(userFullName, CommonUserContextKeys.UserFullName);
                    userContext.SetValue(permissions, CommonUserContextKeys.UserPermissions);
                    userContext.SetValue(permissions.Where(p => !string.IsNullOrEmpty(p.Action)).ToDictionary(p => p.Action, p => p), CommonUserContextKeys.UserPermissionsDic);
                }
            }

            // This case is for ThunderSecretKey.
            string secretKey = context.Request.Headers[ThunderSecretKeyName];
            if (!string.IsNullOrEmpty(secretKey) && _secretKey.Equals(secretKey))
            {
                userContext.SetValue(true, CommonUserContextKeys.ValidThunderSecretKey);
            }

            // To track the origin IP.
            string originIp = context.Request.Headers["X-Forwarded-For"];
            if (!string.IsNullOrEmpty(originIp))
            {
                userContext.SetValue(originIp, CommonUserContextKeys.OriginIp);
            }

            await _next(context);
        }

        private async Task<List<ModulePermission>> GetPermissions(HttpContext context, string userId)
        {
            try
            {
                var permissionsJsonStr = await _distributedCache.GetStringAsync(CommonUserContextKeys.UserPermissions);
                var permissions = new Dictionary<string, UserPermissionsCacheItem>();
                if (!string.IsNullOrEmpty(permissionsJsonStr))
                {
                    permissions = JsonSerializer.Deserialize<Dictionary<string, UserPermissionsCacheItem>>(permissionsJsonStr);

                    // Try to load permissions by userId from cached permissions dictionnary
                    // If not exist then try to load from portal API
                    return permissions.ContainsKey(userId) && permissions[userId].IsValid()
                        ? permissions[userId].Permissions
                        : await LoadLatestPermissions(context, userId, permissions);
                }

                // Try to load from portal API
                return await LoadLatestPermissions(context, userId, permissions);
            }
            catch (Exception ex)
            {
                _logger.LogError("[PermissionException]: {Time} {ErrorMessage}", Clock.Now.ToLongTimeString(), ex.Message);

                // Try to remove the cache if exception. May be the cache structure is invalid.
                await _distributedCache.RemoveAsync(CommonUserContextKeys.UserPermissions);

                // In case competence portal return null data for permissions => return empty array permissions.
                // Already implement validate permission in PermissionRequiredAttribute.
                return new List<ModulePermission>();
            }
        }

        private async Task<List<ModulePermission>> LoadLatestPermissions(
            HttpContext context,
            string userId,
            Dictionary<string, UserPermissionsCacheItem> permissions)
        {
            // Step 1: Build request inputs
            var permissionsRequestUrl = _configuration.GetValue<string>("GetAccessRightsUrl");
            var permissionsRequestQueryParams = Enum.GetValues(typeof(PermissionModuleType))
                .Cast<PermissionModuleType>()
                .Select(v => new KeyValuePair<string, string>("modules", v.ToString()))
                .ToList();
            var cxTokenKey = "cxtoken";
            var cxToken = context.Request.Headers.ContainsKey(cxTokenKey)
                            ? context.Request.Headers[cxTokenKey].ToString()
                            : "3001:2052";
            var permissionsRequestHeaders = new List<KeyValuePair<string, string>>
            {
                new KeyValuePair<string, string>(cxTokenKey, cxToken),
                new KeyValuePair<string, string>("Authorization", context.Request.Headers["Authorization"])
            };

            // Step 2: Request to permissions server to get latest permissions
            var userPermissions = await _httpService.GetAsync<List<ModulePermission>>(
                permissionsRequestUrl,
                permissionsRequestQueryParams,
                requestHeaders: permissionsRequestHeaders);
            permissions[userId] = new UserPermissionsCacheItem(userPermissions);

            // Step 3: Update current permissions
            await _distributedCache.SetStringAsync(
                    CommonUserContextKeys.UserPermissions,
                    JsonSerializer.Serialize(permissions),
                    new DistributedCacheEntryOptions()
                    {
                        SlidingExpiration = TimeSpan.FromMinutes(UserPermissionsCacheItem.PermissionCacheTimeInMinutes)
                    });
            return userPermissions ?? new List<ModulePermission>();
        }
    }

    internal class UserPermissionsCacheItem
    {
        public const int PermissionCacheTimeInMinutes = 2;

        public UserPermissionsCacheItem(List<ModulePermission> permissions)
        {
            ExpiredTime = Clock.Now.AddMinutes(PermissionCacheTimeInMinutes);
            Permissions = permissions;
        }

        public DateTime ExpiredTime { get; set; }

        public List<ModulePermission> Permissions { get; set; }

        public bool IsValid()
        {
            return ExpiredTime > Clock.Now;
        }
    }
}
