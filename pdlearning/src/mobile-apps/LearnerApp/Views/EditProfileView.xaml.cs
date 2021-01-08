using System.Threading.Tasks;
using LearnerApp.ViewModels;
using Xamarin.Forms;

namespace LearnerApp.Views
{
    public partial class EditProfileView
    {
        private EditProfileViewModel _viewModel;

        public EditProfileView()
        {
            InitializeComponent();

            _viewModel = (EditProfileViewModel)BindingContext;
        }

        private async void EditProfile_OnContentLoaded(object sender, System.EventArgs e)
        {
            var injectJavascriptAsync = $"submit('{_viewModel.IdmResponse.DirectLoginUrl}','{_viewModel.IdmResponse.UserId}','{_viewModel.IdmResponse.Token}', '{_viewModel.ReturnUrl}');";
            await EditProfile.InjectJavascriptAsync("setTimeout(function() {" + injectJavascriptAsync + "}, 2000);");

            EditProfile.AddLocalCallback("OnUserUpdatedProfile", async (data) => await OnUserUpdatedProfile(data));
            await EditProfile.InjectJavascriptAsync($"window.SAM.onUserUpdatedProfile = OnUserUpdatedProfile;");

            EditProfile.AddLocalCallback("OnUserClickCancel", async (data) => await OnUserClickCancel(data));
            await EditProfile.InjectJavascriptAsync($"window.SAM.onUserClickCancel = OnUserClickCancel;");
        }

        private async Task OnUserClickCancel(string data)
        {
            var viewModel = (EditProfileViewModel)BindingContext;
            await viewModel.RedirectNavigationPage(true);
        }

        private async Task OnUserUpdatedProfile(string data)
        {
            var viewModel = (EditProfileViewModel)BindingContext;
            await viewModel.RedirectNavigationPage(false);
        }
    }
}
