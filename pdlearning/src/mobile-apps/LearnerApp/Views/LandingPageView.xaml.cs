using LearnerApp.ViewModels;
using Xamarin.Forms;

namespace LearnerApp.Views
{
    public partial class LandingPageView
    {
        public LandingPageView()
        {
            InitializeComponent();
        }

        private LandingPageViewModel ViewModel => (LandingPageViewModel)BindingContext;

        protected override async void OnAppearing()
        {
            base.OnAppearing();
            await ViewModel.NavigateToProperPage();
        }
    }
}
