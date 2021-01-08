using LearnerApp.Services.Navigation;
using LearnerApp.ViewModels;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace LearnerApp.Views.Home.Newsfeed
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class CommunityPostFeedItem : ContentView
    {
        private static INavigationService _navigationService;

        public CommunityPostFeedItem()
        {
            InitializeComponent();

            _navigationService = DependencyService.Resolve<INavigationService>();
        }

        private void TapGestureRecognizer_Tapped(object sender, System.EventArgs e)
        {
            _navigationService.NavigateToAsync<SocialViewModel>();
        }
    }
}
