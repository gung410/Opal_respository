using System.Threading.Tasks;
using LearnerApp.Services.Navigation;
using LearnerApp.ViewModels.Base;
using Xamarin.Forms;

namespace LearnerApp.ViewModels
{
    public class LandingPageViewModel : BasePageViewModel
    {
        private INavigationService _navigationService;

        public LandingPageViewModel()
        {
            _navigationService = DependencyService.Resolve<INavigationService>();
        }

        public override string PageTitle => string.Empty;

        public override string RoutingName => NavigationRoutes.LandingPage;

        public async Task NavigateToProperPage()
        {
            using (DialogService.DisplayLoadingIndicator())
            {
                await _navigationService.InitializeAsync();
            }
        }
    }
}
