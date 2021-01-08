using System;
using System.Threading.Tasks;
using Xamarin.Essentials;

namespace LearnerApp.Services.OpenUrl
{
    public class OpenUrlService : IOpenUrlService
    {
        public async Task OpenUrl(string url)
        {
            await Browser.OpenAsync(url, BrowserLaunchMode.SystemPreferred);
        }

        public async Task OpenUrl(Uri url)
        {
            await Browser.OpenAsync(url, BrowserLaunchMode.SystemPreferred);
        }
    }
}
