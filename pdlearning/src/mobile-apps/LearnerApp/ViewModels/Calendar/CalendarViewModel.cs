using System.Threading.Tasks;
using LearnerApp.Services.Navigation;
using LearnerApp.ViewModels.Base;

namespace LearnerApp.ViewModels.Calendar
{
    public class CalendarViewModel : BasePageViewModel
    {
        private string _sourceUrl;

        public CalendarViewModel()
        {
        }

        public string SourceUrl
        {
            get => _sourceUrl;
            set
            {
                _sourceUrl = value;
                RaisePropertyChanged(() => SourceUrl);
            }
        }

        public override string PageTitle => "Calendar";

        public override string RoutingName => NavigationRoutes.Calendar;

        protected override async Task InternalNavigatedTo(NavigationParameters navigationParameters)
        {
            var apiResult = await IdentityService.GetAccountPropertiesAsync();

            SourceUrl = $"{GlobalSettings.WebViewUrlCalendar}?accessToken={apiResult.AccessToken}&viewMode=Personal";
        }
    }
}
