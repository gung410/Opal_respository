using System.Threading.Tasks;
using System.Windows.Input;
using LearnerApp.Helper;
using LearnerApp.Services.Identity;
using LearnerApp.ViewModels.Achievement;
using LearnerApp.ViewModels.Base;
using LearnerApp.ViewModels.Calendar;
using LearnerApp.ViewModels.Report;
using Xamarin.Forms;

namespace LearnerApp.ViewModels
{
    public class MoreViewModel : BaseViewModel
    {
        public readonly IIdentityService _identityService;

        private bool _isVisibleEPortfolio;
        private bool _isVisibleCalendar;
        private bool _isVisibleReport;

        public MoreViewModel()
        {
            _identityService = DependencyService.Resolve<IIdentityService>();
            IsVisibleEPortfolio = PermissionHelper.GetPermissionForEPortfolioPage();
            IsVisibleCalendar = PermissionHelper.GetPermissionForCalendarPage();
            IsVisibleReport = PermissionHelper.GetPermissionForReportPage();
        }

        public bool IsVisibleEPortfolio
        {
            get
            {
                return _isVisibleEPortfolio;
            }

            set
            {
                _isVisibleEPortfolio = value;
                RaisePropertyChanged(() => IsVisibleEPortfolio);
            }
        }

        public bool IsVisibleCalendar
        {
            get
            {
                return _isVisibleCalendar;
            }

            set
            {
                _isVisibleCalendar = value;
                RaisePropertyChanged(() => IsVisibleCalendar);
            }
        }

        public bool IsVisibleReport
        {
            get
            {
                return _isVisibleReport;
            }

            set
            {
                _isVisibleReport = value;
                RaisePropertyChanged(() => IsVisibleReport);
            }
        }

        public ICommand OnClickEportfolioCommand => new Command(async () => await OnNavigatedToEportfolio());

        public ICommand OnClickSocialCommand => new Command(async () => await OnNavigatedToSocial());

        public ICommand OnClickCalendarCommand => new Command(async () => await OnNavigatedToCalendar());

        public ICommand OnClickAchievementCommand => new Command(async () => await OnNavigateToAchievement());

        public ICommand OnClickReportCommand => new Command(async () => await OnNavigateToReport());

        private async Task OnNavigateToReport()
        {
            await this.NavigationService.NavigateToAsync<ReportPageViewModel>();
        }

        private async Task OnNavigateToAchievement()
        {
            await this.NavigationService.NavigateToAsync<AchievementOverviewViewModel>();
        }

        private async Task OnNavigatedToCalendar()
        {
            await this.NavigationService.NavigateToAsync<CalendarViewModel>();
        }

        private async Task OnNavigatedToEportfolio()
        {
            await this.NavigationService.NavigateToAsync<EportfolioViewModel>();
        }

        private async Task OnNavigatedToSocial()
        {
            await this.NavigationService.NavigateToAsync<SocialViewModel>();
        }
    }
}
