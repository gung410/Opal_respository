using System;
using System.Threading.Tasks;
using LearnerApp.Common.Helper;
using LearnerApp.ViewModels;
using Xamarin.Forms;

namespace LearnerApp.Views
{
    public partial class LogoutView
    {
        private StressActionHandler _logoutStressHandler = new StressActionHandler();
        private LogoutViewModel _viewModel;

        public LogoutView()
        {
            InitializeComponent();

            _viewModel = (LogoutViewModel)BindingContext;
        }

        protected override bool OnBackButtonPressed() => true;

        private async Task OnFinishLogout(string data)
        {
            await _logoutStressHandler.RunAsync(async () =>
            {
                await WebView.InjectJavascriptAsync("window.IDM.onFinishLogout = null;");
                _viewModel.FinishLogout();
            });
        }

        private async void WebView_OnOnContentLoaded(object sender, EventArgs e)
        {
            WebView.AddLocalCallback("OnFinishLogout", async (data) => await OnFinishLogout(data));
            await WebView.InjectJavascriptAsync("window.IDM.onFinishLogout = OnFinishLogout;");
        }

        private void WebView_OnNavigationStarted(object sender, Plugin.HybridWebView.Shared.Delegates.DecisionHandlerDelegate e)
        {
            if (e == null || string.IsNullOrEmpty(e.Uri))
            {
                return;
            }

            bool isUrl = Uri.TryCreate(e.Uri, UriKind.Absolute, out var uriResult) && (uriResult != null && (uriResult.Scheme == Uri.UriSchemeHttp || uriResult.Scheme == Uri.UriSchemeHttps));

            if (!isUrl)
            {
                return;
            }

            // Android device not raise event logout OnFinishLogout because sometime redirect so fast.
            // We must compare with url to ensure logout complete and block redirect page.
            if (e.Uri == GlobalSettings.AuthRedirectLogoutUrl)
            {
                e.Cancel = true;

                return;
            }
        }
    }
}
