using LearnerApp.Plugins.ScreenRecorder;
using LearnerApp.ViewModels;
using Xamarin.Forms;

namespace LearnerApp.Views
{
    public partial class SocialView
    {
        private readonly SocialViewModel _viewModel;
        private readonly ScreenRecorderManager _recordScreenManager;

        public SocialView()
        {
            InitializeComponent();

            _viewModel = BindingContext as SocialViewModel;
            _recordScreenManager = new ScreenRecorderManager();
        }

        protected override bool OnBackButtonPressed() => true;

        private async void SocialWebview_Navigated(object sender, string url)
        {
            if (url.Contains("DirectLogin.html"))
            {
                var injectJavascriptAsync = $"submit('{_viewModel.IdmResponse.DirectLoginUrl}','{_viewModel.IdmResponse.UserId}','{_viewModel.IdmResponse.Token}', '{_viewModel.ReturnUrl}');";
                await SocialWebview.InjectJavascriptAsync("setTimeout(function() {" + injectJavascriptAsync + "}, 3000);");
                return;
            }
            else if (url.Contains(GlobalSettings.WebViewUrlSocial))
            {
                // Add Record callback
                await SocialWebview.InjectJavascriptAsync("window.mobileStartRecording = function () { internalMobileStartRecording() }");

                SocialWebview.AddLocalCallback("internalMobileStartRecording", async s =>
                {
                    await _recordScreenManager.StartRecording();
                });
            }
        }
    }
}
