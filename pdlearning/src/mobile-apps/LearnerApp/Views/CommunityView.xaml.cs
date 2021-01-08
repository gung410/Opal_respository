using System.Threading.Tasks;
using LearnerApp.ViewModels;
using Xamarin.Forms;

namespace LearnerApp.Views
{
    public partial class CommunityView
    {
        private CommunityViewModel _viewModel;

        public CommunityView()
        {
            InitializeComponent();

            _viewModel = (CommunityViewModel)BindingContext;
        }

        private async void Community_OnContentLoaded(object sender, System.EventArgs e)
        {
            var injectJavascriptAsync = $"submit('{_viewModel.IdmResponse.DirectLoginUrl}','{_viewModel.IdmResponse.UserId}','{_viewModel.IdmResponse.Token}', '{_viewModel.ReturnUrl}');";
            await Community.InjectJavascriptAsync("setTimeout(function() {" + injectJavascriptAsync + "}, 2000);");
        }
    }
}