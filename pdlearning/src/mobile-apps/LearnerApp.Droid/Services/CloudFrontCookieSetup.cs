using System.Net;
using System.Threading.Tasks;
using Android.Webkit;
using LearnerApp.Droid.Services;
using LearnerApp.PlatformServices;
using Xamarin.Forms;

[assembly: Dependency(typeof(CloudFrontCookieSetup))]

namespace LearnerApp.Droid.Services
{
    public class CloudFrontCookieSetup : ICloudFrontCookieSetup
    {
        public Task SetupCloudFrontCookie(CookieCollection cookies)
        {
            var cookieManager = CookieManager.Instance;
            for (int i = 0; i < cookies.Count; i++)
            {
                cookieManager.SetCookie(GlobalSettings.CloudFrontUrl, cookies[i].Name + "=" + cookies[i].Value);
            }

            return Task.CompletedTask;
        }
    }
}
