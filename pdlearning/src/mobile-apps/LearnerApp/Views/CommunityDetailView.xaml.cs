using LearnerApp.ViewModels;
using Xamarin.Forms;

namespace LearnerApp.Views
{
    public partial class CommunityDetailView
    {
        private readonly CommunityDetailViewModel _viewModel;

        public CommunityDetailView()
        {
            InitializeComponent();

            _viewModel = (CommunityDetailViewModel)BindingContext;
        }

        private async void CommunityDetail_OnOnNavigationCompleted(object sender, string e)
        {
            var injectJavascriptAsync = $"submit('{_viewModel.IdmResponse.DirectLoginUrl}','{_viewModel.IdmResponse.UserId}','{_viewModel.IdmResponse.Token}', '{_viewModel.ReturnUrl}');";
            await CommunityDetail.InjectJavascriptAsync("setTimeout(function() {" + injectJavascriptAsync + "}, 2000);");
        }
    }
}
