using System.Net;
using System.Threading.Tasks;
using Foundation;
using LearnerApp.iOS.Services;
using LearnerApp.PlatformServices;
using Xamarin.Forms;

[assembly: Dependency(typeof(CloudFrontCookieSetup))]

namespace LearnerApp.iOS.Services
{
    public class CloudFrontCookieSetup : ICloudFrontCookieSetup
    {
        public Task SetupCloudFrontCookie(CookieCollection cookies)
        {
            var cookieJar = NSHttpCookieStorage.SharedStorage;
            cookieJar.AcceptPolicy = NSHttpCookieAcceptPolicy.Always;
            for (int i = 0; i < cookies.Count; i++)
            {
                cookieJar.SetCookie(new NSHttpCookie(cookies[i].Name, cookies[i].Value, "/", GlobalSettings.CloudFrontUrl));
            }

            return Task.CompletedTask;
        }
    }
}
