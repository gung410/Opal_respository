using System.Threading.Tasks;
using LearnerApp.Common;
using LearnerApp.Services.Identity;
using LearnerApp.Services.Navigation;
using LearnerApp.ViewModels.Base;
using Plugin.HybridWebView.Shared.Enumerations;
using Xamarin.Forms;

namespace LearnerApp.ViewModels
{
    public class PDPlannerViewModel : BasePageViewModel
    {
        private string _sourceUrl;
        private WebViewContentType _webviewContentType;

        public PDPlannerViewModel()
        {
            CachingMode = PageCachingMode.None;

            SourceUrl = GlobalSettings.WebViewUrlPdPlanner;
        }

        public string SourceUrl
        {
            get => _sourceUrl;
            set
            {
                _sourceUrl = value;
                RaisePropertyChanged(() => SourceUrl);
            }
        }

        public WebViewContentType ContentType
        {
            get => _webviewContentType;
            set
            {
                _webviewContentType = value;
                RaisePropertyChanged(() => ContentType);
            }
        }

        public override string PageTitle { get; } = "PD Planner";

        public override string RoutingName => NavigationRoutes.PdPlanner;

        public async Task<string> GetAccessToken()
        {
            var properties = await IdentityService.GetAccountPropertiesAsync();

            return properties != null ? properties.AccessToken : string.Empty;
        }

        protected override Task InternalNavigatedTo(NavigationParameters navigationParameters)
        {
            if (navigationParameters == null)
            {
                // This is trick for reload source url to call event navigate in webview
                // We need reload data when user click PDPlanner
                ContentType = WebViewContentType.StringData;
                SourceUrl = "PLEASE WAIT";
                ContentType = WebViewContentType.Internet;
                SourceUrl = GlobalSettings.WebViewUrlPdPlanner;
            }

            return base.InternalNavigatedTo(navigationParameters);
        }
    }
}
