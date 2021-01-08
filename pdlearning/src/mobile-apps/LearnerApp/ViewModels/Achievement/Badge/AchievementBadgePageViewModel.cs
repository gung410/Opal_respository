using System;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using LearnerApp.Common;
using LearnerApp.Common.Helper;
using LearnerApp.Controls.LearnerObservableCollection;
using LearnerApp.Models.Achievement;
using LearnerApp.Services.Navigation;
using LearnerApp.Services.ServiceManager;
using LearnerApp.ViewModels.Achievement.ECertificate;
using LearnerApp.ViewModels.Base;
using Xamarin.Forms;

namespace LearnerApp.ViewModels.Achievement.Badge
{
    public class AchievementBadgePageViewModel : BasePageViewModel
    {
        public const int MaxResultPerPage = 10;

        private readonly StressActionHandler _loadMoreStressActionHandler = new StressActionHandler();
        private readonly AchievementBackendServiceManager _achievementBackendServiceManager;

        private int _page = 1;
        private int _totalCount;
        private bool _isLoading = true;
        private AchievementBadgeInfoDto[] _badgeInfoData;

        public AchievementBadgePageViewModel()
        {
            _achievementBackendServiceManager = new AchievementBackendServiceManager();
        }

        public LearnerObservableCollection<AchievementBadgeItemViewModel> BadgeSource { get; private set; }
            = new LearnerObservableCollection<AchievementBadgeItemViewModel>();

        public int Page
        {
            get => _page;
            set
            {
                _page = value;
                RaisePropertyChanged(() => Page);
            }
        }

        public int TotalPageNumber
        {
            get
            {
                return (int)Math.Ceiling((double)TotalCount / GlobalSettings.MaxResultPerPage);
            }
        }

        public int TotalCount
        {
            get => _totalCount;
            set
            {
                _totalCount = value;
                RaisePropertyChanged(() => TotalCount);
                RaisePropertyChanged(() => TotalPageNumber);
            }
        }

        public bool IsLoading
        {
            get => _isLoading;
            set
            {
                _isLoading = value;
                RaisePropertyChanged(() => IsLoading);
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
                if (_isLoading == false)
                {
                    RaisePropertyChanged(() => IsRefreshingForRefreshView);
                }
            }
        }

        public override string PageTitle => string.Empty;

        public override string RoutingName => NavigationRoutes.AchievementBadge;

        public ICommand LoadPageCommand => new Command(async () =>
        {
            await LoadData();
        });

        public ICommand RefreshCommand => new Command(async () =>
        {
            Page = 1;
            await _loadMoreStressActionHandler.RunAsync(
                async () => await LoadData());
        });

        public async Task LoadData()
        {
            IsLoading = true;
            if (_badgeInfoData.IsNullOrEmpty())
            {
                _badgeInfoData = await _achievementBackendServiceManager.GetAchievementBadgesInfo();
                if (_badgeInfoData == null)
                {
                    IsLoading = false;
                    return;
                }
            }

            var badges = await _achievementBackendServiceManager.GetAchievementBadges(
                _badgeInfoData, BadgeSource.Count);

            if (badges == null)
            {
                IsLoading = false;
                return;
            }

            var badgesVm =
                badges.Items.Select(x => new AchievementBadgeItemViewModel(x)).ToList();

            TotalCount = badges.TotalCount;
            BadgeSource = new LearnerObservableCollection<AchievementBadgeItemViewModel>(badgesVm);
            RaisePropertyChanged(() => BadgeSource);
            IsLoading = false;
        }

        protected override async Task InternalNavigatedTo(NavigationParameters navigationParameters)
        {
            await base.InternalNavigatedTo(navigationParameters);
            await LoadData();
        }
    }
}
