using System.Threading.Tasks;
using LearnerApp.Common;
using LearnerApp.PlatformServices;
using LearnerApp.Services.Identity;
using LearnerApp.Services.Navigation;
using LearnerApp.ViewModels.Base;
using Xamarin.Forms;

namespace LearnerApp.ViewModels
{
    public class LogoutViewModel : BasePageViewModel
    {
        private readonly IIdentityService _identityService;
        private readonly IOAuth2PkceSupport _pkceSupport;

        private string _sourceUrl;

        public LogoutViewModel()
        {
            _identityService = DependencyService.Resolve<IIdentityService>();
            _pkceSupport = DependencyService.Resolve<IOAuth2PkceSupport>();
        }

        public string SourceUrl
        {
            get
            {
                return _sourceUrl;
            }

            set
            {
                _sourceUrl = value;
                RaisePropertyChanged(() => SourceUrl);
            }
        }

        public override string PageTitle { get; } = "Logout";

        public override string RoutingName => NavigationRoutes.Logout;

        public void FinishLogout()
        {
            Logout();
        }

        protected override Task InternalNavigatedTo(NavigationParameters navigationParameters)
        {
            using (DialogService.DisplayLoadingIndicator())
            {
                var accountProperties = Application.Current.Properties.GetAccountProperties();

                if (accountProperties != null)
                {
                    var logoutUrl = $"{GlobalSettings.BackendServiceIdm}/connect/endsession?id_token_hint={accountProperties.IdToken}";
                    _pkceSupport.ManualClearAllCookies();
                    _identityService.RemoveAccountProperties();
                    _identityService.RemoveCloudFrontCookieInfo();

                    SourceUrl = logoutUrl;
                }
                else
                {
                    Logout();
                }
            }

            return Task.CompletedTask;
        }

        private void Logout()
        {
            Device.BeginInvokeOnMainThread(() =>
            {
                // Set IsLoginPageLoaded = false to raise login view need to reload.
                LoginState.IsLoginPageLoaded = false;

                // Initialize entire view stack for avoiding keeping old instances and data
                App.Current.MainPage = new AppShell();
            });
        }
    }
}
