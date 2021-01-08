using LearnerApp.ViewModels;
using Xamarin.Forms;

namespace LearnerApp.Views
{
    public partial class ChangePasswordView
    {
        private ChangePasswordViewModel _viewModel;

        public ChangePasswordView()
        {
            InitializeComponent();

            _viewModel = (ChangePasswordViewModel)BindingContext;
        }

        private async void ChangePasswordWebview_OnNavigationCompleted(object sender, string e)
        {
            var injectJavascriptAsync = $"submit('{_viewModel.IdmResponse.DirectLoginUrl}','{_viewModel.IdmResponse.UserId}','{_viewModel.IdmResponse.Token}', '{_viewModel.ReturnUrl}');";
            await ChangePasswordWebview.InjectJavascriptAsync("setTimeout(function() {" + injectJavascriptAsync + "}, 2000);");
        }
    }
}
