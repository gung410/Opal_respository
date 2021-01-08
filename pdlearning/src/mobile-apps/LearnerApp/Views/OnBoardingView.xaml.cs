using System.Threading.Tasks;
using LearnerApp.ViewModels;

namespace LearnerApp.Views
{
    public partial class OnBoardingView
    {
        private OnBoardingViewModel _viewModel;

        public OnBoardingView()
        {
            InitializeComponent();

            _viewModel = (OnBoardingViewModel)BindingContext;
        }

        private async void OnBoarding_OnContentLoaded(object sender, System.EventArgs e)
        {
            var injectJavascriptAsync = $"submit('{_viewModel.IdmResponse.DirectLoginUrl}','{_viewModel.IdmResponse.UserId}','{_viewModel.IdmResponse.Token}', '{_viewModel.ReturnUrl}');";
            await OnBoarding.InjectJavascriptAsync("setTimeout(function() {" + injectJavascriptAsync + "}, 2000);");

            OnBoarding.AddLocalCallback("OnUserOnboardingFinish", async (data) => await OnUserOnboardingFinish(data));
            await OnBoarding.InjectJavascriptAsync("window.SAM.onUserOnboardingFinish = OnUserOnboardingFinish;");
        }

        private async Task OnUserOnboardingFinish(string obj)
        {
            var viewModel = (OnBoardingViewModel)BindingContext;
            await viewModel.GetUserManagement();
        }
    }
}
