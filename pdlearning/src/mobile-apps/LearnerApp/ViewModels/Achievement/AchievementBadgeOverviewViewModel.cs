using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using LearnerApp.Common;
using LearnerApp.Controls.LearnerObservableCollection;
using LearnerApp.Models.Achievement;
using LearnerApp.Services.ServiceManager;
using LearnerApp.ViewModels.Achievement.Badge;
using LearnerApp.ViewModels.Achievement.ECertificate;
using LearnerApp.ViewModels.Base;
using Xamarin.Forms;

namespace LearnerApp.ViewModels.Achievement
{
    public class AchievementBadgeOverviewViewModel : BaseViewModel
    {
        private readonly AchievementBackendServiceManager _achievementBackendSm;

        private int _totalCount;
        private bool _isLoading = true;
        private AchievementBadgeInfoDto[] _badgeInfoData;

        public AchievementBadgeOverviewViewModel()
        {
            _achievementBackendSm = new AchievementBackendServiceManager();
        }

        public LearnerObservableCollection<AchievementBadgeItemViewModel> Badges { get; private set; }

        public int TotalCount
        {
            get => _totalCount;
            set
            {
                _totalCount = value;
                RaisePropertyChanged(() => TotalCount);
            }
        }

        public bool IsLoading
        {
            get => _isLoading;
            set
            {
                _isLoading = value;
                RaisePropertyChanged(() => IsLoading);
                RaisePropertyChanged(() => IsShowList);
            }
        }

        public bool IsShowList
        {
            get => _isLoading || TotalCount > 0;
        }

        public ICommand ShowAllCommand => new Command(ShowAll);

        public async Task LoadData()
        {
            IsLoading = true;
            if (_badgeInfoData.IsNullOrEmpty())
            {
                _badgeInfoData = await _achievementBackendSm.GetAchievementBadgesInfo();
                if (_badgeInfoData == null)
                {
                    IsLoading = false;
                    return;
                }
            }

            var list =
                await _achievementBackendSm.GetAchievementBadges(_badgeInfoData);

            if (list == null)
            {
                IsLoading = false;
                TotalCount = 0;
                return;
            }

            var badges =
                list.Items.Take(10).Select(x => new AchievementBadgeItemViewModel(x)).ToList();

            TotalCount = list.TotalCount;
            Badges = new LearnerObservableCollection<AchievementBadgeItemViewModel>(badges);
            RaisePropertyChanged(() => Badges);
            IsLoading = false;
        }

        private async void ShowAll()
        {
            await NavigationService.NavigateToAsync<AchievementBadgePageViewModel>();
        }
    }
}
