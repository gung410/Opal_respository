using System.Threading.Tasks;
using LearnerApp.Services.Navigation;
using LearnerApp.ViewModels.Base;

namespace LearnerApp.ViewModels
{
    public class TermsOfUseViewModel : BasePageViewModel, INavigationAware
    {
        private string _sourceUrl;

        public TermsOfUseViewModel()
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

        public override string PageTitle => string.Empty;

        public override string RoutingName => NavigationRoutes.TermsOfUse;

        protected override Task InternalNavigatedTo(NavigationParameters navigationParameters)
        {
            SourceUrl = navigationParameters.GetParameter<string>("Url");
            return Task.CompletedTask;
        }
    }
}
