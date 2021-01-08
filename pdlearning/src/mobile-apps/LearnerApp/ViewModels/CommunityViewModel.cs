using System.Threading.Tasks;
using FFImageLoading;
using LearnerApp.Models;
using LearnerApp.Services.Navigation;
using LearnerApp.ViewModels.Base;
using Xamarin.Forms;

namespace LearnerApp.ViewModels
{
    public class CommunityViewModel : BasePageViewModel, INavigationAware
    {
        public IdmResponse IdmResponse;

        private string _returnUrl;

        private string _sourceUrl;

        public CommunityViewModel()
        {
        }

        public string SourceUrl
        {
            get => _sourceUrl;
            set
            {
                _sourceUrl = value;
                if (!string.IsNullOrEmpty(value))
                {
                    RaisePropertyChanged(() => SourceUrl);
                }
            }
        }

        public string ReturnUrl
        {
            get => _returnUrl;
            set
            {
                _returnUrl = value;
                if (!string.IsNullOrEmpty(value))
                {
                    RaisePropertyChanged(() => ReturnUrl);
                }
            }
        }

        public override string PageTitle => string.Empty;

        public override string RoutingName => NavigationRoutes.Community;

        protected override Task InternalNavigatedTo(NavigationParameters navigationParameters)
        {
            var idmResponse = navigationParameters.GetParameter<IdmResponse>("IdmResponse");

            ReturnUrl = navigationParameters.GetParameter<string>("ReturnUrl");

            IdmResponse = idmResponse;

            SourceUrl = $"{GlobalSettings.BackendServiceIdm}/DirectLogin.html";

            return Task.CompletedTask;
        }
    }
}