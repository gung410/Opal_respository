using System;
using System.Threading.Tasks;
using System.Windows.Input;
using LearnerApp.Common.TaskController;
using LearnerApp.Services.Navigation;
using LearnerApp.ViewModels.Base;
using Xamarin.Forms;

namespace LearnerApp.ViewModels.Achievement
{
    public class AchievementOverviewViewModel : BasePageViewModel
    {
        private bool _isRefreshing;

        public AchievementOverviewViewModel()
        {
            BadgeOverviewViewModel = new AchievementBadgeOverviewViewModel();
            ECertificateViewModel = new AchievementECertificateOverviewViewModel();
        }

        public AchievementBadgeOverviewViewModel BadgeOverviewViewModel { get; }

        public AchievementECertificateOverviewViewModel ECertificateViewModel { get; }

        public override string PageTitle => string.Empty;

        public override string RoutingName => NavigationRoutes.Achievement;

        public bool IsRefreshing
        {
            get
            {
                return _isRefreshing;
            }

            set
            {
                _isRefreshing = value;
                IsRefreshingForRefreshView = value;
                RaisePropertyChanged(() => IsRefreshing);
            }
        }

        public bool IsRefreshingForRefreshView
        {
            get
            {
                return false;
            }

            set
            {
                if (_isRefreshing == false)
                {
                    RaisePropertyChanged(() => IsRefreshingForRefreshView);
                }
            }
        }

        public ICommand RefreshCommand => new Command(async () => await InitScreen());

        public async Task InitScreen()
        {
            IsRefreshing = true;
            var parallelRunner = new ParallelTaskRunner(3);

            var listTasks = new Func<Task>[]
            {
                ECertificateViewModel.LoadData,
                BadgeOverviewViewModel.LoadData
            };

            try
            {
                await parallelRunner.RunAsync(listTasks);
            }
            finally
            {
                IsRefreshing = false;
            }
        }

        protected override async Task InternalNavigatedTo(NavigationParameters navigationParameters)
        {
            await base.InternalNavigatedTo(navigationParameters);
            await InitScreen();
        }
    }
}
