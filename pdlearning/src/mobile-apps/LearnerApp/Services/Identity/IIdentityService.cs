using System;
using System.Threading.Tasks;
using LearnerApp.Models;
using LearnerApp.PlatformServices;

namespace LearnerApp.Services.Identity
{
    public interface IIdentityService
    {
        DateTime AuthenticatedCheckTime { get; }

        Task<IdentityModel> Authenticate(IOAuth2PkceSupport pkceSupport);

        Task<IdentityModel> GetAccountPropertiesAsync();

        Task<bool> IsAuthenticated();

        Task SetAccountPropertiesAsync(string props);

        void RemoveAccountProperties();

        /// <summary>
        /// Store cloud front cookie to secure storage for later use.
        /// This method will also set cookie to CookieManager in Android and CookieJar in iOS by calling DependencyService.
        /// </summary>
        /// <param name="cookieInfo">The cookie information from cloud front Api.</param>
        /// <returns>A completed task.</returns>
        Task StoreCloudFrontCookieInfo(CloudFrontCookieInfo cookieInfo);

        /// <summary>
        /// Remove cloud front cookie to secure storage for kill app or uninstall.
        /// </summary>
        void RemoveCloudFrontCookieInfo();

        /// <summary>
        /// This method will setup cookie for FFImageLoader to access CloudFront resource with signed cookie.
        /// </summary>
        /// <returns>A completed task.</returns>
        Task SetupCloudFrontCookieForImageService();
    }
}
