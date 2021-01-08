using System.Threading.Tasks;
using LearnerApp.Common;
using LearnerApp.ViewModels;
using Xamarin.Forms;

namespace LearnerApp.Views
{
    public partial class LoginView
    {
        public LoginView()
        {
            InitializeComponent();
        }

        protected override bool OnBackButtonPressed() => true;

        protected override async void OnAppearing()
        {
            base.OnAppearing();

            var viewModel = (LoginViewModel)BindingContext;

            // We need delay 2 seconds to render all control in view.
            await Task.Delay(2000);

            // If the user is already logged in, do nothing.
            if (!LoginState.IsLoginPageLoaded)
            {
                LoginState.IsLoginPageLoaded = true;

                viewModel.LoginCommand.Execute(null);
            }
        }
    }
}
